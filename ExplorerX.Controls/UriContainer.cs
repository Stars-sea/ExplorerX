using ExplorerX.Controls.Events;
using ExplorerX.Controls.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExplorerX.Controls {
	/// <summary>
	/// Uri 地址容器
	/// </summary>
	public class UriContainer : ItemsControl {
		#region Static Members
		public static readonly Uri IndexUri = new("explorerx://Index");

		private static readonly RegisterHelper register = new(typeof(UriContainer));

		public static readonly DependencyProperty UriProperty;

		public static readonly RoutedEvent        ChangedUriEvent;
		public static readonly RoutedEvent        AddedChildUriEvent;
		public static readonly RoutedEvent        RemovedChildUriEvent;

		static UriContainer() {
			UriProperty          = register.Register(nameof(Uri), new PropertyMetadata(IndexUri, OnChangedUri));

			ChangedUriEvent      = register.RegisterRoutedEvent(nameof(ChangedUri), RoutingStrategy.Bubble);
			AddedChildUriEvent   = register.RegisterRoutedEvent(nameof(AddedChildUri), RoutingStrategy.Bubble);
			RemovedChildUriEvent = register.RegisterRoutedEvent(nameof(RemovedChildUri), RoutingStrategy.Bubble);

			DefaultStyleKeyProperty.OverrideMetadata(typeof(UriContainer), new FrameworkPropertyMetadata(typeof(UriContainer)));
		}
		#endregion

		#region Property
		public Uri Uri {
			get => (Uri) GetValue(UriProperty);
			set => SetValue(UriProperty, value);
		}
		#endregion

		#region Events
		public event ChangedPropertyEventHandler<Uri> ChangedUri {
			add => AddHandler(ChangedUriEvent, value);
			remove => RemoveHandler(ChangedUriEvent, value);
		}

		public event ChangedItemEventHandler<Uri> AddedChildUri {
			add => AddHandler(AddedChildUriEvent, value);
			remove => RemoveHandler(AddedChildUriEvent, value);
		}

		public event ChangedItemEventHandler<Uri> RemovedChildUri {
			add => AddHandler(RemovedChildUriEvent, value);
			remove => RemoveHandler(RemovedChildUriEvent, value);
		}
		#endregion

		#region Event Handlers
		public UriContainer() {
			AddedChildUri   += OnAddedChildUri;
			RemovedChildUri += OnRemovedChildUri;
		}

		private static void OnChangedUri(DependencyObject @object, DependencyPropertyChangedEventArgs args) {
			UriContainer container = (UriContainer) @object;
			Uri          oldValue  = (Uri) args.OldValue;
			Uri          newValue  = (Uri) args.NewValue;
			container.RaiseEvent(new ChangedPropertyEventArgs<Uri>(ChangedUriEvent, oldValue, newValue));
			
			// Call AddedChildUriEvent / RemovedChildUriEvent
			var diffierences = oldValue.Segments.Except(newValue.Segments);
			if (diffierences is null || !diffierences.Any()) return;

			Queue<string> queue = new(diffierences);
			RoutedEvent? @event = null;
			Uri?        current = null;

			if (newValue.IsBaseOf(oldValue)) {
				@event  = AddedChildUriEvent;
				current = oldValue;
			}
			else if (oldValue.IsBaseOf(newValue)) {
				@event  = RemovedChildUriEvent;
				current = newValue;
			}

			if (@event is not null && current is not null) {
				while (queue.TryDequeue(out string child)) {
					container.RaiseEvent(new ChangedItemEventArgs<Uri>(@event, new(current, child)));
				}
			}
		}

		private void OnAddedChildUri(object sender, ChangedItemEventArgs<Uri> args) {
			
		}

		private void OnRemovedChildUri(object sender, ChangedItemEventArgs<Uri> args) {

		}

		#endregion

		#region Items Operations
		public Uri Append(Uri child) => Uri = new(Uri, child);
		#endregion
	}
}
