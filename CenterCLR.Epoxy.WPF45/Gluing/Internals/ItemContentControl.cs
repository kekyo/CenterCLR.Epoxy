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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endif
#if WIN32 || SILVERLIGHT
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
#endif
#if XAMARIN
using System.Windows;
using Xamarin.Forms;
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using FrameworkElement = Xamarin.Forms.Element;
#endif

namespace CenterCLR.Epoxy.Gluing.Internals
{
	internal sealed class ItemContentControl :
		ContentControl
	{
		public ItemContentControl()
		{
		}

		private void SetBinding(DependencyProperty targetProperty, DependencyObject source, string sourceProperty)
		{
			var binding = new Binding
			{
				Source = source,
#if WIN32 || SILVERLIGHT || NETFX_CORE
				Path = new PropertyPath(sourceProperty)
#endif
#if XAMARIN
				Path = sourceProperty
#endif
			};

			this.SetBinding(targetProperty, binding);
		}

		public void SetContentValue(ItemsGlue parent, object value)
		{
#if WIN32
			this.SetBinding(ContentStringFormatProperty, parent, "ItemStringFormat");
#endif
#if WIN32 || NETFX_CORE
			this.SetBinding(ContentTemplateSelectorProperty, parent, "ItemTemplateSelector");
#endif
			this.SetBinding(ContentTemplateProperty, parent, "ItemTemplate");
			this.SetValue(ContentProperty, value);
		}

		public void SetContentValue(object value)
		{
			this.SetValue(ContentProperty, value);
		}

		public void ResetContentValue()
		{
			this.ClearValue(ContentProperty);
			this.ClearValue(ContentTemplateProperty);
#if WIN32 || NETFX_CORE
			this.ClearValue(ContentTemplateSelectorProperty);
#endif
#if WIN32
			this.ClearValue(ContentStringFormatProperty);
#endif
		}
	}
}
