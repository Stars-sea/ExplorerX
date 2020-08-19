using ExplorerX.Wrapper;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ExplorerX {

	public static class FileOperations {

		public static void OpenFile(params string[] filePath)
			=> OpenFile(filePath.Select(p => ResourceContainer.Create(p).Info).ToArray());

		public static void OpenFile(params FileSystemInfo[] infos) {
			foreach (FileSystemInfo info in infos)
				OpenFile(info);
		}

		private static void OpenFile(FileSystemInfo info) {
			if (!ShellExcute.Execute(info.FullName)) {
				Trace.WriteLine($"[{DateTime.Now:g}] Failed to execute file {info.FullName}.");
				Process.Start("Rundll32.exe", $"shell32,OpenAs_RunDLL {info.FullName}");
			}
		}

		public static void MoveFile(FileSystemInfo infos, DirectoryInfo dir) {
		}

		public static void CopyFile(FileSystemInfo infos, FileSystemInfo info) {
		}

		public static void DeleteFile(bool isRecoveries = false, params ResourceContainer[] info) {
		}
	}
}