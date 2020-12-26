using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ExplorerX.Helpers {

	public static class SpecialFolderHelper {

		public static string GetPath(this Environment.SpecialFolder specialFolder)
			=> Environment.GetFolderPath(specialFolder);
	}

	public static class FileSystemInfoHelper {

		public static bool HasAttributes(this FileSystemInfo info, params FileAttributes[] attributes) {
			foreach (FileAttributes attribute in attributes)
				if ((info.Attributes & attribute) != attribute) return false;
			return true;
		}

		public static bool IsHidden(this FileSystemInfo info)
			=> info.HasAttributes(FileAttributes.Hidden);

		public static FileSystemInfo ToFileSystemInfo(string path) {
			if (File.Exists(path))
				return new FileInfo(path);
			else if (Directory.Exists(path))
				return new DirectoryInfo(path);
			throw new FileNotFoundException($"Not found file/directory {path}");
		}
	}

	public static class TraceHelper {

		public static void TraceError(this Exception e) {
			Trace.TraceError("[{0:g}]: {1}: {2}", DateTime.Now, e.GetType().Name, e.Message);
			Trace.Indent();
			if (e.StackTrace != null) {
				Trace.WriteLine("StackTrace: ");
				Trace.Indent();
				foreach (string line in NormalizeStackTrace(e.StackTrace))
					Trace.WriteLine(line);
				Trace.Unindent();
			}
			if (e.InnerException != null) {
				Trace.WriteLine("InnerException: ");
				Trace.Indent();
				e.InnerException.TraceError();
				Trace.Unindent();
			}
			Trace.Unindent();
		}

		private static IEnumerable<string> NormalizeStackTrace(string stackTrace) => stackTrace.Split("\n").Select(s => s.Trim());
	}
}