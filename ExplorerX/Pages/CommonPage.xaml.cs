using ExplorerX.Data;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

		#region Menu Items

		public void ReloadQuickAccess() {
			var items = RegistryManagers.QuickAccess.Select(
				p => new NavigationViewItem() {
					Content = p.Key,
					Tag     = new Uri($"page:///ExplorerX.Pages.ItemsViewPage?{p.Value}")
					// TODO: Icon
				}
			);

			QuickAccess.MenuItems.Clear();
			foreach (NavigationViewItem item in items)
				QuickAccess.MenuItems.Add(item);
		}

		public void ReloadDrives() {
			var items =
				from drive in DriveInfo.GetDrives()
				where drive.IsReady
				let name = drive.Name[..^1]
				orderby name
				select new NavigationViewItem {
					Content	= $"{drive.VolumeLabel}({name})",
					Tag		= new Uri($"page:///ExplorerX.Pages.ItemsViewPage?{name}")
				};

			Drives.MenuItems.Clear();
			foreach (NavigationViewItem item in items)
				Drives.MenuItems.Add(item);
		}

		private void OnQucikAccessLoading(FrameworkElement sender, object args) 
			=> ReloadQuickAccess();

		private void OnDrivesLoading(FrameworkElement sender, object args)
			=> ReloadDrives();

		private void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
			NavigationViewItem item = (NavigationViewItem)args.SelectedItem;
			Uri uri = item.Tag switch {
				string str => new Uri(str),
				Uri uri1 => uri1,
				_ => throw new InvalidDataException($"{item.Tag} is an invalid url")
			};

			Type? pageType = Type.GetType(uri.AbsolutePath.Trim('/'));
			string param   = uri.Query.Trim('?');

			MainFrame.Navigate(pageType ?? throw new NullReferenceException(), param);
		}

#endregion
		private void OnNavigated(object sender, NavigationEventArgs e) {
			if (sender is not Frame frame || frame.Content is not IModifiablePage page) return;
			page.Modify(e.Parameter);
		}

		#region Inner Class
		private sealed class NavItemEqualityComparer : EqualityComparer<object> {

			public override bool Equals(object? x, object? y) {
				if (x == y) return true;

				if (x is NavigationViewItem first && y is NavigationViewItem second)
					return first.Tag == second.Tag;

				return false;
			}

			public override int GetHashCode([DisallowNull] object obj) 
				=> obj == null ? 0 : obj.GetHashCode();
		}
#endregion
	}
}
