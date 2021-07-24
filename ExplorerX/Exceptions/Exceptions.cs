using System;

namespace ExplorerX.Exceptions {

	public class ShellInfoException : Exception {

		public ShellInfoException() : base("Get info of file/directory error.") {
		}

		public ShellInfoException(string msg) : base(msg) {
		}

		public ShellInfoException(string msg, Exception innerException) : base(msg, innerException) {
		}

		public static ShellInfoException GetCase(string path)
			=> new ShellInfoException($"Get info of {path} error.");
	}
}