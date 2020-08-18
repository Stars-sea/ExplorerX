using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ExplorerX.Controls {

	/// <summary>
	/// 表示目录的子节点
	/// </summary>
	/// <remarks>
	/// Tag 属性是它当前表示的目录 (DirectoryInfo 类型)
	/// </remarks>
	public partial class PathsListItem : ButtonBase {

		public PathsListItem() => InitializeComponent();

		public PathsListItem(DirectoryInfo dir) : this() {
			if (!dir.Exists)
				throw new DirectoryNotFoundException($"Not found directory {dir.FullName}");
			Content = dir.Name.Trim('\\');
			Tag = dir;
		}

		public PathsListItem(DirectoryInfo dir, RoutedEventHandler onclick) : this(dir) => Click += onclick;

		private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
			=> RaiseEvent(new RoutedEventArgs(ClickEvent, this));
	}
}