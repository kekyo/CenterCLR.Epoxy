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
#endif
#if WIN32 || SILVERLIGHT5 || WINDOWS_PHONE71 || WINDOWS_PHONE80
using System.Windows;
#endif
#if XAMARIN
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using FrameworkElement = Xamarin.Forms.Element;
#endif

namespace CenterCLR.Epoxy.Gluing
{
	public sealed class GlueCollection :
#if WIN32
		DependencyObjectList<GlueBase, GlueCollection>
#else
		DependencyObjectList<GlueBase>
#endif
	{
		private FrameworkElement elementContext_;

		protected override void OnAdded(GlueBase newItem, int index)
		{
			newItem.SetElementContext(elementContext_);
			base.OnAdded(newItem, index);
		}

		protected override void OnRemoved(GlueBase oldItem, int index)
		{
			oldItem.SetElementContext(null);
			base.OnRemoved(oldItem, index);
		}

		protected override void OnReplaced(GlueBase newItem, GlueBase oldItem, int index)
		{
			oldItem.SetElementContext(null);
			newItem.SetElementContext(elementContext_);
			base.OnReplaced(newItem, oldItem, index);
		}

		protected override void OnClearing()
		{
			base.OnClearing();

			foreach (var item in this)
			{
				item.SetElementContext(null);
			}
		}

		internal void SetElementContext(FrameworkElement elementContext)
		{
			if (object.ReferenceEquals(elementContext, elementContext_) == true)
			{
				return;
			}

			elementContext_ = elementContext;

			foreach (var item in this)
			{
				item.SetElementContext(elementContext_);
			}
		}
	}
}
