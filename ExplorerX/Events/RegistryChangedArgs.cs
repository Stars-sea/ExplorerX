using System.Collections.Generic;


namespace ExplorerX.Events {
	public record class RegistryChangedArgs<T>(
		IDictionary<string, T> ChangedEntries,
		RegistryChangedArgs<T>.OperationMode Operation
	) where T : notnull {
		public enum OperationMode {
			Register,
			Unregister
		}
	}
}
