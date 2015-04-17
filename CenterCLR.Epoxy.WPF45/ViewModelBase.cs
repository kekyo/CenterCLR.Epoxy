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

using CenterCLR.Epoxy.Internals;

namespace CenterCLR.Epoxy
{
	public abstract class ViewModelBase : ModelBase
	{
		protected ViewModelBase()
		{
		}

		internal ViewModelBase(ModelValues values)
			: base(values)
		{
		}

		protected CommandReceiver CreateReceiver(Func<Task> asyncAction)
		{
			return new CommandReceiver(asyncAction);
		}

		protected CommandReceiver<T> CreateReceiver<T>(Func<T, Task> asyncAction)
		{
			return new CommandReceiver<T>(asyncAction);
		}

		protected ActionInvoker CreateAction()
		{
			return new ActionInvoker();
		}

		protected ActionInvoker<T1> CreateAction<T1>()
		{
			return new ActionInvoker<T1>();
		}

		protected ActionInvoker<T1, T2> CreateAction<T1, T2>()
		{
			return new ActionInvoker<T1, T2>();
		}

		protected ActionInvoker<T1, T2, T3> CreateAction<T1, T2, T3>()
		{
			return new ActionInvoker<T1, T2, T3>();
		}

		protected ActionInvoker<T1, T2, T3, T4> CreateAction<T1, T2, T3, T4>()
		{
			return new ActionInvoker<T1, T2, T3, T4>();
		}

		protected FuncInvoker<TR> CreateFunc<TR>()
		{
			return new FuncInvoker<TR>();
		}

		protected FuncInvoker<T1, TR> CreateFunc<T1, TR>()
		{
			return new FuncInvoker<T1, TR>();
		}

		protected FuncInvoker<T1, T2, TR> CreateFunc<T1, T2, TR>()
		{
			return new FuncInvoker<T1, T2, TR>();
		}

		protected FuncInvoker<T1, T2, T3, TR> CreateFunc<T1, T2, T3, TR>()
		{
			return new FuncInvoker<T1, T2, T3, TR>();
		}

		protected FuncInvoker<T1, T2, T3, T4, TR> CreateFunc<T1, T2, T3, T4, TR>()
		{
			return new FuncInvoker<T1, T2, T3, T4, TR>();
		}
	}

	public abstract class ViewModelBase<T> : ViewModelBase
		where T : ViewModelBase, new()
	{
		protected ViewModelBase()
			: base(new ModelValues<T>())
		{
		}
	}
}
