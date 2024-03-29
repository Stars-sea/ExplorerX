﻿using ExplorerX.CLI;
using ExplorerX.Components;
using ExplorerX.Data;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;


namespace ExplorerX.Pages {
	/// <summary>
	/// <para>Common Pages</para>
	/// <para>通用页面</para>
	/// </summary>
	public sealed partial class CommonPage : Page {
		public CommonPage() {
			InitializeComponent();
		}

		#region Menu Items

		private static WriteableBitmap GetSmallIconFromPath(string path) {
			ShellFile    file   = new(path);
			Trace.WriteLine(file.SmallIconHandle);
			using Bitmap bitmap = Bitmap.FromHicon((IntPtr)file.SmallIconHandle);
			
			MemoryStream ms = new();
			bitmap.Save(ms, ImageFormat.MemoryBmp);
			ms.Position = 0;

			WriteableBitmap writeable = new(bitmap.Width, bitmap.Height);
			writeable.SetSource(ms.AsRandomAccessStream());
			return writeable;
		}

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
				select new {
					Path = name,
					Item = new NavigationViewItem {
						Content = $"{drive.VolumeLabel}({name})",
						Tag     = new Uri($"page:///ExplorerX.Pages.ItemsViewPage?{name}")
					}
				};
			
			Drives.MenuItems.Clear();
			foreach (var item in items) {
				Drives.MenuItems.Add(item.Item);
				item.Item.Icon = new ImageSourceIcon {
					ImageSource = GetSmallIconFromPath(item.Path)
				};
			}
		}

		private void OnQucikAccessLoading(FrameworkElement sender, object args) 
			=> ReloadQuickAccess();

		private void OnDrivesLoading(FrameworkElement sender, object args)
			=> ReloadDrives();

		private void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
			NavigationViewItem item = (NavigationViewItem)args.SelectedItem;
			Uri uri = item.Tag switch {
				string str	=> new Uri(str),
				Uri	   uri1	=> uri1,
				_			=> throw new UriFormatException($"{item.Tag} is an invalid url")
			};
			
			Type?  pageType = Type.GetType(uri.AbsolutePath.TrimStart('/'));
			string param    = Uri.UnescapeDataString(uri.Query.TrimStart('?'));
			
			Trace.Assert(pageType is not null);

			if (pageType == MainFrame.SourcePageType &&
				MainFrame.Content is IModifiable modifiable)
				modifiable.Modify(param);
			else MainFrame.Navigate(pageType, param);
		}
		#endregion

		private void OnNavigated(object sender, NavigationEventArgs e) {
			if (MainFrame.Content is IModifiable modifiable)
				modifiable.Modify(e.Parameter);
		}
	}
}
