using ExplorerX.Data;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


namespace ExplorerX.Pages {
	/// <summary>
	/// <para>File browsing page</para>
	/// <para>文件浏览页</para>
	/// </summary>
	public sealed partial class ItemsViewPage : Page {
		#region Dependency Props

		public PathContainer ViewPath {
			get { return (PathContainer)GetValue(ViewPathProperty); }
			set { SetValue(ViewPathProperty, value); }
		}

		public static readonly DependencyProperty ViewPathProperty =
			DependencyProperty.Register("ViewPath", typeof(PathContainer),
				typeof(ItemsViewPage), new PropertyMetadata((PathContainer)@"C:\"));

		#endregion

		public ItemsViewPage() {
			InitializeComponent();
		}
	}
}
