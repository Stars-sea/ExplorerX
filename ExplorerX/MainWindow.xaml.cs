using ExplorerX.Controls;
using ExplorerX.Controls.Events;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ExplorerX {

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		public MainWindow() {
			InitializeComponent();
		}

		private void OnItemAdded(object sender, ChangedItemEventArgs<ElasticTabItem> args) {
			args.Item.Content ??= new Frame {
				Source = new Uri("/Pages/FilesViewer.xaml", UriKind.Relative)
			};
		}
	}
}