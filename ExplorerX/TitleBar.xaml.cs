using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


namespace ExplorerX {
	public sealed partial class TitleBar : UserControl {

		public string Title {
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register("Title", typeof(string), typeof(TitleBar), new PropertyMetadata("ExplorerX"));

		public TitleBar() {
			InitializeComponent();
		}
	}
}
