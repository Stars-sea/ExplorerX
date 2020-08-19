using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ExplorerX.Controls {

	/// <summary>
	/// PathsList.xaml 的交互逻辑
	/// </summary>
	public partial class PathsList : Selector {

		public static readonly DependencyProperty DirectoryPathProperty =
			DependencyProperty.Register("DirctoryPathProperty", typeof(string), typeof(PathsList));

		public string DirectoryPath {
			get => (string)GetValue(DirectoryPathProperty);
			set {
				string backup = DirectoryPath;
				SetValue(DirectoryPathProperty, value);
				OnPathChanged(backup, DirectoryPath);
			}
		}

		public static readonly RoutedEvent ItemClickEvent =
			EventManager.RegisterRoutedEvent("ItemClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PathsList));

		/// <summary>
		/// 当某个子项触发 Click 事件时被触发
		/// </summary>
		public event RoutedEventHandler ItemClick {
			add => AddHandler(ItemClickEvent, value);
			remove => RemoveHandler(ItemClickEvent, value);
		}

		public PathsList() => InitializeComponent();

		public PathsList(string dirPath) : this() {
			if (!Directory.Exists(dirPath))
				throw new DirectoryNotFoundException($"Not found directory {dirPath}");

			DirectoryPath = dirPath;
		}

		public PathsList(string dirPath, RoutedEventHandler handler) : this(dirPath)
			=> ItemClick += handler;

		public virtual void RefreshItems()
			=> ItemsSource = GetParentDirs(new DirectoryInfo(DirectoryPath))
				.Select(i => new PathsListItem(i, (s, e) => {
					RaiseEvent(new RoutedEventArgs(ItemClickEvent, i));
					SelectedItem = i;
				}));

		protected virtual void OnPathChanged(string oldPath, string newPath) => RefreshItems();

		private static IEnumerable<DirectoryInfo> GetParentDirs(DirectoryInfo dir) {
			DirectoryInfo info = dir;
			List<DirectoryInfo> paths = new List<DirectoryInfo>(new[] { dir });
			while (info.Parent != null) paths.Add(info = info.Parent);

			return paths.Where(i => i != null).Reverse();
		}
	}
}