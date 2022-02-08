using ExplorerX.Data;
using ExplorerX.Events;

using System;

using Windows.Foundation;

namespace ExplorerX {
	/// <summary>
	/// <para>Application life cricle.</para>
	/// <para>应用程序生命周期 (存放全局事件)</para>
	/// </summary>
	public static class AppLifecircle {
		public static event Action<App>? AppInitiating;
		public static event TypedEventHandler<App, RegistryLoadingArgs<object>>? LoadingVariables;
		public static event TypedEventHandler<App, RegistryLoadingArgs<string>>? LoadingQuickAcess;

		private static void RaiseEvent<TSender, TArgs>(
			TypedEventHandler<TSender, TArgs>? @event, TSender sender, TArgs args
		) {
			if (@event is not null)
				@event(sender, args);
		}

		private static void RaiseEvent<T>(Action<T>? @event, T sender) {
			if (@event is not null)
				@event(sender);
		}

		private static void OnRegistryLoading<T>(
			TypedEventHandler<App, RegistryLoadingArgs<T>>? @event,
			App sender, RegistryManager<T> registry
		) where T : notnull {
			RegistryLoadingArgs<T> args = new(registry.Register, registry.ContainsKey);
			RaiseEvent(@event, sender, args);
		}

		internal static void OnAppInitiating(App sender)
			=> RaiseEvent(AppInitiating, sender);

		internal static void OnLoadingVariables(App sender)
			=> OnRegistryLoading(LoadingVariables, sender, RegistryManagers.VariablePool);

		internal static void OnLoadingQuickAccess(App sender)
			=> OnRegistryLoading(LoadingQuickAcess, sender, RegistryManagers.QuickAccess);
	}
}
