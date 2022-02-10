using ExplorerX.Data;


namespace ExplorerX.Events.Handlers {
	internal static class AppInitiatingHandler {
		internal static void Init() {
			AppLifecircle.AppInitiating += Handler;
		}

		private static async void Handler(App app) {
			// Handlers binding
			WindowClosedHandler.Init();
			RegistryLoadingHandler.Init();

			await RegistryManagers.Reload();

			// UNDONE: Scan other assemblies & run its code, like bukkit plugin in Minecraft
		}
	}
}
