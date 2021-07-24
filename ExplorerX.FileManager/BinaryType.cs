using Vanara.PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SCS = Vanara.PInvoke.Kernel32.SCS;

namespace ExplorerX.FileManager {
	public enum BinaryType : uint {
		Win32 = 0,      // SCS_32BIT_BINARY
		Win64 = 6,      // SCS_64BIT_BINARY
		MSDos = 1,      // SCS_DOS_BINARY
		OS216 = 5,      // SCS_OS216_BINARY
		MSPIF = 3,      // SCS_PIF_BINARY
		POSIX = 4,      // SCS_POSIX_BINARY
		Win16 = 2,      // SCS_WOW_BINARY

		WinDll = 7,     // ERROR_BAD_EXE_FORMAT
		Normal = 8      // Non-executable file
	}

	public static class BinaryTypeHelper {
		public static BinaryType GetBinaryType(string path) {
			if (Kernel32.GetBinaryType(path, out SCS type)) {
				if (path.EndsWith(".dll"))
					return BinaryType.WinDll;

				uint value = (uint) type;
				return (BinaryType) value;
			}
			return BinaryType.Normal;
		}

		public static bool IsExecutable(this BinaryType type) {
			uint value = (uint) type;
			return value <= 6;
		}
	}
}
