using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Vanara.PInvoke;
using SHFILEINFO = Vanara.PInvoke.Shell32.SHFILEINFO;
using SHGFI = Vanara.PInvoke.Shell32.SHGFI;

namespace ExplorerX.FileManager {

	public abstract class LocalSystemInfo {

		#region Private fields

		private string? actuallyName;
		private string? displayName;
		private string? typeName;

		private ImageSource? smallIcon;
		private ImageSource? largeIcon;

		private Task<FileSize>? size;
		//private FileSystemWatcher? watcher;

		#endregion Private fields

		#region Properties

		public Uri Uri { get; init; }
		public string Path => Uri.LocalPath;

		public string ActuallyName => actuallyName  ??= GetActuallyName();
		public string DispalyName => displayName   ??= GetDisplayName();
		public string TypeName => typeName      ??= GetTypeName();

		public Task<FileSize> Size => size ??= GetSizeAsync();

		public ImageSource SmallIcon => smallIcon ??= GetSmallIcon();
		public ImageSource LargeIcon => largeIcon ??= GetLargeIcon();

		public virtual FileAttributes Attributes => File.GetAttributes(Path);

		public virtual DateTime LastAccessTime => File.GetLastAccessTime(Path);
		public virtual DateTime LastWriteTime => File.GetLastWriteTime(Path);

		public virtual bool Exists => File.Exists(Path) || Directory.Exists(Path);
		public abstract FileSystemInfo InnerInfo { get; }

		#endregion Properties

		protected LocalSystemInfo(Uri uri) {
			if (uri.Scheme != Uri.UriSchemeFile)
				throw new InvalidOperationException($"Invalid scheme {uri.Scheme}. ({uri.OriginalString})");
			Uri = uri;

			//if (Uri.IsAbsoluteUri) {
			//	if (File.Exists(Path))
			//		watcher = new FileSystemWatcher(
			//			Path[..Path.LastIndexOfAny(new char[] { '\\', '/' })],
			//			PathHelper.GetFileName(Path) ?? throw new NullReferenceException()
			//		);
			//	else if (Directory.Exists(Path))
			//		watcher = new FileSystemWatcher(Path);
			//}
		}

		public virtual void Flush() {
			actuallyName = null;
			displayName  = null;
			typeName     = null;
			smallIcon    = null;
			largeIcon    = null;
			size         = null;
		}

		/// <summary>
		/// Create a file/directory.
		/// </summary>
		/// <returns>It will return true if succeed; otherwise, false.</returns>
		public abstract CreateResponse Create();

		public static LocalSystemInfo GetInstance(string path) => GetInstance(new Uri(path));

		public static LocalSystemInfo GetInstance(Uri uri) {
			string path = uri.LocalPath;
			if (File.Exists(path))
				return new LocalFileInfo(uri);
			if (Directory.Exists(path))
				return new LocalDirectoryInfo(uri);

			throw new FileNotFoundException($"No such file/directory. ({path})");
		}

		#region Property implementations

		protected virtual string GetActuallyName() => Uri.Segments.Last();

		internal IntPtr TryGetInfo(SHGFI flags, out SHFILEINFO info) {
			info = new();
			return Shell32.SHGetFileInfo(Path, Attributes, ref info, SHFILEINFO.Size, flags);
		}

		private static bool Check(IntPtr code, string msg)
			=> code == IntPtr.Zero ? throw new InteropException(msg, code.ToInt32()) : true;

		protected virtual string GetDisplayName() {
			Check(TryGetInfo(SHGFI.SHGFI_DISPLAYNAME, out SHFILEINFO info),
				  $"Can't get the display name of file '{Path}'.");
			return info.szDisplayName;
		}

		protected virtual string GetTypeName() {
			Check(TryGetInfo(SHGFI.SHGFI_TYPENAME, out SHFILEINFO info),
				  $"Can't get the type name of file '{Path}'.");
			return info.szTypeName;
		}

		protected abstract Task<FileSize> GetSizeAsync();

		private ImageSource GetIcon(SHGFI flags) {
			Check(TryGetInfo(flags, out SHFILEINFO info),
				  $"Can't get icon handle of file {Path}.");
			HICON       icon    = info.hIcon;
			ImageSource source  = icon.ToBitmapSource();

			User32.DestroyIcon(icon);
			return source;
		}

		protected virtual ImageSource GetSmallIcon() => GetIcon(SHGFI.SHGFI_ICON | SHGFI.SHGFI_SMALLICON);

		protected virtual ImageSource GetLargeIcon() => GetIcon(SHGFI.SHGFI_ICON | SHGFI.SHGFI_LARGEICON);

		#endregion Property implementations
	}
}