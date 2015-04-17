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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using CenterCLR.Epoxy.Internals;
using CenterCLR.Epoxy.UnitTesting;

namespace CenterCLR.Epoxy
{
	public abstract class MethodInvoker
	{
		internal static readonly MemberExtractor<MethodInfo, MethodInfo> methods_ = MemberExtractor.Create(
			(type, name, signatureTypes) => type.GetMethodsTrampoline().
				FirstOrDefault(method => (method.Name == name) && (method.GetParameters().Select(parameter => parameter.ParameterType).SequenceEqual(signatureTypes))),
			method => method);

		internal MethodInvoker()
		{
		}

		internal abstract void SetTarget(object target, string name);

		public static ActionInvoker CreateAction()
		{
			return new ActionInvoker();
		}

		public static ActionInvoker<T1> CreateAction<T1>()
		{
			return new ActionInvoker<T1>();
		}

		public static ActionInvoker<T1, T2> CreateAction<T1, T2>()
		{
			return new ActionInvoker<T1, T2>();
		}

		public static ActionInvoker<T1, T2, T3> CreateAction<T1, T2, T3>()
		{
			return new ActionInvoker<T1, T2, T3>();
		}

		public static ActionInvoker<T1, T2, T3, T4> CreateAction<T1, T2, T3, T4>()
		{
			return new ActionInvoker<T1, T2, T3, T4>();
		}

		public static FuncInvoker<TR> CreateFunc<TR>()
		{
			return new FuncInvoker<TR>();
		}

		public static FuncInvoker<T1, TR> CreateFunc<T1, TR>()
		{
			return new FuncInvoker<T1, TR>();
		}

		public static FuncInvoker<T1, T2, TR> CreateFunc<T1, T2, TR>()
		{
			return new FuncInvoker<T1, T2, TR>();
		}

		public static FuncInvoker<T1, T2, T3, TR> CreateFunc<T1, T2, T3, TR>()
		{
			return new FuncInvoker<T1, T2, T3, TR>();
		}

		public static FuncInvoker<T1, T2, T3, T4, TR> CreateFunc<T1, T2, T3, T4, TR>()
		{
			return new FuncInvoker<T1, T2, T3, T4, TR>();
		}
	}

	public abstract class MethodInvoker<T> : MethodInvoker, IMethodInvoker<T>
		where T : class
	{
		static MethodInvoker()
		{
			Debug.Assert(typeof(Delegate).IsAssignableFromTrampoline(typeof(T)));
		}

		private readonly WeakReference wr_ = new WeakReference(null);
		private MethodInfo method_;

		protected MethodInvoker()
		{
		}

		internal override void SetTarget(object target, string name)
		{
			if ((target == null) || (name == null))
			{
				wr_.Target = null;
				method_ = null;
				return;
			}

			var type = target.GetType();
			method_ = methods_.TryGetMember(type, name, Utilities.GetDelegateParameterTypes<T>());
			if (method_ != null)
			{
				wr_.Target = target;
			}
			else
			{
				wr_.Target = null;
				Debug.WriteLine(string.Format("MethodInvoker: warning: cannot found method: Type={0}, Name={1}", type.FullName, name));
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		void IMethodInvoker<T>.SetAction(T action)
		{
			if (action == null)
			{
				wr_.Target = null;
				method_ = null;
				return;
			}

			var dlg = (Delegate)(object)action;
			wr_.Target = dlg.Target;
			method_ = dlg.GetMethodInfoTrampoline();
		}

		public T Invoke
		{
			get
			{
				var target = wr_.Target;
				if (target == null)
				{
					method_ = null;
					throw new InvalidOperationException("Binding source not assigned/released.");
				}

				return (T)(object)method_.CreateDelegateTrampoline(typeof(T), target);
			}
		}
	}
}
