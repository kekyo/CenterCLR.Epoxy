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
using System.Diagnostics;
using System.Windows.Input;
using Windows.UI.Xaml;
#endif
#if WIN32 || SILVERLIGHT
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
#endif
#if XAMARIN
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using FrameworkElement = Xamarin.Forms.Element;
#endif

using CenterCLR.Epoxy.Gluing.Internals;
using CenterCLR.Epoxy.Internals;

namespace CenterCLR.Epoxy.Gluing
{
	public sealed class EventGlue : GlueBase<EventGlue>
	{
		private EventHookFacade currentFacade_;

		public static readonly DependencyProperty NameProperty = Utilities.Register<string>(
			"Name",
			typeof(EventGlue),
			null,
			OnChanged);

		public static readonly DependencyProperty CommandProperty = Utilities.Register<ICommand>(
			"Command",
			typeof(EventGlue),
			null,
			OnChanged);

		public EventGlue()
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
		public ICommand Command
		{
			get
			{
				return (ICommand)base.GetValue(CommandProperty);
			}
			set
			{
				base.SetValue(CommandProperty, value);
			}
		}

#if NETFX_CORE
		public void OnInvoke<T>(object sender, T parameter)
#else
		internal void OnInvoke<T>(object sender, T parameter)
#endif
		{
			var command = this.Command;
			if (command != null)
			{
				if (command.CanExecute(parameter) == true)
				{
					command.Execute(parameter);
				}
			}
		}

		protected override void OnSetElementContext(FrameworkElement oldElementContext, FrameworkElement newElementContext)
		{
			if (currentFacade_ != null)
			{
				currentFacade_.RemoveHandler(this, oldElementContext);
				currentFacade_ = null;
			}

			if ((newElementContext == null) || string.IsNullOrEmpty(this.Name))
			{
				return;
			}

			var type = newElementContext.GetType();

			currentFacade_ = EventHookFacade.TryGetFacade(type, this.Name);
			if (currentFacade_ == null)
			{
				Debug.WriteLine(string.Format("Epoxy.EventGlue: warning: cannot found event: Type={0}, Name={1}", type.FullName, this.Name));
				return;
			}

			currentFacade_.AddHandler(this, newElementContext);
		}

		private void OnChanged(DependencyPropertyChangedEventArgs e)
		{
			var elementContext = base.GetElementContext();
			this.OnSetElementContext(elementContext, elementContext);
		}

		private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var eventGlue = (EventGlue)d;
			eventGlue.OnChanged(e);
		}
	}
}
