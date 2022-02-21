using ExplorerX.Data;

using PInvoke;

using System;
using System.Collections.Generic;


namespace ExplorerX.Events.Handlers {
	internal static class RegistryLoadedHandler {
		internal static void Init() {
			RegistryManagers.VariablePool.RegLoaded += LoadingDefaultVariables;
			RegistryManagers.QuickAccess.RegLoaded += LoadingDefaultQuickAcess;
		}

		private static void LoadingDefaultVariables(object? sender, RegistryLoadedArgs<object> args) {
			if (args.IsSuccess) return;

			// We do not use AppDataPaths because of its limit
			args.AddAll(new Dictionary<string, object> {
				["$Desktop"]	= Environment.SpecialFolder.Desktop.Get(),
				["$Pictures"]	= Environment.SpecialFolder.MyPictures.Get(),
				["$Videos"]		= Environment.SpecialFolder.MyVideos.Get(),
				["$Music"]		= Environment.SpecialFolder.MyMusic.Get(),
				["$Documents"]	= Environment.SpecialFolder.MyDocuments.Get(),
				["$Downloads"]	= Shell32.SHGetKnownFolderPath(Shell32.KNOWNFOLDERID.FOLDERID_Downloads)
			});
		}

		private static void LoadingDefaultQuickAcess(object? sender, RegistryLoadedArgs<string> args) {
			if (args.IsSuccess) return;

			args.AddAll(new Dictionary<string, string> {
				["Desktop"]		= "$Desktop",
				["Pictures"]	= "$Pictrues",
				["Videos"]		= "$Videos",
				["Music"]		= "$Music",
				["Documents"]	= "$Documents",
				["Downloads"]	= "$Downloads"
			});
		}
	}
}
