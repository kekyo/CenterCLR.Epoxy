﻿////////////////////////////////////////////////////////////////////////////
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

#if WINFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#else
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
#endif

namespace CenterCLR.Epoxy.Gluing.Internals
{
	internal sealed class ItemContentControl : ContentControl
	{
		public static readonly DependencyProperty ItemIndexProperty = DependencyProperty.Register(
			"ItemIndex",
			typeof(int),
			typeof(ItemContentControl),
			new PropertyMetadata(-1));

		public ItemContentControl()
		{
		}

#if NET35 || NET40 || NET45
		[Bindable(true)]
#endif
		public int ItemIndex
		{
			get
			{
				return (int)base.GetValue(ItemIndexProperty);
			}
			set
			{
				base.SetValue(ItemIndexProperty, value);
			}
		}

		private void SetBinding(DependencyProperty targetProperty, DependencyObject source, string sourceProperty)
		{
			var binding = new Binding
			{
				Source = source,
				Path = new PropertyPath(sourceProperty)
			};

			this.SetBinding(targetProperty, binding);
		}

		public void SetItem(ItemsGlue parent, object value)
		{
#if NET35 || NET40 || NET45
			this.SetBinding(ContentStringFormatProperty, parent, "ItemStringFormat");
#endif
			this.SetBinding(ContentTemplateProperty, parent, "ItemTemplate");
#if NET35 || NET40 || NET45 || WINFX_CORE
			this.SetBinding(ContentTemplateSelectorProperty, parent, "ItemTemplateSelector");
#endif
			this.SetValue(ContentProperty, value);
		}

		public void ResetItem()
		{
			this.SetValue(ContentProperty, DependencyProperty.UnsetValue);

#if NET35 || NET40 || NET45
			this.SetValue(ContentStringFormatProperty, DependencyProperty.UnsetValue);
#endif
			this.SetValue(ContentTemplateProperty, DependencyProperty.UnsetValue);
#if NET35 || NET40 || NET45 || WINFX_CORE
			this.SetValue(ContentTemplateSelectorProperty, DependencyProperty.UnsetValue);
#endif
		}
	}
}