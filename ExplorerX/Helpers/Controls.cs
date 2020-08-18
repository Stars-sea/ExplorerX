using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

/// <summary>
/// 存放扩展类
/// </summary>
namespace ExplorerX.Helpers {

	public static class DependencyObjectHelper {
		public static T? FindParent<T>(this DependencyObject @object, uint depth) where T : DependencyObject {
			DependencyObject backup = @object;
			uint currentDepth = 0;
			while (currentDepth++ < depth && !(backup is T))
				backup = VisualTreeHelper.GetParent(backup);
			return backup as T;
		}
	}

	public static class UIElementHelper {

		public static void BeginDoubleAnimation(this IAnimatable control, DependencyProperty dp,
			double from, double to, Duration duration, IEasingFunction? function = null)
			=> control.BeginAnimation(dp, new DoubleAnimation(from, to, duration) { EasingFunction = function });

		public static void BeginDoubleAnimation(this IAnimatable control, DependencyProperty dp,
			double from, double to, TimeSpan time, IEasingFunction? function = null)
			=> control.BeginDoubleAnimation(dp, from, to, new Duration(time), function);

		public static void BeginDoubleAnimation(this IAnimatable control, DependencyProperty dp,
			double from, double to, double sec = 0.3, IEasingFunction? function = null)
			=> control.BeginDoubleAnimation(dp, from, to, TimeSpan.FromSeconds(sec), function);

		public static Task BeginDoubleAnimationAsync(this IAnimatable control, DependencyProperty dp,
			double from, double to, Duration duration, IEasingFunction? function = null) {
			control.BeginDoubleAnimation(dp, from, to, duration, function);
			return Task.Delay(duration.TimeSpan);
		}

		public static Task BeginDoubleAnimationAsync(this IAnimatable control, DependencyProperty dp,
			double from, double to, TimeSpan time, IEasingFunction? function = null)
			=> control.BeginDoubleAnimationAsync(dp, from, to, new Duration(time), function);

		public static Task BeginDoubleAnimationAsync(this IAnimatable control, DependencyProperty dp,
			double from, double to, double sec = 0.3, IEasingFunction? function = null)
			=> control.BeginDoubleAnimationAsync(dp, from, to, TimeSpan.FromSeconds(sec), function);

		public static void BeginDoubleAnimation(this IAnimatable control, DependencyProperty dp,
			double to, Duration duration, IEasingFunction? function = null)
			=> control.BeginAnimation(dp, new DoubleAnimation(to, duration) { EasingFunction = function });

		public static void BeginDoubleAnimation(this IAnimatable control, DependencyProperty dp,
			double to, TimeSpan time, IEasingFunction? function = null)
			=> control.BeginDoubleAnimation(dp, to, new Duration(time), function);

		public static void BeginDoubleAnimation(this IAnimatable control, DependencyProperty dp,
			double to, double sec = 0.3, IEasingFunction? function = null)
			=> control.BeginDoubleAnimation(dp, to, TimeSpan.FromSeconds(sec), function);

		public static Task BeginDoubleAnimationAsync(this IAnimatable control, DependencyProperty dp,
			double to, Duration duration, IEasingFunction? function = null) {
			control.BeginAnimation(dp, new DoubleAnimation(to, duration) { EasingFunction = function });
			return Task.Delay(duration.TimeSpan);
		}

		public static Task BeginDoubleAnimationAsync(this IAnimatable control, DependencyProperty dp,
			double to, TimeSpan time, IEasingFunction? function = null)
			=> control.BeginDoubleAnimationAsync(dp, to, new Duration(time), function);

		public static Task BeginDoubleAnimationAsync(this IAnimatable control, DependencyProperty dp,
			double to, double sec = 0.3, IEasingFunction? function = null)
			=> control.BeginDoubleAnimationAsync(dp, to, TimeSpan.FromSeconds(sec), function);
	}
}