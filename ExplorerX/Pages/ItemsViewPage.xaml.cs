using ExplorerX.Data;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using System;
using System.IO;


namespace ExplorerX.Pages {
	/// <summary>
	/// <para>File browsing page</para>
	/// <para>文件浏览页</para>
	/// </summary>
	public sealed partial class ItemsViewPage : Page, IModifiable {
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

		public void Modify(object? param) {
			string path = (string?)param ?? throw new NullReferenceException();
			if (Directory.Exists(path))
				ViewPath = (PathContainer)path;
			else if (RegistryManagers.VariablePool.TryGetValue(path, out object? real))
				Modify(real);
			else throw new DirectoryNotFoundException($"Can't find dir \"{path}\"");
		}
	}
}
