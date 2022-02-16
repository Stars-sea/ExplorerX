using ExplorerX.Data;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.IO;
using System.Linq;


namespace ExplorerX.Pages {
	/// <summary>
	/// <para>Common Pages</para>
	/// <para>通用页面</para>
	/// </summary>
	public sealed partial class CommonPage : Page {
		private record NavTag(Type PageType, object Param);

		public CommonPage() {
			InitializeComponent();
		}

		private void OnPanelLoading(FrameworkElement sender, object args) {
			if (sender is not NavigationView view) return;

			// Home
			NavigationViewItem home = new() {
				Content		= "Home",
				Tag			= new NavTag(typeof(ItemsRootPage), ItemsRootPage.ViewMode.All),
				Icon		= new SymbolIcon(Symbol.Home),
				IsSelected	= true
			};
			view.MenuItems.Add(home);
			

			if (RegistryManagers.QuickAccess.Count != 0) {
				// QuickAccess
				NavigationViewItem item = new() {
					Content	= "Quick Access",
					Tag		= new NavTag(typeof(ItemsRootPage), ItemsRootPage.ViewMode.QuickAccess),
					Icon	= new SymbolIcon(Symbol.Favorite),
				};
				// Children
				foreach ((string name, string path) in RegistryManagers.QuickAccess)
					item.MenuItems.Add(new NavigationViewItem {
						Content	= name,
						Tag		= new NavTag(typeof(ItemsViewPage), path)
						// TODO: Icon
					});

				view.MenuItems.Add(item);
			}


			// Drives
			NavigationViewItem drives = new() {
				Content	= "Drives",
				Tag		= new NavTag(typeof(ItemsRootPage), ItemsRootPage.ViewMode.Drives),
				Icon	= new SymbolIcon(Symbol.MapDrive),
			};
			// Children
			foreach (DriveInfo info in DriveInfo.GetDrives().Where(d => d.IsReady))
				drives.MenuItems.Add(new NavigationViewItem {
					Content = $"{info.VolumeLabel}({info.Name[..^1]})",
					Tag		= new NavTag(typeof(ItemsViewPage), info.RootDirectory.FullName)
					// TODO: Icon
				});
			view.MenuItems.Add(drives);
		}

		private void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
			NavigationViewItem item = (NavigationViewItem)args.SelectedItem;
			if (item.Tag is NavTag tag)
				MainFrame.Navigate(tag.PageType, tag.Param);
		}

		private void OnNavigating(object sender, NavigatingCancelEventArgs e) {
			// UNDONE: Set Page's property
		}
	}
}
