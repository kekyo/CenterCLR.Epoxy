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

#if WINFX_CORE
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

using CenterCLR.Epoxy.Gluing.Internals;

namespace CenterCLR.Epoxy.Gluing
{
	public static class Epoxy
	{
		public static readonly DependencyProperty GluesProperty =
			DependencyProperty.RegisterAttached(
				"ShadowGlues",
				typeof(GlueItemCollection),
				typeof(Epoxy),
				new PropertyMetadata(null, OnChanged));

		public static GlueItemCollection GetGlues(DependencyObject d)
		{
			var value = (GlueItemCollection)d.GetValue(GluesProperty);
			if (value == null)
			{
				value = new GlueItemCollection();
				d.SetValue(GluesProperty, value);
			}

			return value;
		}

		private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var oldValue = e.OldValue as IInternalGlueItem;
			if (oldValue != null)
			{
				oldValue.SetTarget(null);
			}

			var newValue = e.NewValue as IInternalGlueItem;
			if (newValue != null)
			{
				newValue.SetTarget(d);
			}
		}
	}
}
