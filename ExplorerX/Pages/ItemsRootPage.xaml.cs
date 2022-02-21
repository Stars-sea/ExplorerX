using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


namespace ExplorerX.Pages {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ItemsRootPage : Page, IModifiablePage {
		#region Dependency Props

		public ItemsRootPageMode Mode {
			get { return (ItemsRootPageMode)GetValue(ModeProperty); }
			set { SetValue(ModeProperty, value); }
		}

		public static readonly DependencyProperty ModeProperty = 
			DependencyProperty.Register("Mode", typeof(ItemsRootPageMode),
				typeof(ItemsRootPage), new PropertyMetadata(ItemsRootPageMode.All));

		#endregion

		public ItemsRootPage() {
			InitializeComponent();
		}

		public void Modify(object? param)
			=> Modify(param switch {
				ItemsRootPageMode mode => mode,
				string name => System.Enum.Parse<ItemsRootPageMode>(name),
				_ => ItemsRootPageMode.All
			});

		public void Modify(ItemsRootPageMode mode) => Mode = mode;
	}

	public enum ItemsRootPageMode {
		All,
		Drives,
		QuickAccess
	};
}
