using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace ExplorerX {

	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		public static IntPtr InstanceHandle => Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]);
	}
}