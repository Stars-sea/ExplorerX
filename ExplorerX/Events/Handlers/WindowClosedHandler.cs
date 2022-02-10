using ExplorerX.Data;

using Microsoft.UI.Xaml;

namespace ExplorerX.Events.Handlers {
	internal static class WindowClosedHandler {
		internal static void Init() {
			MainWindow? window = App.Window;
			if (window is not null)
				window.Closed += Handler;
		}

		private static async void Handler(object sender, WindowEventArgs args) {
			await RegistryManagers.SaveAll();
		}
	}
}
