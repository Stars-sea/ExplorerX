using ExplorerX.Controls;
using ExplorerX.Helpers;
using ExplorerX.Wrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExplorerX {

	/// <summary>
	/// ResourcesView.xaml 的交互逻辑
	/// </summary>
	public partial class ResourcesViewer : UserControl {
		public  readonly FileSystemWatcher Watcher = new FileSystemWatcher();

		private DirectoryInfo? currentDir;

		public DirectoryInfo CurrentDir {
			get => currentDir ?? throw new NullReferenceException();
			set {
				if (!value.Exists)
					throw new DirectoryNotFoundException($"Not found directory {value.FullName}");

				currentDir = value;
				Reload();

				Application.Current.MainWindow.Title    = CurrentDir.Name;
				if (Parent is TabItem item) item.Header = CurrentDir.FullName;
				Watcher.Path = CurrentDir.FullName;
				Watcher.EnableRaisingEvents = true;
			}
		}

		public ResourcesViewer() {
			InitializeComponent();
			Watcher.Renamed += FileSystemEventHandler;
			Watcher.Created += FileSystemEventHandler;
			Watcher.Deleted += FileSystemEventHandler;
			Watcher.Changed += FileSystemEventHandler;
		}

		~ResourcesViewer() => Watcher.Dispose();

		private void LoadShortcuts(object sender, RoutedEventArgs e) {
			// TODO: 改为读取配置文件, 和加入回收站
			IEnumerable<DirectoryInfo> infos = new string?[] {
				Environment.SpecialFolder.Desktop.GetPath(),
				KnownFolders.Downloads.Path,
				Environment.SpecialFolder.MyDocuments.GetPath(),
				Environment.SpecialFolder.MyPictures.GetPath(),
				Environment.SpecialFolder.MyMusic.GetPath(),
				Environment.SpecialFolder.MyVideos.GetPath(),
				Environment.GetEnvironmentVariable("OneDriveConsumer")
			}.Where(p => p != null && Directory.Exists(p))
			 .Select(p => new DirectoryInfo(p ?? throw new NullReferenceException()));

			ShortcutsBox.ItemsSource = infos.Select(info => new LeftItem(info));
		}

		private void LoadDrives(object sender, RoutedEventArgs e)
			=> DrivesBox.ItemsSource =
				from drive in DriveInfo.GetDrives()
				where drive.IsReady
				select new LeftItem(drive);

		private void OnResourceSelected(object sender, MouseButtonEventArgs e) {
			if (e.ChangedButton != MouseButton.Left ||
				ViewGrid.SelectedItems.Count < 1) return;

			DirectoryInfo? dirInfo = null;
			foreach (ResourceContainer container in GetSelectedItems()) {
				if (container.Info is DirectoryInfo info)
					dirInfo = info;
				else FileOperations.OpenFile(container.Info);
			}

			if (dirInfo != null) CurrentDir = dirInfo;
		}

		private void OnDriveSelected(object sender, MouseButtonEventArgs e) {
			if (DrivesBox.SelectedItem is LeftItem item && item.DelegateDir != null) {
				if (ShortcutsBox.SelectedItem is ListBoxItem selectedShortcut)
					selectedShortcut.IsSelected = false;

				CurrentDir = item.DelegateDir;
			}
			else ((LeftItem)DrivesBox.SelectedItem).IsSelected = false;
		}

		private void OnShortcutSelected(object sender, MouseButtonEventArgs e) {
			if (ShortcutsBox.SelectedItem is LeftItem item && item.DelegateDir != null) {
				if (DrivesBox.SelectedItem is ListBoxItem selectedDrive)
					selectedDrive.IsSelected = false;

				CurrentDir = item.DelegateDir;
			}
			else ((LeftItem)ShortcutsBox.SelectedItem).IsSelected = false;
		}

		public async void Reload() {
			try {
				ViewGrid.ItemsSource = await GetItemsAsync();
			}
			catch (UnauthorizedAccessException e) { e.TraceError(); }
		}

		public IEnumerable<ResourceContainer> GetSelectedItems() => ViewGrid.SelectedItems.OfType<ResourceContainer>();

		public IEnumerable<FileSystemInfo> GetSelectedInfos() => GetSelectedItems().Select(c => c.Info);

		public Task<IEnumerable<ResourceContainer>> GetItemsAsync() => Task.Run(GetItems);

		public IEnumerable<ResourceContainer> GetItems() => GetItemsExceptAttributes(CurrentDir, FileAttributes.Hidden);

		public static IEnumerable<ResourceContainer> GetItems(DirectoryInfo dir)
			=> GetItemsExceptAttributes(dir, FileAttributes.Hidden);

		public static IEnumerable<ResourceContainer> GetItemsExceptAttributes(DirectoryInfo dir, params FileAttributes[] attributes)
			=> GetItems(dir, info => !info.HasAttributes(attributes));

		public static IEnumerable<ResourceContainer> GetItems(DirectoryInfo dir, params FileAttributes[] attributes)
			=> GetItems(dir, info => info.HasAttributes(attributes));

		public static IEnumerable<ResourceContainer> GetItems(DirectoryInfo dir, Func<FileSystemInfo, bool> verification)
			=> from info in dir.GetFileSystemInfos().Where(verification)
			   let contiainer = ResourceContainer.Create(info)
			   orderby contiainer.Name
			   orderby contiainer.InfoType descending
			   select contiainer;

		// FileSystem event handler
		private void FileSystemEventHandler(object sender, FileSystemEventArgs e) => Dispatcher.Invoke(Reload);

		// ContextMenu Click
		private void OpenFile(object sender, RoutedEventArgs e)
			=> FileOperations.OpenFile(GetSelectedInfos().ToArray());

		private void MoveFile(object sender, RoutedEventArgs e) {
		}

		private void CopyFile(object sender, RoutedEventArgs e) {
		}

		private void DeleteFile(object sender, RoutedEventArgs e) {
		}
	}
}