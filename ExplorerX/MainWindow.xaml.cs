using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace ExplorerX {

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		public MainWindow() {
			Task.WaitAll(new[] { InitCongifureAsync() }, 2000);
			InitializeComponent();
		}

		private Task InitCongifureAsync() => Task.Run(InitCongifure);

		private void InitCongifure() {
			Trace.Listeners.Add(GetTextWriterTraceListener());
			Trace.AutoFlush = true;
			Trace.IndentSize = 4;
			Trace.UseGlobalLock = true;
		}

		private static TextWriterTraceListener GetTextWriterTraceListener() {
			string fileName = $"Logs\\{DateTime.Now:g}";
			int triedTimes  = 0;
			while (true) {
				string fullName = $"{fileName} ({triedTimes}).log";
				if (!File.Exists(fullName))
					return new TextWriterTraceListener(fullName);
			}
			throw new FileNotFoundException();
		}
	}
}