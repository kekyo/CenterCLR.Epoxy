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
using System.ComponentModel;
using System.Windows;
#endif
#if MONODROID
using System.Windows;
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using FrameworkElement = Xamarin.Forms.Element;
#endif

using CenterCLR.Epoxy.Internals;

namespace CenterCLR.Epoxy.Gluing
{
	public sealed class MethodGlue : GlueBase<MethodGlue>
	{
		public static readonly DependencyProperty NameProperty = Utilities.Register<string>(
			"Name",
			typeof(MethodGlue),
			null,
			OnChanged);

		public static readonly DependencyProperty InvokerProperty = Utilities.Register<MethodInvoker>(
			"Invoker",
			typeof(MethodGlue),
			null,
			OnChanged);

		public MethodGlue()
		{
		}

#if WIN32
		[Bindable(true)]
#endif
		public string Name
		{
			get
			{
				return (string)base.GetValue(NameProperty);
			}
			set
			{
				base.SetValue(NameProperty, value);
			}
		}

#if WIN32
		[Bindable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
		public MethodInvoker Invoker
		{
			get
			{
				return (MethodInvoker)base.GetValue(InvokerProperty);
			}
			set
			{
				base.SetValue(InvokerProperty, value);
			}
		}

		protected override void OnSetElementContext(FrameworkElement oldElementContext, FrameworkElement newElementContext)
		{
			var invoker = this.Invoker;
			if (invoker != null)
			{
				invoker.SetTarget(newElementContext, this.Name);
			}
		}

		private void OnChanged(DependencyPropertyChangedEventArgs e)
		{
			var elementContext = base.GetElementContext();
			this.OnSetElementContext(elementContext, elementContext);
		}

		private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var methodGlue = (MethodGlue)d;
			methodGlue.OnChanged(e);
		}
	}
}
