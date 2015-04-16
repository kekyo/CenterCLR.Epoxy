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
	public sealed class FuncInvoker<TR> : MethodInvoker<Func<TR>>
	{
		internal FuncInvoker()
		{
		}
	}

	public sealed class FuncInvoker<T1, TR> : MethodInvoker<Func<T1, TR>>
	{
		internal FuncInvoker()
		{
		}
	}

	public sealed class FuncInvoker<T1, T2, TR> : MethodInvoker<Func<T1, T2, TR>>
	{
		internal FuncInvoker()
		{
		}
	}

	public sealed class FuncInvoker<T1, T2, T3, TR> : MethodInvoker<Func<T1, T2, T3, TR>>
	{
		internal FuncInvoker()
		{
		}
	}

	public sealed class FuncInvoker<T1, T2, T3, T4, TR> : MethodInvoker<Func<T1, T2, T3, T4, TR>>
	{
		internal FuncInvoker()
		{
		}
	}
}
