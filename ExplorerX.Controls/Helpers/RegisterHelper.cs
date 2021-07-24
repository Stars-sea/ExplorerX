using System;
using System.Reflection;
using System.Windows;

namespace ExplorerX.Controls.Helpers {

	internal sealed class RegisterHelper {
		public Type OwnerType { get; init; }

		public RegisterHelper(Type owner) {
			OwnerType = owner;
		}

		#region Property Register

		private PropertyInfo GetProperty(string propertyName)
			=> OwnerType.GetProperty(propertyName) ?? throw new NullReferenceException();

		public DependencyProperty Register(string propertyName)
			=> Register(GetProperty(propertyName));

		public DependencyProperty Register(string propertyName, PropertyMetadata metadata)
			=> Register(GetProperty(propertyName), metadata);

		public DependencyProperty Register(string propertyName, PropertyMetadata metadata, ValidateValueCallback validateCallback)
			=> Register(GetProperty(propertyName), metadata, validateCallback);

		public DependencyProperty Register(PropertyInfo info)
			=> DependencyProperty.Register(info.Name, info.PropertyType, OwnerType);

		public DependencyProperty Register(PropertyInfo info, PropertyMetadata metadata)
			=> DependencyProperty.Register(info.Name, info.PropertyType, OwnerType, metadata);

		public DependencyProperty Register(PropertyInfo info, PropertyMetadata metadata, ValidateValueCallback validateCallback)
			=> DependencyProperty.Register(info.Name, info.PropertyType, OwnerType, metadata, validateCallback);

		public DependencyPropertyKey RegisterReadOnly(string propertyName, PropertyMetadata metadata)
			=> RegisterReadOnly(GetProperty(propertyName), metadata);

		public DependencyPropertyKey RegisterReadOnly(PropertyInfo info, PropertyMetadata metadata)
			=> DependencyProperty.RegisterReadOnly(info.Name, info.PropertyType, OwnerType, metadata);

		#endregion Property Register

		#region Event Register

		public RoutedEvent RegisterRoutedEvent(string eventName, RoutingStrategy strategy)
			=> RegisterRoutedEvent(OwnerType.GetEvent(eventName) ?? throw new NullReferenceException(), strategy);

		public RoutedEvent RegisterRoutedEvent(EventInfo info, RoutingStrategy strategy)
			=> EventManager.RegisterRoutedEvent(info.Name, strategy, info.EventHandlerType, OwnerType);

		#endregion Event Register
	}
}