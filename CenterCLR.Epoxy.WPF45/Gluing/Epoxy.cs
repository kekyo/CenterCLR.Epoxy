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
#if WIN32 || SILVERLIGHT
using System.Windows;
#endif
#if XAMARIN
using System.Windows;
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using FrameworkElement = Xamarin.Forms.Element;
#endif

using CenterCLR.Epoxy.Internals;

namespace CenterCLR.Epoxy.Gluing
{
	public static class Epoxy
	{
		public static readonly DependencyProperty GluesProperty =
			Utilities.RegisterAttached<GlueCollection>(
				"ShadowGlues",
				typeof(Epoxy),
				null,
				OnGluesChanged);

		public static GlueCollection GetGlues(FrameworkElement d)
		{
			var value = (GlueCollection)d.GetValue(GluesProperty);
			if (value == null)
			{
				value = new GlueCollection();
				d.SetValue(GluesProperty, value);
			}

			return value;
		}

		private static void OnGluesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var oldValue = (GlueCollection)e.OldValue;
			if (oldValue != null)
			{
				oldValue.SetElementContext(null);
			}

			var newValue = (GlueCollection)e.NewValue;
			if (newValue != null)
			{
				newValue.SetElementContext((FrameworkElement)d);
			}
		}
	}
}
