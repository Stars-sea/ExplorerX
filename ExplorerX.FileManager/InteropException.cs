using System;

namespace ExplorerX.FileManager {
	public class InteropException : Exception {
		public int ErrorCode { get; init; }

		public InteropException(string msg, int code) : base(msg) {
			ErrorCode = code;
		}
	}
}
