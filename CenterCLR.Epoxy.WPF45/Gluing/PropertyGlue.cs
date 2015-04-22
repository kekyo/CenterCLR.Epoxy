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
using System.ComponentModel;
using Windows.UI.Xaml;
#endif
#if WIN32 || SILVERLIGHT
using System.ComponentModel;
using System.Windows;
#endif
#if XAMARIN
using System.ComponentModel;
using System.Windows;
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using FrameworkElement = Xamarin.Forms.Element;
#endif

using CenterCLR.Epoxy.Internals;

namespace CenterCLR.Epoxy.Gluing
{
	public sealed class PropertyGlue : MemberTargettableGlueBase<PropertyGlue>
	{
		public static readonly DependencyProperty PropertyNameProperty = Utilities.Register<string>(
			"PropertyName",
			typeof(PropertyGlue),
			null,
			OnPropertyNameChanged);

		public static readonly DependencyProperty AccessorProperty = Utilities.Register<PropertyAccessor>(
			"Accessor",
			typeof(PropertyGlue),
			null,
			OnAccessorChanged);

		public PropertyGlue()
		{
		}

#if WIN32
		[Bindable(true)]
#endif
		public string PropertyName
		{
			get
			{
				return (string)base.GetValue(PropertyNameProperty);
			}
			set
			{
				base.SetValue(PropertyNameProperty, value);
			}
		}

#if WIN32
		[Bindable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
		public PropertyAccessor Accessor
		{
			get
			{
				return (PropertyAccessor)base.GetValue(AccessorProperty);
			}
			set
			{
				base.SetValue(AccessorProperty, value);
			}
		}

		protected override void OnSetElementContext(FrameworkElement oldElementContext, FrameworkElement newElementContext)
		{
			var accessor = this.Accessor;
			if (accessor != null)
			{
				accessor.SetTarget(newElementContext, this.PropertyName);
			}
		}

		protected override void OnTargetNameChanged(string oldName, string newName)
		{
			var elementContext = base.GetElementContext();
			this.OnSetElementContext(elementContext, elementContext);
		}

		private void OnPropertyNameChanged(DependencyPropertyChangedEventArgs e)
		{
			var elementContext = base.GetElementContext();
			this.OnSetElementContext(elementContext, elementContext);
		}

		private void OnAccessorChanged(DependencyPropertyChangedEventArgs e)
		{
			var elementContext = base.GetElementContext();
			this.OnSetElementContext(elementContext, elementContext);
		}

		private static void OnPropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var propertyGlue = (PropertyGlue)d;
			propertyGlue.OnPropertyNameChanged(e);
		}

		private static void OnAccessorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var propertyGlue = (PropertyGlue)d;
			propertyGlue.OnAccessorChanged(e);
		}
	}
}
