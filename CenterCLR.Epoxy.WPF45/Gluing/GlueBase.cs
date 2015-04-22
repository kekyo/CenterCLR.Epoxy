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
#if WIN32 || SILVERLIGHT5 || WINDOWS_PHONE71 || WINDOWS_PHONE80
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
#endif
#if MONODROID
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using FrameworkElement = Xamarin.Forms.Element;
#endif

namespace CenterCLR.Epoxy.Gluing
{
	public abstract class GlueBase :
#if WIN32
		Animatable
#else
		DependencyObject
#endif
	{
		private FrameworkElement elementContext_;

		internal GlueBase()
		{
		}

#if WIN32
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
		protected virtual FrameworkElement GetElementContext()
		{
			return elementContext_;
		}

		internal void SetElementContext(FrameworkElement target)
		{
			if (object.ReferenceEquals(target, elementContext_) == true)
			{
				return;
			}

			var oldElementContext = this.GetElementContext();

			elementContext_ = target;

			var newElementContext = this.GetElementContext();

			this.OnSetElementContext(oldElementContext, newElementContext);
		}

		protected abstract void OnSetElementContext(FrameworkElement oldElementContext, FrameworkElement newElementContext);
	}

	public abstract class GlueBase<T> : GlueBase
		where T : GlueBase, new()
	{
		protected GlueBase()
		{
		}

#if WIN32
		protected override sealed Freezable CreateInstanceCore()
		{
			return new T();
		}
#endif
	}
}
