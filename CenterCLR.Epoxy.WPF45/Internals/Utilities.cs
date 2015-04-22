////////////////////////////////////////////////////////////////////////////
// CenterCLR.Epoxy - A simplism MVVM assister library.
//   Copyright 2015 (c) Kouji Matsui (@kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
////////////////////////////////////////////////////////////////////////////

#if NETFX_CORE
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
#endif
#if NET35 || NET40 || NET45 || SILVERLIGHT5 || WINDOWS_PHONE71 || WINDOWS_PHONE80
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Windows;
#endif
#if MONODROID
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Xamarin.Forms;
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using FrameworkElement = Xamarin.Forms.Element;
#endif

namespace CenterCLR.Epoxy.Internals
{
	internal static class Utilities
	{
		public static Type[] GetDelegateParameterTypes<T>()
			where T : class
		{
			Debug.Assert(typeof(Delegate).IsAssignableFromTrampoline(typeof(T)));

			return DelegateParameterExtractor<T>.DelegateTypes;
		}

		public static ValidationContext CreateValidationContext(object instance, string propertyName)
		{
#if NETFX_CORE
			return new ValidationContext(instance)
			{
				MemberName = propertyName
			};
#else
			return new ValidationContext(instance, null, null)
			{
				MemberName = propertyName
			};
#endif
		}

		public static DependencyObject GetParent(this DependencyObject d)
		{
#if WIN32
			return LogicalTreeHelper.GetParent(d);
#else
			var fe = d as FrameworkElement;
			if (fe == null)
			{
				return null;
			}

			return fe.Parent;
#endif
		}

		public static DependencyProperty Register<T>(
			string propertyName,
			Type ownerType,
			T defaultValue,
			Action<DependencyObject, DependencyPropertyChangedEventArgs> changedHandler)
		{
#if WIN32 || SILVERLIGHT || NETFX_CORE
			return DependencyProperty.Register(
				propertyName,
				typeof(T),
				ownerType,
				new PropertyMetadata(defaultValue, (s, e) => changedHandler(s, e)));
#endif
#if XAMARIN
			return DependencyProperty.Create(
				propertyName,
				typeof(T),
				ownerType,
				defaultValue,
				BindingMode.OneWay,
				null,
				(b, oldValue, newValue) => changedHandler(b, new DependencyPropertyChangedEventArgs(oldValue, newValue)));
#endif
		}

		public static DependencyProperty RegisterAttached<T>(
			string propertyName,
			Type ownerType,
			T defaultValue,
			Action<DependencyObject, DependencyPropertyChangedEventArgs> changedHandler)
		{
#if WIN32 || SILVERLIGHT || NETFX_CORE
			return DependencyProperty.RegisterAttached(
				propertyName,
				typeof(T),
				ownerType,
				new PropertyMetadata(defaultValue, (s, e) => changedHandler(s, e)));
#endif
#if XAMARIN
			return DependencyProperty.CreateAttached(
				propertyName,
				typeof(T),
				ownerType,
				defaultValue,
				BindingMode.OneWay,
				null,
				(b, oldValue, newValue) => changedHandler(b, new DependencyPropertyChangedEventArgs(oldValue, newValue)));
#endif
		}

		private static class DelegateParameterExtractor<T>
			where T : class
		{
			public static readonly Type[] DelegateTypes;

			static DelegateParameterExtractor()
			{
				Debug.Assert(typeof(Delegate).IsAssignableFromTrampoline(typeof(T)));

				var method = typeof(T).GetMethodTrampoline("Invoke");
				if (method == null)
				{
					DelegateTypes = ReflectionTrampoline.EmptyTypes;
				}
				else
				{
					var parameters = method.GetParameters();
					DelegateTypes = parameters.Select(parameter => parameter.ParameterType).ToArray();
				}
			}
		}
	}
}
