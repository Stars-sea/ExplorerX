using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

using System;

namespace ExplorerX {
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : Window, INavigate {
		public Page? CurrentPage => ContentFrame.Content as Page;

		public event Action<Page?> Navigated;

		public MainWindow() {
			InitializeComponent();
			Navigated += OnNavigated;
		}

		private void OnNavigated(Page? newPage) {
			if (CurrentPage?.Background is SolidColorBrush brush)
				App.InitAppButtons(brush.Color);
		}

		public bool Navigate(Type sourcePageType, NavigationTransitionInfo transition) {
			if (ContentFrame.Navigate(sourcePageType, null, transition)) {
				Navigated(CurrentPage);
				return true;
			}
			return false;
		}

		public bool Navigate(Type sourcePageType) 
			=> Navigate(sourcePageType, new DrillInNavigationTransitionInfo());
	}
}
