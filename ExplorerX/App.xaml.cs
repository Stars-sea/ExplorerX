﻿using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WindowId = Microsoft.UI.WindowId;

using System;
using System.Threading.Tasks;

using Windows.UI;

using WinRT.Interop;


namespace ExplorerX {
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public partial class App : Application {
		#region Public Props
		public static MainWindow? Window    { get; private set; }
		public static AppWindow?  AppWindow { get; private set; }
		#endregion

		public event Action AppInitiating;

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App() {
			InitializeComponent();
			AppInitiating += Events.Handlers.AppInitiatingHandler.InitAsync;
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="args">Details about the launch request and process.</param>
		protected override async void OnLaunched(LaunchActivatedEventArgs args) {
			Window = new MainWindow();

			WindowId id = Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(Window));
			AppWindow   = AppWindow.GetFromWindowId(id);

			AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
			
			await Task.Run(AppInitiating);
			Window.Navigate(typeof(Pages.CommonPage));

			Window.Activate();
		}

		public static void InitAppButtons(Color color) {
			AppWindowTitleBar? titleBar = AppWindow?.TitleBar;
			if (titleBar is not null)
				titleBar.ButtonBackgroundColor = color;
		}
	}
}
