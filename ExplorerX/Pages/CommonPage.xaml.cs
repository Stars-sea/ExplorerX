using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using System.IO;

namespace ExplorerX.Pages {
	/// <summary>
	/// <para>Common Pages</para>
	/// <para>通用页面</para>
	/// </summary>
	public sealed partial class CommonPage : Page {
		public CommonPage() {
			InitializeComponent();
		}

		private void OnPanelLoading(FrameworkElement sender, object args) {
			if (sender is not NavigationView view) return;

			NavigationViewItem home = new() {
				Content	= "Home",
				Tag     = "$Home",
				Icon	= new SymbolIcon(Symbol.Home)
			};

			if (RegistryManagers.QuickAccess.Count != 0) {
				NavigationViewItem item = new() {
					Content	= "Quick Access",
					Tag		= "$QuickAccess",
					Icon	= new SymbolIcon(Symbol.Favorite),
				};
				foreach ((string name, string path) in RegistryManagers.QuickAccess)
					item.MenuItems.Add(new NavigationViewItem {
						Content	= name,
						Tag		= path
						// TODO: Icon
					});

				view.MenuItems.Add(item);
			}

			NavigationViewItem drives = new() {
				Content		= "Drives",
				Icon		= new SymbolIcon(Symbol.MapDrive),
			};
			foreach (DriveInfo info in DriveInfo.GetDrives())
				drives.MenuItems.Add(new NavigationViewItem {
					Content = $"{info.VolumeLabel}({info.Name[..^1]})",
					Tag		= info.RootDirectory.FullName
					// TODO: Icon
				});
			view.MenuItems.Add(drives);
		}
	}
}
