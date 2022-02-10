using ExplorerX.Data;


namespace ExplorerX.Events.Handlers {

	internal static class AppInitiatingHandler {
		internal static async void InitAsync() {
			WindowClosedHandler.Init();
			RegistryFileNotFoundHandler.Init();

			await RegistryManagers.Reload();

			// UNDONE: Scan other assemblies & run its code, like bukkit plugin in Minecraft
		}
	}
}
