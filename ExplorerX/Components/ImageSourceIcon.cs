using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;


namespace ExplorerX.Components {
	public sealed class ImageSourceIcon : BitmapIcon {
		private Image? RootImage;

		public ImageSource? ImageSource {
			get { return (ImageSource?)GetValue(ImageSourceProperty); }
			set {
				SetValue(ImageSourceProperty, value);
				if (RootImage is not null)
					RootImage.Source = value;
			}
		}

		public static readonly DependencyProperty ImageSourceProperty =
			DependencyProperty.Register("ImageSource", typeof(ImageSource), 
				typeof(ImageSourceIcon), new PropertyMetadata(null));

		protected override void OnApplyTemplate() {
			// https://github.com/microsoft/microsoft-ui-xaml/blob/main/dev/ImageIcon/ImageIcon.cpp

			if (ImageSource is null ||
				VisualTreeHelper.GetChild(this, 0) is not Grid grid ||
				VisualTreeHelper.GetChild(grid, 0) is not Image image
			) {
				base.OnApplyTemplate();
				return;
			}

			RootImage	 = image;
			image.Source = ImageSource;
		}
	}
}
