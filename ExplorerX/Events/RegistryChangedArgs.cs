using System.Collections.Generic;


namespace ExplorerX.Events {
	public record class RegistryChangedArgs<T>(
		IReadOnlyDictionary<string, T> ChangedEntries,
		RegistryChangedArgs<T>.OperationMode Operation
	) where T : notnull {
		public enum OperationMode {
			Modify,
			Register,
			Unregister
		}
	}
}
