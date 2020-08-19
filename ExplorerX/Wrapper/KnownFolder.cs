using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ExplorerX.Wrapper {

	public class KnownFolder {
		public Guid FolderGuid { get; set; }

		public string Path {
			get {
				SHGetKnownFolderPath(FolderGuid, 0, IntPtr.Zero, out string path);
				return path;
			}
		}

		public FileInfo FileInfo => new FileInfo(Path);

		public KnownFolder(Guid guid) => FolderGuid = guid;

		public KnownFolder(string guid) : this(new Guid(guid)) { }

		/// <summary>
		/// 取得文件夹路径
		/// </summary>
		[DllImport("shell32.dll", CharSet = CharSet.Unicode)]
		internal static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid,
			uint dwFlags, IntPtr hToken, out string pszPath);

		public static implicit operator KnownFolder(string guid) => new KnownFolder(guid);
	}
}