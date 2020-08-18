using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ExplorerX.Controls {

	public interface ICustomAnimatable {

		public static readonly DependencyProperty DefaultEasingFunctionProperty =
			DependencyProperty.Register("DefaultEasingFunction", typeof(IEasingFunction), typeof(ContentControl),
				new PropertyMetadata(new CircleEase { EasingMode = EasingMode.EaseOut }));
	}
}