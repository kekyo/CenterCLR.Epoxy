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

namespace CenterCLR.Epoxy
{
	public sealed class ActionInvoker : MethodInvoker<Action>
	{
		internal ActionInvoker()
		{
		}
	}

	public sealed class ActionInvoker<T1> : MethodInvoker<Action<T1>>
	{
		internal ActionInvoker()
		{
		}
	}

	public sealed class ActionInvoker<T1, T2> : MethodInvoker<Action<T1, T2>>
	{
		internal ActionInvoker()
		{
		}
	}

	public sealed class ActionInvoker<T1, T2, T3> : MethodInvoker<Action<T1, T2, T3>>
	{
		internal ActionInvoker()
		{
		}
	}

	public sealed class ActionInvoker<T1, T2, T3, T4> : MethodInvoker<Action<T1, T2, T3, T4>>
	{
		internal ActionInvoker()
		{
		}
	}
}
