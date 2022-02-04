using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


namespace ExplorerX.Pages {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ItemsRootPage : Page {
		public enum ViewMode {
			All,
			Drives,
			QuickAccess
		};

		#region Dependency Props

		public ViewMode Mode {
			get { return (ViewMode)GetValue(ModeProperty); }
			set { SetValue(ModeProperty, value); }
		}

		public static readonly DependencyProperty ModeProperty = 
			DependencyProperty.Register("Mode", typeof(ViewMode),
				typeof(ItemsRootPage), new PropertyMetadata(ViewMode.All));

		#endregion

		public ItemsRootPage() {
			InitializeComponent();
		}
	}
}
