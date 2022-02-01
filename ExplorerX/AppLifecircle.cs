using ExplorerX.Events;

using Windows.Foundation;

namespace ExplorerX {
	/// <summary>
	/// <para>Application life cricle.</para>
	/// <para>应用程序生命周期 (存放全局事件)</para>
	/// </summary>
	public static class AppLifecircle {
		public static event TypedEventHandler<App, LoadingQuickAccessItemsArgs>? LoadingQuickAcess;

		private static void RaiseEvent<TSender, TArgs>(
			TypedEventHandler<TSender, TArgs>? @event, TSender sender, TArgs args
		) {
			if (@event is not null)
				@event(sender, args);
		}

		internal static void OnLoadingQuickAccess(App sender) {
			LoadingQuickAccessItemsArgs args = new(
				RegistryManagers.QuickAccess.Register, 
				RegistryManagers.QuickAccess.Exist
			);

			RaiseEvent(LoadingQuickAcess, sender, args);
		}
	}
}
