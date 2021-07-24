using System.Windows;

namespace ExplorerX.Controls.Events {

	public delegate void ChangedItemEventHandler<T>(object sender, ChangedItemEventArgs<T> args);

	public class ChangedItemEventArgs<T> : RoutedEventArgs {
		public T Item { get; init; }

		public ChangedItemEventArgs(RoutedEvent @event, T item) : base(@event) {
			Item = item;
		}
	}
}