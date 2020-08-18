using ExplorerX.Wrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace ExplorerX {
	public class ResourceContainer {
		private static readonly string[] Units = new string[] {
			"B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB", "BB"
		};
		private static readonly Dictionary<string, WeakReference<ResourceContainer>> Cache =
			new Dictionary<string, WeakReference<ResourceContainer>>();

		public FileSystemInfo Info { get; set; }

		public ImageSource ImageSource => ShellInfoContainer.Create(Info.FullName);
		public string Name => Info.Name;
		public string LastWriteTime => Info.LastWriteTime.ToString("g");
		public string InfoType => Info is FileInfo ? "文件" : "文件夹";

		public string Size {
			get {
				if (Info is FileInfo fileInfo) {
					double size = fileInfo.Length;
					int times = 0;
					while (size >= 1024 && times < Units.Length - 1) {
						size /= 1024D;
						times++;
					}
					return $"{size:N} {Units[times]}";
				}
				return "";
			}
		}

		public string Path => Info.FullName;

		private ResourceContainer(FileSystemInfo info) {
			Info = info;
			Cache[info.FullName] = new WeakReference<ResourceContainer>(this);
		}

		private ResourceContainer(string path) {
			if (File.Exists(path))
				Info = new FileInfo(path);
			else if (Directory.Exists(path))
				Info = new DirectoryInfo(path);
			throw new FileNotFoundException($"Not found file/directory {path}");
		}
		~ResourceContainer() => Cache.Remove(Info.FullName);

		public static ResourceContainer Create(FileSystemInfo info) {
			if (Cache.TryGetValue(info.FullName, out var reference))
				if (reference.TryGetTarget(out ResourceContainer? container) && container != null)
					return container;
			return new ResourceContainer(info);
		}

		public static ResourceContainer Create(string path) {
			if (Cache.TryGetValue(path, out var reference))
				if (reference.TryGetTarget(out ResourceContainer? container) && container != null) 
					return container;
			return new ResourceContainer(path);
		}

		public override string ToString() => Path;

		public static implicit operator string(ResourceContainer container)	=> container.Path;
		public static implicit operator ResourceContainer(string path)		=> Create(path);
	}
}
