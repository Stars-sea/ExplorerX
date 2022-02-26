namespace ExplorerX.Events.Handlers {
	internal static class WindowClosedHandler {
		internal static void Init() {
			App.Window!.Closed += OnWindowClosed;
		}

		private static void OnWindowClosed(object sender, Microsoft.UI.Xaml.WindowEventArgs args) {
			CLI.NativeManager.Shutdown();
		}
	}
}
