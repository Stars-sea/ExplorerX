using ExplorerX.Data;

using System;

namespace ExplorerX.Events.Handlers {
	internal static class RegistryLoadingHandler {
		internal static void Init() {
			AppLifecircle.LoadingQuickAcess += LoadingQuickAcessHandler;
			AppLifecircle.LoadingVariables += LoadingVariablesHandler;
		}

		private static void LoadingVariablesHandler(App sender, RegistryLoadingArgs<object> args) {
			if (RegistryManagers.VariablePool.Count == 0) {
				// We do not use AppDataPaths because of its limit
				args.Add("$Desktop", Environment.SpecialFolder.Desktop.Get());
				args.Add("$Pictures", Environment.SpecialFolder.MyPictures.Get());
				args.Add("$Videos", Environment.SpecialFolder.MyVideos.Get());
				args.Add("$Music", Environment.SpecialFolder.MyMusic.Get());
				args.Add("$Documents", Environment.SpecialFolder.MyDocuments.Get());
				// TODO: Add Downloads
			}
		}

		private static void LoadingQuickAcessHandler(App sender, RegistryLoadingArgs<string> args) {
			if (RegistryManagers.QuickAccess.Count == 0) {
				string[] names = new string[] {
					"Desktop",	"Pictures",	"Videos",
					"Music",	"Documents"
				};
				foreach (string name in names)
					args.Add(name, $"${name}");
			}
		}
	}
}
