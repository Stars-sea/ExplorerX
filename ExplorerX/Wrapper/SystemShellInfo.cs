using ExplorerX.Exceptions;
using ExplorerX.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ExplorerX.Wrapper {

	/// <summary>
	/// 获取文件(夹)的图标
	/// (包装 SHGetFileInfo, DestroyIcon)
	/// </summary>
	public sealed class ShellInfoContainer : IDisposable {

		private static readonly Dictionary<string, WeakReference<ShellInfoContainer>> Cache =
			new Dictionary<string, WeakReference<ShellInfoContainer>>();

		internal readonly SystemShellInfo.ShellInfo InnerInfo;
		public   readonly string PathOrExtension;

		private ImageSource?    source;
		public ImageSource Source => source ??= GetIconSource();

		private ShellInfoContainer(string path, bool largeIcon = false) {
			InnerInfo = GetShellInfo(path, largeIcon, !Directory.Exists(PathOrExtension = path));
			Cache[path] = new WeakReference<ShellInfoContainer>(this);
		}

		~ShellInfoContainer() => Dispose();

		public static ShellInfoContainer Create(string fullpath, bool largeIcon = false) {
			string pathOrExtension = GetPathOrExtension(fullpath);
			if (Cache.TryGetValue(pathOrExtension, out WeakReference<ShellInfoContainer>? value) && value != null)
				if (value.TryGetTarget(out ShellInfoContainer? container) && container != null)
					return container;
			return new ShellInfoContainer(pathOrExtension, largeIcon);
		}

		private SystemShellInfo.ShellInfo GetShellInfo(string path, bool largeIcon, bool isFile) {
			SystemShellInfo.Flag flag;
			if (isFile)
				flag = largeIcon ? SystemShellInfo.FileLargeIconFlags : SystemShellInfo.FileIconFlags;
			else flag = largeIcon ? SystemShellInfo.DirLargeIconFlags : SystemShellInfo.DirIconFlags;

			if (SystemShellInfo.SHGetFileInfo(path, (uint)(isFile ? 256 : 0), out SystemShellInfo.ShellInfo info,
					SystemShellInfo.ShellInfo.Size, flag) != IntPtr.Zero && info.hIcon != IntPtr.Zero) {
				return info;
			}
			throw ShellInfoException.GetCase(path);
		}

		private static string GetPathOrExtension(string path) {
			if (File.Exists(path))
				return Path.HasExtension(path) ? Path.GetExtension(path) : " ";
			else if (Directory.Exists(path)) return path;
			throw new FileNotFoundException($"Not found file/directory {path}");
		}

		private ImageSource GetIconSource() {
			try {
				ImageSource source = Imaging.CreateBitmapSourceFromHIcon(InnerInfo.hIcon,
						Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

				if (source.CanFreeze) source.Freeze();
				return source;
			}
			catch (NullReferenceException) {
				Trace.Fail($"[{DateTime.Now:g}] Get file/directory icon {PathOrExtension} error.");
				Trace.WriteLine($"[{DateTime.Now:g}] Program will use the default icon.");

				return (ExeIconContainer)$"@{Environment.SpecialFolder.Windows.GetPath()}\\System32\\SHELL32.dll,0";
			}
		}

		public static implicit operator ImageSource(ShellInfoContainer container) => container.GetIconSource();

		public static implicit operator ShellInfoContainer(string path) => Create(path);

		public void Dispose() {
			if (!Cache.Remove(PathOrExtension))
				SystemShellInfo.DestroyIcon(InnerInfo.hIcon);
		}
	}

	/// <summary>
	/// 通过 "@[executable file], index" 的方式获取图标
	/// (包装 ExtractIcon, DestroyIcon)
	/// </summary>
	public sealed class ExeIconContainer : IDisposable {

		private static readonly Dictionary<(string, uint), WeakReference<ExeIconContainer>> Cache =
			new Dictionary<(string, uint), WeakReference<ExeIconContainer>>();

		private static readonly Regex SpiltRegex = new Regex(@"^@(?<path>.+?),\s*?(?<index>\d+)$");

		public readonly string  ExePath;
		public readonly uint    Index;
		public readonly IntPtr  IconHandle;

		private ImageSource?    source;
		public ImageSource Source => source ??= GetIconSource();

		private ExeIconContainer(string exePath, uint index) {
			IntPtr hIcon = SystemShellInfo.ExtractIcon(App.InstanceHandle, ExePath = exePath, Index = index);
			if (hIcon == IntPtr.Zero)
				throw new NullReferenceException($"Get [@{ExePath},{Index}] error.");
			else if (hIcon.ToInt32() == 1)
				throw new FileNotFoundException($"Not found executable file {ExePath}.");
			IconHandle = hIcon;
			Cache[(ExePath, Index)] = new WeakReference<ExeIconContainer>(this);
		}

		~ExeIconContainer() => Dispose();

		public static ExeIconContainer Create(string exePath, uint index) {
			if (Cache.ContainsKey((exePath, index)))
				if (Cache.TryGetValue((exePath, index), out WeakReference<ExeIconContainer>? value) && value != null)
					if (value.TryGetTarget(out ExeIconContainer? container) && container != null)
						return container;
			return new ExeIconContainer(exePath, index);
		}

		private ImageSource GetIconSource() {
			ImageSource source = Imaging.CreateBitmapSourceFromHIcon(IconHandle,
				Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

			if (source.CanFreeze) source.Freeze();
			return source;
		}

		public static implicit operator ExeIconContainer(string pathAndIndex) {
			GroupCollection groups = SpiltRegex.Match(pathAndIndex).Groups;
			string exePath  = groups["path"].Value;
			uint   index    = Convert.ToUInt32(groups["index"].Value);
			return Create(exePath, index);
		}

		public static implicit operator ExeIconContainer((string, uint) tuple) => Create(tuple.Item1, tuple.Item2);

		public static implicit operator ImageSource(ExeIconContainer container) => container.Source;

		public void Dispose() {
			if (!Cache.Remove((ExePath, Index)))
				SystemShellInfo.DestroyIcon(IconHandle);
		}
	}

	internal static class SystemShellInfo {

		/// <summary>
		/// 获取文件的 ShellInfo
		/// <see cref="https://docs.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-shgetfileinfoa"/>
		/// </summary>
		/// <param name="pszPath">文件(夹)路径或后缀, 无文件后缀则用空格</param>
		/// <param name="dwFileAttributes">文件属性</param>
		/// <param name="psfi">输出的结果</param>
		/// <param name="cbfileInfo"> <paramref name="cbfileInfo"/> 的大小</param>
		/// <param name="uFlags">获取文件的哪些东西</param>
		/// <returns>如果为 0, 则获取失败</returns>
		[DllImport("Shell32.dll")]
		internal static extern IntPtr SHGetFileInfo(
			string pszPath,
			uint dwFileAttributes,
			out ShellInfo psfi,
			uint cbfileInfo,
			Flag uFlags
		);

		/// <summary>
		/// 获取可执行/库文件的资源图标
		/// <see cref="https://docs.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-extracticona"/>
		/// </summary>
		/// <param name="hInst">应用程序实例句柄 (App 类里有)</param>
		/// <param name="pszExeFileName">可执行/库文件路径</param>
		/// <param name="nIconIndex">图标的标号</param>
		/// <returns>图标句柄</returns>
		[DllImport("Shell32.dll")]
		internal static extern IntPtr ExtractIcon(IntPtr hInst, string pszExeFileName, uint nIconIndex);

		/// <summary>
		/// 释放图标
		/// <see cref="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-destroyicon"/>
		/// </summary>
		/// <param name="hIcon">图标句柄</param>
		/// <returns>是否释放成功</returns>
		[DllImport("User32.dll")]
		internal static extern bool DestroyIcon(IntPtr hIcon);

		[StructLayout(LayoutKind.Sequential)]
		internal struct ShellInfo {
			public IntPtr   hIcon;
			public int      iIcon;
			public uint     dwAttributes;

			[MarshalAs(UnmanagedType.LPStr, SizeConst = 260)]
			public string   szDisplayName;

			[MarshalAs(UnmanagedType.LPStr, SizeConst = 80)]
			public string   szTypeName;

			public static readonly uint Size = (uint)Marshal.SizeOf(typeof(ShellInfo));
		}

		internal enum Flag {
			SmallIcon           = 0x00000001,
			LargeIcon           = 0x00000000,
			Icon                = 0x00000100,
			DisplayName         = 0x00000200,
			Typename            = 0x00000400,
			SysIconIndex        = 0x00004000,
			UseFileAttributes   = 0x00000010
		}

		internal static readonly Flag FileIconFlags = Flag.Icon | Flag.SmallIcon | Flag.UseFileAttributes;
		internal static readonly Flag FileLargeIconFlags = Flag.Icon | Flag.LargeIcon | Flag.UseFileAttributes;

		internal static readonly Flag DirIconFlags = Flag.Icon | Flag.SmallIcon;
		internal static readonly Flag DirLargeIconFlags = Flag.Icon | Flag.LargeIcon;
	}
}