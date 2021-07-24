using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using PathHelper = System.IO.Path;

namespace ExplorerX.FileManager {

	public class LocalDirectoryInfo : LocalSystemInfo {

		#region Properties

		public LocalSystemInfo[] Children =>
			Directory.EnumerateFileSystemEntries(Path).Select(GetInstance).ToArray();

		public override DirectoryInfo InnerInfo => new(Path);

		#endregion Properties

		public LocalDirectoryInfo(Uri uri) : base(uri) {
		}

		public override CreateResponse Create() {
			if (base.Exists)
				return new CreateResponse(false, null, new IOException(
					$"A directory/file has had the same name with the file. ({Path})"));

			if (!PathHelper.IsPathRooted(Path))
				return new CreateResponse(false, null, new IOException($"'{Path}' isn't a absolute path."));

			try {
				Directory.CreateDirectory(Path);
				return new CreateResponse(true, null, null);
			}
			catch (Exception error) {
				return new CreateResponse(false, null, error);
			}
		}

		#region Override Method

		protected async override Task<FileSize> GetSizeAsync() {
			FileSize size = new(0);
			foreach (LocalSystemInfo child in Children)
				size += await child.Size;
			return size;
		}

		#endregion Override Method
	}
}