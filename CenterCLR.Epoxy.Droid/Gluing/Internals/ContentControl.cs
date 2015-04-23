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

#if XAMARIN
using System.Windows;
using Xamarin.Forms;
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using FrameworkElement = Xamarin.Forms.Element;

using CenterCLR.Epoxy.Internals;

namespace CenterCLR.Epoxy.Gluing.Internals
{
	internal class ContentControl : ContentView
	{
		public static readonly DependencyProperty ContentProperty =
			Utilities.Register("Content", typeof(ContentControl), (object)null, OnContentChanged);

		public static readonly DependencyProperty ContentTemplateProperty =
			Utilities.Register("ContentTemplate", typeof(ContentControl), (DataTemplate)null, OnContentTemplateChanged);

		public ContentControl()
		{
		}

		public new object Content
		{
			get
			{
				return base.GetValue(ContentProperty);
			}
			set
			{
				base.SetValue(ContentProperty, value);
			}
		}

		public DataTemplate ContentTemplate
		{
			get
			{
				return (DataTemplate)base.GetValue(ContentTemplateProperty);
			}
			set
			{
				base.SetValue(ContentTemplateProperty, value);
			}
		}

		private void OnContentChanged(DependencyPropertyChangedEventArgs e)
		{
			var newContent = e.NewValue;
			if (newContent != null)
			{
				var template = this.ContentTemplate;
				if (template != null)
				{
					var view = (View)template.CreateContent();
					view.SetValue(BindingContextProperty, newContent);
					base.Content = view;
				}
				else
				{
					var view = new Label() { Text = newContent.ToString() };
					base.Content = view;
				}
			}
			else
			{
				base.Content = null;
			}
		}

		private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = (ContentControl)d;
			control.OnContentChanged(e);
		}

		private void OnContentTemplateChanged(DependencyPropertyChangedEventArgs e)
		{
			var newTemplate = (DataTemplate)e.NewValue;
			var content = this.Content;
			if (content != null)
			{
				if (newTemplate != null)
				{
					var view = (View)newTemplate.CreateContent();
					view.SetValue(BindingContextProperty, content);
					base.Content = view;
				}
				else
				{
					var view = new Label() { Text = content.ToString() };
					base.Content = view;
				}
			}
			else
			{
				base.Content = null;
			}
		}

		private static void OnContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = (ContentControl)d;
			control.OnContentTemplateChanged(e);
		}
	}
}
#endif
