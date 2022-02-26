using ExplorerX.Data;


namespace ExplorerX.Events.Handlers {

	internal static class AppInitiatingHandler {
		internal static async void InitAsync() {
			CLI.NativeManager.Startup();
			WindowClosedHandler.Init();

			RegistryChangedHandler.Init();
			RegistryLoadedHandler.Init();

			await RegistryManagers.Reload();

			// UNDONE: Scan other assemblies & run its code, like bukkit plugin in Minecraft
		}
	}
}
