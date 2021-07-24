using ExplorerX.Controls.Events;
using ExplorerX.Controls.Helpers;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace ExplorerX.Controls {

	/// <summary>
	/// 弹性 TabControl
	/// </summary>
	[TemplatePart(Name = "PART_HeadersHost", Type = typeof(Panel))]
	[TemplatePart(Name = "PART_NewTabButton", Type = typeof(ButtonBase))]
	public class ElasticTabControl : TabControl {

		#region Static Members

		private static readonly RegisterHelper register = new(typeof(ElasticTabControl));

		private static readonly DependencyPropertyKey CanNewTabPropertyKey;

		public static readonly DependencyProperty CanAddChildProperty;
		public static readonly DependencyProperty CanNewTabProperty;
		public static readonly DependencyProperty HeadersAnimationFuncProperty;

		public static readonly RoutedEvent        AddedItemEvent;
		public static readonly RoutedEvent        RemovedItemEvent;

		static ElasticTabControl() {
			CanNewTabPropertyKey         = register.RegisterReadOnly(nameof(CanNewTab), new PropertyMetadata(true));
			CanAddChildProperty          = register.Register(nameof(CanAddChild), new PropertyMetadata(new Func<ElasticTabControl, bool>(_ => true)));
			CanNewTabProperty            = CanNewTabPropertyKey.DependencyProperty;
			HeadersAnimationFuncProperty = register.Register(nameof(HeadersAnimationFunc), new FrameworkPropertyMetadata(new CircleEase { EasingMode = EasingMode.EaseInOut }));

			AddedItemEvent               = register.RegisterRoutedEvent(nameof(AddedItem), RoutingStrategy.Bubble);
			RemovedItemEvent             = register.RegisterRoutedEvent(nameof(RemovedItem), RoutingStrategy.Bubble);

			DefaultStyleKeyProperty.OverrideMetadata(typeof(ElasticTabControl), new FrameworkPropertyMetadata(typeof(ElasticTabControl)));
		}

		#endregion Static Members

		#region Properties

		public Func<ElasticTabControl, bool> CanAddChild {
			get => (Func<ElasticTabControl, bool>) GetValue(CanAddChildProperty);
			set => SetValue(CanAddChildProperty, value);
		}

		public bool CanNewTab => CanAddChild(this);

		public IEasingFunction HeadersAnimationFunc {
			get => (IEasingFunction) GetValue(HeadersAnimationFuncProperty);
			set => SetValue(HeadersAnimationFuncProperty, value);
		}

		protected ButtonBase NewTabButton => (ButtonBase) Template.FindName("PART_NewTabButton", this);
		protected Panel HeadersHost => (Panel) Template.FindName("PART_HeadersHost", this);
		protected double HeadersTotalWidth => ActualWidth - NewTabButton.Width;

		/// <summary>
		/// 储存等待鼠标离开任务, 避免关闭多个 ElasticTabItem 时创建过多任务
		/// </summary>
		private Task? waitForMouseLeavingTask;

		#endregion Properties

		#region Events

		public event ChangedItemEventHandler<ElasticTabItem> AddedItem {
			add => AddHandler(AddedItemEvent, value);
			remove => RemoveHandler(AddedItemEvent, value);
		}

		public event ChangedItemEventHandler<ElasticTabItem> RemovedItem {
			add => AddHandler(RemovedItemEvent, value);
			remove => RemoveHandler(RemovedItemEvent, value);
		}

		#endregion Events

		#region Event Handlers

		public ElasticTabControl() {
			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs args) {
			base.EndInit();

			NewTabButton.Click += (_, _) => NewTab().IsSelected = true;

			NewTab().IsSelected = true;
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
			base.OnRenderSizeChanged(sizeInfo);
			ResizeItems();
		}

		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);

			RoutedEvent? @event = e.Action switch {
				NotifyCollectionChangedAction.Add or
				NotifyCollectionChangedAction.Replace => AddedItemEvent,
				NotifyCollectionChangedAction.Remove  => RemovedItemEvent,
				_                                     => null
			};

			IList? items = e.NewItems;
			if (@event is not null && items is not null) {
				foreach (ElasticTabItem item in items.OfType<ElasticTabItem>())
					RaiseEvent(new ChangedItemEventArgs<ElasticTabItem>(@event, item));
			}
		}

		#endregion Event Handlers

		#region Item Operations

		public void ResizeItems(double averageWidth) {
			foreach (ElasticTabItem item in Items.OfType<ElasticTabItem>()) {
				item.BeginDoubleAnimation(WidthProperty, averageWidth, func: HeadersAnimationFunc);
				item.Width = averageWidth;
			}
		}

		public void ResizeItems() => ResizeItems(HeadersTotalWidth / Items.Count);

		/// <summary>
		/// 在鼠标离开 HeadersHost 之后, 重置标签大小
		/// </summary>
		public async void ResizeItemsAfterMouseLeft() {
			if (waitForMouseLeavingTask is null || waitForMouseLeavingTask.IsCompleted) {
				waitForMouseLeavingTask = Task.Run(() => {
					while (HeadersHost.IsMouseOver) Thread.Sleep(20);
					Dispatcher.Invoke(ResizeItems);
				});
				await waitForMouseLeavingTask;
			}
		}

		public void AddItem(ElasticTabItem item) {
			double averageWidth = HeadersTotalWidth / (Items.Count + 1);
			ResizeItems(averageWidth);

			Items.Add(item);
			item.BeginDoubleAnimation(WidthProperty, averageWidth, 0.15, HeadersAnimationFunc);
			item.Width = averageWidth;
		}

		public async void RemoveItem(ElasticTabItem item) {
			await item.BeginDoubleAnimationAsync(WidthProperty, 0, func: HeadersAnimationFunc);
			Items.Remove(item);

			ResizeItemsAfterMouseLeft();
		}

		public ElasticTabItem NewTab() {
			if (CanNewTab) {
				ElasticTabItem item = new();
				AddItem(item);
				return item;
			}

			throw new InvalidOperationException("Can't new tab.");
		}

		#endregion Item Operations
	}
}