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
using System.Diagnostics;
using System.Windows.Input;
using Windows.UI.Xaml;
#else
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
#endif

using CenterCLR.Epoxy.Gluing.Internals;

namespace CenterCLR.Epoxy.Gluing
{
	public sealed class EventGlue : GlueItemBase<EventGlue>
	{
		private EventHookFacade currentFacade_;

		public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
			"Name",
			typeof(string),
			typeof(EventGlue),
			new PropertyMetadata(null, OnChanged));

		public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
			"Command",
			typeof(ICommand),
			typeof(EventGlue),
			new PropertyMetadata(null, OnChanged));

		public EventGlue()
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

		internal void OnInvoke<T>(object sender, T parameter)
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

		protected override void OnSetTarget(DependencyObject oldTarget, DependencyObject newTarget)
		{
			if (currentFacade_ != null)
			{
				currentFacade_.RemoveHandler(this, oldTarget);
				currentFacade_ = null;
			}

			if ((newTarget == null) || string.IsNullOrEmpty(this.Name))
			{
				return;
			}

			var type = newTarget.GetType();

			currentFacade_ = EventHookFacade.TryGetFacade(type, this.Name);
			if (currentFacade_ == null)
			{
				Debug.WriteLine(string.Format("EventGlue: warning: cannot found event: Type={0}, Name={1}", type.FullName, this.Name));
				return;
			}

			currentFacade_.AddHandler(this, newTarget);
		}

		private void OnChanged(DependencyPropertyChangedEventArgs e)
		{
			this.OnSetTarget(base.Target, base.Target);
		}

		private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var eventGlue = (EventGlue)d;
			eventGlue.OnChanged(e);
		}
	}
}
