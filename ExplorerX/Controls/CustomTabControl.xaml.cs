using ExplorerX.Helpers;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ExplorerX.Controls {

	/// <summary>
	/// CustomTabControl.xaml 的交互逻辑
	/// </summary>
	public partial class CustomTabControl : TabControl, ICustomAnimatable {

		public static readonly DependencyProperty ReservedWidthProperty =
			DependencyProperty.Register("ReservedWidth", typeof(double), typeof(CustomTabControl), new PropertyMetadata(45D));

		public static readonly DependencyProperty IsGlobalProperty =
			DependencyProperty.Register("IsGlobal", typeof(bool), typeof(CustomTabControl), new PropertyMetadata(false));

		public static readonly DependencyProperty IsJumpToNewTabProperty =
			DependencyProperty.Register("IsJumpToNewTab", typeof(bool), typeof(CustomTabControl), new PropertyMetadata(false));

		public static readonly DependencyProperty ContentTypeProperty =
			DependencyProperty.Register("ContentType", typeof(Type), typeof(CustomTabControl));

		public double ReservedWidth {
			get => (double)GetValue(ReservedWidthProperty);
			set => SetValue(ReservedWidthProperty, value);
		}

		public bool IsGlobal {
			get => (bool)GetValue(IsGlobalProperty);
			set => SetValue(IsGlobalProperty, value);
		}

		public IEasingFunction DefaultEasingFunction {
			get => (IEasingFunction)GetValue(ICustomAnimatable.DefaultEasingFunctionProperty);
			set => SetValue(ICustomAnimatable.DefaultEasingFunctionProperty, value);
		}

		public bool IsJumpToNewTab {
			get => (bool)GetValue(IsJumpToNewTabProperty);
			set => SetValue(IsJumpToNewTabProperty, value);
		}

		public Type ContentType {
			get => (Type)GetValue(ContentTypeProperty);
			set => SetValue(ContentTypeProperty, value);
		}

		public event RoutedEventHandler AllItemsClosed;

		public CustomTabControl() {
			InitializeComponent();
			AllItemsClosed += OnAllItemsClosed;
		}

		/// <summary>
		/// 重置所有子项的宽
		/// </summary>
		/// <param name="itemWidth">每个标签页宽</param>
		public void ResetItemsWidth(double itemWidth) {
			foreach (TabItem item in Items.OfType<TabItem>()) {
				item.BeginDoubleAnimation(WidthProperty, itemWidth, function: DefaultEasingFunction);
				item.Width = itemWidth;
			}
		}

		public void ResetItemsWidth() => ResetItemsWidth(GetSingleItemWidth());

		/// <summary>
		/// 取得子项平均宽
		/// </summary>
		/// <param name="count">模拟的标签页个数</param>
		/// <returns>子项平均宽</returns>
		public double GetSingleItemWidth(int count)
			=> (ActualWidth - (ActualWidth > ReservedWidth ? ReservedWidth : 0)) / count;

		public double GetSingleItemWidth() => GetSingleItemWidth(Items.Count);

		public virtual async void AddItems(params TabItem[] items) {
			foreach (TabItem item in items) {
				double itemWidth = GetSingleItemWidth(Items.Count + 1);
				ResetItemsWidth(itemWidth);

				await Task.Delay(150);
				item.BeginDoubleAnimation(WidthProperty, itemWidth, 0.15, DefaultEasingFunction);
				item.Width = itemWidth;
				Items.Add(item);
			}
		}

		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);

			if (Items.Count == 0) AllItemsClosed(this, new RoutedEventArgs());
			if (IsJumpToNewTab && e.Action == NotifyCollectionChangedAction.Add)
				SelectedIndex = e.NewStartingIndex;

			ResetItemsWidth();
		}

		protected virtual void OnAllItemsClosed(object sender, RoutedEventArgs e) {
			if (IsGlobal) Application.Current.MainWindow.Close();
		}

		public CustomTabItem NewTab() {
			CustomTabItem item = new CustomTabItem { Content = CreateContent() };
			AddItems(item);
			return item;
		}

		private async void OnLoaded(object sender, RoutedEventArgs e) {
			if (IsGlobal && Items.Count == 0) {
				await Task.Delay(100);
				NewTab().IsSelected = true;
			}
			Button button = (Button)Template.FindName("NewTabButton", this);
			button.Click += (sender, e) => NewTab();
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs e) => ResetItemsWidth();

		private object? CreateContent() {
			if (ContentType is null) return null;

			return Activator.CreateInstance(ContentType) ??
				throw new NullReferenceException($"创建 Content ({ContentType.FullName} 实例) 失败");
		}
	}
}