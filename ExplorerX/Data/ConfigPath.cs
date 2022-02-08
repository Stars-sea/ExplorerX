using System;
using System.IO;

using Windows.Storage;


namespace ExplorerX.Data {
	public static class ConfigPath {
		// We can't create Roaming folder in this way. So we use ApplicationData in UWP
		// 由于 WinUI 夹带私货, 所以我们用 UWP 的 ApplicationData
		// private static readonly string Roaming = Environment.SpecialFolder.ApplicationData.Get();

		private static readonly ApplicationData data = ApplicationData.Current;

		/// <summary>
		/// <para>Parent directory of all app config file</para>
		/// <para>所有配置文件的父目录</para>
		/// </summary>
		public static readonly string LocalPath = data.LocalFolder.Path;

		/// <summary>
		/// <para>Parent directory of all registries</para>
		/// <para>所有注册表的父目录</para>
		/// </summary>
		public static readonly string Registry = Combine(LocalPath, "Registry");

		private static string Combine(params string[] segments) {
			string path = Path.Combine(segments);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}
	}

	internal static class SpecialFolderExt {
		public static string Get(this Environment.SpecialFolder folder)
			=> Environment.GetFolderPath(folder);
	}
}
