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

using System;
using System.Threading.Tasks;
using System.Windows.Input;

using CenterCLR.Epoxy.UnitTesting;

namespace CenterCLR.Epoxy
{
	public sealed class CommandReceiver : ICommandReceiver
	{
		private bool canExecute_ = true;
		private EventHandler canExecuteChanged_;
		private readonly Func<Task> asyncAction_;

		internal CommandReceiver(Func<Task> asyncAction)
		{
			asyncAction_ = asyncAction;
		}

		public bool CanExecute
		{
			get
			{
				return canExecute_;
			}
			set
			{
				if (value != canExecute_)
				{
					canExecute_ = value;

					var changed = canExecuteChanged_;
					if (changed != null)
					{
						changed(this, EventArgs.Empty);
					}
				}
			}
		}

		event EventHandler ICommand.CanExecuteChanged
		{
			add
			{
				lock (this)
				{
					canExecuteChanged_ += value;
				}
			}
			remove
			{
				lock (this)
				{
					canExecuteChanged_ -= value;
				}
			}
		}

		bool ICommand.CanExecute(object parameter)
		{
			return canExecute_;
		}

		void ICommand.Execute(object parameter)
		{
			var task = asyncAction_();
		}

		Task ICommandReceiver.ExecuteAsync(object parameter)
		{
			return asyncAction_();
		}
	}

	public sealed class CommandReceiver<T> : ICommandReceiver
	{
		private bool canExecute_ = true;
		private EventHandler canExecuteChanged_;
		private readonly Func<T, Task> asyncAction_;

		internal CommandReceiver(Func<T, Task> asyncAction)
		{
			asyncAction_ = asyncAction;
		}

		public bool CanExecute
		{
			get
			{
				return canExecute_;
			}
			set
			{
				if (value != canExecute_)
				{
					canExecute_ = value;
					var changed = canExecuteChanged_;
					if (changed != null)
					{
						changed(this, EventArgs.Empty);
					}
				}
			}
		}

		event EventHandler ICommand.CanExecuteChanged
		{
			add
			{
				lock (this)
				{
					canExecuteChanged_ += value;
				}
			}
			remove
			{
				lock (this)
				{
					canExecuteChanged_ -= value;
				}
			}
		}

		bool ICommand.CanExecute(object parameter)
		{
			return canExecute_;
		}

		void ICommand.Execute(object parameter)
		{
			var task = asyncAction_((T)parameter);
		}

		Task ICommandReceiver.ExecuteAsync(object parameter)
		{
			return asyncAction_((T)parameter);
		}
	}
}
