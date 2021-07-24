using ExplorerX.Controls.Helpers;
using SourceChord.FluentWPF;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

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
		#endregion
		
		#region Properties
		protected ButtonBase CloseButton => (ButtonBase) Template.FindName("PART_CloseButton", this);
		protected ElasticTabControl ParentTab => (ElasticTabControl) ItemsControl.ItemsControlFromItemContainer(this);
		#endregion
		
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
		#endregion

		public void Close() => ParentTab.RemoveItem(this);
	}
}
