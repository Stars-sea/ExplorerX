using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using Vanara.PInvoke;
using PathHelper = System.IO.Path;
using SafeHICON  = Vanara.PInvoke.User32.SafeHICON;

namespace ExplorerX.FileManager {

	public class LocalFileInfo : LocalSystemInfo {

		#region Private Fields

		private static readonly WeakDict<string, ImageSource> SmallIconDict = new();
		private static readonly WeakDict<string, ImageSource> LargeIconDict = new();

		#endregion Private Fields

		#region Properties

		public override bool Exists => File.Exists(Path);

		public string? Extension => PathHelper.GetExtension(Path);

		public BinaryType BinaryType => BinaryTypeHelper.GetBinaryType(Path);
		public bool IsExecutable => BinaryType.IsExecutable();

		public override FileInfo InnerInfo => new(Path);

		#endregion Properties

		public LocalFileInfo(Uri uri) : base(uri) {
		}

		#region Override methods

		public override CreateResponse Create() {
			if (base.Exists)
				return new CreateResponse(false, null, new IOException(
					$"A directory/file has had the same name with the file. ({Path})"));

			if (!PathHelper.IsPathRooted(Path))
				return new CreateResponse(false, null, new IOException($"'{Path}' isn't a absolute path."));

			try {
				return new CreateResponse(true, File.Create(Path), null);
			}
			catch (Exception error) {
				return new CreateResponse(false, null, error);
			}
		}

		protected override Task<FileSize> GetSizeAsync()
			=> Task.Run(() => new FileSize(InnerInfo.Length));

		private uint GetExecutableFileIcons(out ImageSource small, out ImageSource large) {
			uint result = Shell32.ExtractIconEx(Path, 0, 1, out SafeHICON[] hsIcon, out SafeHICON[] hlIcon);

			small = hsIcon[0].ToBitmapSource();
			large = hsIcon[0].ToBitmapSource();

			SmallIconDict[Path] = small;
			LargeIconDict[Path] = large;

			User32.DestroyIcon(hsIcon[0]);
			User32.DestroyIcon(hlIcon[0]);

			return result;
		}

		// TODO: 代码耦合度太高
		protected override ImageSource GetSmallIcon() {
			string pathOrExtension = IsExecutable ? Path : PathHelper.GetExtension(Path);

			if (SmallIconDict.ContainsKey(pathOrExtension) &&
				SmallIconDict.TryGetValue(pathOrExtension, out ImageSource? source))
				return source;
			else SmallIconDict.Clean();

			return IsExecutable && GetExecutableFileIcons(out ImageSource small, out _) != uint.MaxValue
				? (SmallIconDict[pathOrExtension] = small)
				: (SmallIconDict[pathOrExtension] = base.GetSmallIcon());
		}

		protected override ImageSource GetLargeIcon() {
			string pathOrExtension = IsExecutable ? Path : PathHelper.GetExtension(Path);

			if (LargeIconDict.ContainsKey(pathOrExtension) &&
				LargeIconDict.TryGetValue(pathOrExtension, out ImageSource? source))
				return source;
			else LargeIconDict.Clean();

			return IsExecutable && GetExecutableFileIcons(out _, out ImageSource large) != uint.MaxValue
				? (LargeIconDict[pathOrExtension] = large)
				: (LargeIconDict[pathOrExtension] = base.GetLargeIcon());
		}

		#endregion Override methods
	}
}