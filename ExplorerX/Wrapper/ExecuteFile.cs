using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ExplorerX.Wrapper {

	public class ShellExcute {
		private string? exePath;

		public string? ExePath {
			get => exePath;
			set {
				if (File.Exists(value))
					exePath = value;
				else if (Directory.Exists(value))
					exePath = value;
				else throw new FileNotFoundException($"Not found file/directory {value}");
			}
		}

		public string Params { get; set; }

		private string? workingDirectory;

		public string? WorkingDirectory {
			get => workingDirectory;
			set {
				if (Directory.Exists(value))
					workingDirectory = value;
				else throw new DirectoryNotFoundException($"Not found directory {value}");
			}
		}

		public ShowMode ShowMode { get; set; }

		public ShellExcute(string exeFile, string param, string workingDirectory, ShowMode mode = ShowMode.Show) {
			ExePath = exeFile;
			Params = param;
			WorkingDirectory = workingDirectory;
			ShowMode = mode;
		}

		public ShellExcute(string exeFile, string param = "", ShowMode mode = ShowMode.Show) :
			this(exeFile, param, Environment.CurrentDirectory, mode) { }

		public bool Execute()
			=> Execute(ExePath ?? throw new NullReferenceException(), Params,
				WorkingDirectory ?? throw new NullReferenceException(), ShowMode);

		public bool Execute(string newParam) {
			Params = newParam;
			return Execute();
		}

		public static bool Execute(string exePath, string param, string workingDir, ShowMode mode = ShowMode.Show)
			=> ExecuteFile.ShellExecute(IntPtr.Zero, "", exePath, param, workingDir, mode).ToInt32() > 32;

		public static bool Execute(string exePath, string param = "", ShowMode mode = ShowMode.Show)
			=> Execute(exePath, param, Environment.CurrentDirectory, mode);
	}

	/// <summary>
	/// <see cref="https://docs.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-shellexecutea#sw_hide-0"/>
	/// </summary>
	public enum ShowMode {
		Hide            = 0,
		Maximize        = 3,
		Minimize        = 6,
		Restore         = 9,
		Show            = 5,
		ShowDefault     = 10,
		ShowMaximize    = 3,
		ShowMinimize    = 2,
		ShowMinNoActive = 7,
		ShowNA          = 8,
		ShowNoActive    = 4,
		ShowNormal      = 1
	}

	internal static class ExecuteFile {

		/// <summary>
		/// 执行文件
		/// <see cref="https://docs.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-shellexecutea"/>
		/// </summary>
		/// <param name="hwnd">父窗口的句柄, 可以是 0</param>
		/// <param name="lpOperation">操作 (edit open find explore print runas null)</param>
		/// <param name="lpFile">执行文件</param>
		/// <param name="lpParameters">参数</param>
		/// <param name="lpDirectory">启动目录</param>
		/// <param name="nShowCmd">显示模式</param>
		/// <returns>如果大于 32, 操作成功, 反之操作失败</returns>
		[DllImport("Shell32.dll")]
		internal static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation,
			string lpFile, string lpParameters, string lpDirectory, ShowMode nShowCmd);
	}
}