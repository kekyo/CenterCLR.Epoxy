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
#else
using System.ComponentModel;
using System.Windows;
#endif

namespace CenterCLR.Epoxy.Gluing
{
	public sealed class MethodGlue : GlueBase<MethodGlue>
	{
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
			"Name",
			typeof(string),
			typeof(MethodGlue),
			new PropertyMetadata(null, OnChanged));

		public static readonly DependencyProperty InvokerProperty = DependencyProperty.Register(
			"Invoker",
			typeof(MethodInvoker),
			typeof(MethodGlue),
			new PropertyMetadata(null, OnChanged));

		public MethodGlue()
		{
		}

#if NET35 || NET40 || NET45
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

#if NET35 || NET40 || NET45
		[Bindable(true)]
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

		protected override void OnSetTarget(DependencyObject oldTarget, DependencyObject newTarget)
		{
			var invoker = this.Invoker;
			if (invoker != null)
			{
				invoker.SetTarget(newTarget, this.Name);
			}
		}

		private void OnChanged(DependencyPropertyChangedEventArgs e)
		{
			this.OnSetTarget(base.Target, base.Target);
		}

		private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var methodGlue = (MethodGlue)d;
			methodGlue.OnChanged(e);
		}
	}
}
