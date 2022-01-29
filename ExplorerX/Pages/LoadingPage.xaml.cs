using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


namespace ExplorerX.Pages {
	/// <summary>
	/// <para>
	/// Loading Page, like Splash Screen in UWP
	/// </para><para>
	/// 加载页面, 类似于 UWP 的 Splash Screen
	/// </para>
	/// </summary>
	public sealed partial class LoadingPage : Page {

		#region Dependency Property

		public Visibility LogoVisibility {
			get { return (Visibility)GetValue(LogoVisibilityProperty); }
			set { SetValue(LogoVisibilityProperty, value); }
		}

		public static readonly DependencyProperty LogoVisibilityProperty =
			DependencyProperty.Register("LogoVisibility", typeof(Visibility), 
				typeof(LoadingPage), new PropertyMetadata(Visibility.Visible));


		public bool IsProgressing {
			get { return (bool)GetValue(IsProgressingProperty); }
			set { SetValue(IsProgressingProperty, value); }
		}

		public static readonly DependencyProperty IsProgressingProperty =
			DependencyProperty.Register("IsProgressing", typeof(bool), 
				typeof(LoadingPage), new PropertyMetadata(true));

		#endregion

		public LoadingPage() {
			InitializeComponent();
		}
	}
}
