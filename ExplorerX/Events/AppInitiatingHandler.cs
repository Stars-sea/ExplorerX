namespace ExplorerX.Events {
	public static class AppInitiatingHandler {
		internal static void Init() {
			AppLifecircle.AppInitiating += Handler;
		}

		private static void Handler(App app) {
			// UNDONE: Scan other assmebly & run its code, like bukkit plugin in Minecraft
		}
	}
}
