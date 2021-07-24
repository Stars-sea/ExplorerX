using System.Windows;

namespace ExplorerX.Controls.Events {
	public delegate void ChangedPropertyEventHandler<T>(object sender, ChangedPropertyEventArgs<T> args);

	public class ChangedPropertyEventArgs<T> : RoutedEventArgs {
		public T OldValue { get; init; }
		public T NewValue { get; init; }

		public ChangedPropertyEventArgs(RoutedEvent @event, T oldValue, T newValue) : base(@event) {
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}
