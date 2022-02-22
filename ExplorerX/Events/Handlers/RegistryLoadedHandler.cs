using ExplorerX.Data;

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
				["$Desktop"]	= CLI.KnownFolder.Desktop.Path,
				["$Pictures"]	= CLI.KnownFolder.Pictures.Path,
				["$Videos"]		= CLI.KnownFolder.Videos.Path,
				["$Music"]		= CLI.KnownFolder.Music.Path,
				["$Documents"]	= CLI.KnownFolder.Documents.Path,
				["$Downloads"]	= CLI.KnownFolder.Downloads.Path
			});
		}

		private static void LoadingDefaultQuickAcess(object? sender, RegistryLoadedArgs<string> args) {
			if (args.IsSuccess) return;

			args.AddAll(new Dictionary<string, string> {
				["Desktop"]		= "$Desktop",
				["Pictures"]	= "$Pictures",
				["Videos"]		= "$Videos",
				["Music"]		= "$Music",
				["Documents"]	= "$Documents",
				["Downloads"]	= "$Downloads"
			});
		}
	}
}
