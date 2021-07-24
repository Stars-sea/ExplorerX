using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ExplorerX.Controls {

	/// <summary>
	/// 弹性 TabItem
	/// </summary>
	[TemplatePart(Name = "PART_CloseButton", Type = typeof(ButtonBase))]
	public class ElasticTabItem : TabItem {

		#region Static Member

		static ElasticTabItem() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ElasticTabItem), new FrameworkPropertyMetadata(typeof(ElasticTabItem)));
		}

		#endregion Static Member

		#region Properties

		protected ButtonBase CloseButton => (ButtonBase) Template.FindName("PART_CloseButton", this);
		protected ElasticTabControl ParentTab => (ElasticTabControl) ItemsControl.ItemsControlFromItemContainer(this);

		#endregion Properties

		#region Event Handlers

		public ElasticTabItem() {
			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs args) {
			CloseButton.Click += (_, _) => Close();
		}

		protected override void OnMouseDown(MouseButtonEventArgs e) {
			base.OnMouseDown(e);
			if (e.ChangedButton == MouseButton.Middle)
				Close();
		}

		#endregion Event Handlers

		public void Close() => ParentTab.RemoveItem(this);
	}
}