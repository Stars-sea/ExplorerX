using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace ExplorerX.FileManager {
	public record CreateResponse(bool Succeed, Stream? Stream, Exception? Error) {
		public void Throw() {
			if (Error is not null) {
				throw Error;
			}
		}

		public bool TryGetStream([NotNullWhen(true)][MaybeNullWhen(false)] out Stream stream)
			=> (stream = Stream) is not null;

		public bool TryGetError([NotNullWhen(true)][MaybeNullWhen(false)] out Exception error)
			=> (error = Error) is not null;
	}
}