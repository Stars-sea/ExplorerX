using ExplorerX.Data;

using System;
using System.Collections.Generic;


namespace ExplorerX.Events.Handlers {
	internal static class RegistryFileNotFoundHandler {
		internal static void Init() {
			RegistryManagers.VariablePool.NotFoundFile += LoadingDefaultVariables;
			RegistryManagers.QuickAccess.NotFoundFile += LoadingDefaultQuickAcess;
		}

		private static void LoadingDefaultVariables(object? sender, RegistryEventArgs<object> args) {
			// We do not use AppDataPaths because of its limit
			args.AddAll(new Dictionary<string, object> {
				["$Desktop"]	= Environment.SpecialFolder.Desktop.Get(),
				["$Pictures"]	= Environment.SpecialFolder.MyPictures.Get(),
				["$Videos"]		= Environment.SpecialFolder.MyVideos.Get(),
				["$Music"]		= Environment.SpecialFolder.MyMusic.Get(),
				["$Documents"]	= Environment.SpecialFolder.MyDocuments.Get()
			});
			// TODO: Add Downloads
		}

		private static void LoadingDefaultQuickAcess(object? sender, RegistryEventArgs<string> args) {
			args.AddAll(new Dictionary<string, string> {
				["Desktop"]		= "$Desktop",
				["Pictures"]	= "$Pictrues",
				["Videos"]		= "$Videos",
				["Music"]		= "$Music",
				["Documents"]	= "$Documents"
			});
		}
	}
}
