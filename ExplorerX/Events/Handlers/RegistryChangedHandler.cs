using ExplorerX.Data;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ExplorerX.Events.Handlers {
	internal static class RegistryChangedHandler {
		private readonly static List<IStorable> timerCache = new();

		internal static void Init() {
			RegistryManagers.VariablePool.RegChanged += OnRegistryChanged<object>;
			RegistryManagers.QuickAccess.RegChanged += OnRegistryChanged<string>;
		}

		private static void OnRegistryChanged<T>(
			object? sender, RegistryChangedArgs<T> e
		) where T : notnull {
			if (e.ChangedEntries.Any() && sender is Registry<T> registry) {
				if (timerCache.Contains(registry)) return;

				Timer timer = new(1500) { AutoReset = false };
				timer.Elapsed += async (_, _) => await SaveAsync(registry);
				timerCache.Add(registry);
				timer.Start();
			}
		}

		private static async ValueTask SaveAsync<T>(Registry<T> registry) where T : notnull {
			if (await ((IStorable)registry).SaveAsync(ConfigPath.Registry, 3))
				Debug.Print($"{registry} was saved to \"{ConfigPath.Registry}\" successfully");
			else
				Debug.Print($"Failed to save {registry} to \"{ConfigPath.Registry}\"");

			timerCache.Remove(registry);
		}
	}
}
