using ExplorerX.Exceptions;
using ExplorerX.Helpers;
using ExplorerX.Wrapper;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ExplorerX.Controls {

	/// <summary>
	/// CustomTabItem.xaml 的交互逻辑
	/// </summary>
	public partial class CustomTabItem : TabItem, ICustomAnimatable {
		public Image Icon => (Image)Template.FindName("Icon", this);
		protected TextBox InputPathBox => (TextBox)Template.FindName("InputPathBox", this);
		protected ContentPresenter HeaderPresenter => (ContentPresenter)Template.FindName("Presenter", this);

		public ImageSource IconSource {
			get => Icon.Source;
			set => Icon.Source = value;
		}

		public IEasingFunction DefaultEasingFunction {
			get => (IEasingFunction)GetValue(ICustomAnimatable.DefaultEasingFunctionProperty);
			set => SetValue(ICustomAnimatable.DefaultEasingFunctionProperty, value);
		}

		protected TabControl ParentTC => (TabControl)Parent;

		public CustomTabItem() : this("NewTab") { }

		public CustomTabItem(string header) {
			InitializeComponent();
			Header = header;
		}

		public void SetTabIcon(string path) {
			try {
				IconSource = ShellInfoContainer.Create(path);
			}
			catch (ShellInfoException e) { e.TraceError(); }
		}

		protected override void OnHeaderChanged(object oldHeader, object newHeader) {
			base.OnHeaderChanged(oldHeader, newHeader);
			if (newHeader is string header && Directory.Exists(header)) {
				Header = new PathsList(header, (s, e) => {
					if (e.OriginalSource is DirectoryInfo dirInfo)
						if (Content is ResourcesViewer viewer)
							viewer.CurrentDir = dirInfo;
				});
				SetTabIcon(header);
				InputPathBox.Text = header;
			}
		}

		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			e.Handled = true;
			if (IsSelected) OnSelected(e);
		}

		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) => Close();

		public virtual void Close() {
			if (ParentTC != null) ParentTC.Items.Remove(this);
		}

		private void OnMouseDown(object sender, MouseButtonEventArgs e) {
			if (e.MiddleButton == MouseButtonState.Pressed) Close();

			if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount >= 2) {
				HeaderPresenter.Visibility = Visibility.Collapsed;
				InputPathBox.Visibility = Visibility.Visible;
			}
		}

		/// <summary>
		/// 当 InputPathBox 的一些事件触发后执行
		/// </summary>
		private void Jump() {
			if (Content is ResourcesViewer viewer) {
				HeaderPresenter.Visibility  = Visibility.Visible;
				InputPathBox.Visibility     = Visibility.Collapsed;
				viewer.CurrentDir = new DirectoryInfo(InputPathBox.Text);
			}
		}

		private void InputPathBox_LostFocus(object sender, RoutedEventArgs e) => Jump();

		private void InputPathBox_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) Jump();
		}
	}
}