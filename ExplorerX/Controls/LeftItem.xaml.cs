using ExplorerX.Helpers;
using ExplorerX.Wrapper;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ExplorerX.Controls {
	/// <summary>
	/// LeftItem.xaml 的交互逻辑
	/// </summary>
	public partial class LeftItem : ListBoxItem {

		public Image Icon => (Image)Template.FindName("Icon", this);
		public ImageSource IconSource {
			get => Icon.Source;
			set => Icon.Source = value;
		}

		public TextBlock Detail => (TextBlock)Template.FindName("Detail", this);
		public string Text {
			get => Detail.Text;
			set => Detail.Text = value;
		}

		private	DirectoryInfo? delegateDir;
		public	DirectoryInfo? DelegateDir {
			get => delegateDir;
			set {
				if (value == null) throw new NullReferenceException();
				DirectoryInfo? old = DelegateDir;
				delegateDir = value;
				OnDelegateDirChanged(old, DelegateDir);
			}
		}

		private EllipseGeometry EffectEllipse => (EllipseGeometry)Template.FindName("EffectEllipse", this);


		public LeftItem() => InitializeComponent();

		public LeftItem(DirectoryInfo dir) : this() {
			Loaded += (s, e) => DelegateDir = dir;
		}

		public LeftItem(DriveInfo drive) : this(drive.RootDirectory) {
			Loaded += (s, e) => Text = $"{drive.VolumeLabel} ({drive.Name[..^1]})";
		}

		protected virtual void OnDelegateDirChanged(DirectoryInfo? oldValue, DirectoryInfo? newValue) {
			IconSource = ShellInfoContainer.Create(newValue?.FullName ?? throw new NullReferenceException());
			Text = newValue.Name;
		}

		protected async override void OnSelected(RoutedEventArgs e) {
			base.OnSelected(e);
			if (EffectEllipse is null) return;

			EffectEllipse.Center = Mouse.GetPosition(this);
			await EffectEllipse.BeginDoubleAnimationAsync(EllipseGeometry.RadiusXProperty, from: 0, ActualWidth);
			EffectEllipse.BeginDoubleAnimation(EllipseGeometry.RadiusXProperty, 0, sec: 0);
			if (IsSelected) Background = (Brush)FindResource("SelectedBackground");
		}

		protected override void OnUnselected(RoutedEventArgs e) {
			base.OnUnselected(e);
			Background = Brushes.Transparent;
		}
	}
}
