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
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace CenterCLR.Epoxy.Internals
{
	internal static class Utilities
	{
		public static Type[] GetDelegateParameterTypes<T>()
			where T : class
		{
			Debug.Assert(typeof(Delegate).IsAssignableFromTrampoline(typeof(T)));

			return DelegateParameterExtractor<T>.DelegateTypes;
		}

		public static ValidationContext CreateValidationContext(object instance, string propertyName)
		{
#if WINFX_CORE
			return new ValidationContext(instance)
			{
				MemberName = propertyName
			};
#else
			return new ValidationContext(instance, null, null)
			{
				MemberName = propertyName
			};
#endif
		}

		private static class DelegateParameterExtractor<T>
			where T : class
		{
			public static readonly Type[] DelegateTypes;

			static DelegateParameterExtractor()
			{
				Debug.Assert(typeof(Delegate).IsAssignableFromTrampoline(typeof(T)));

				var method = typeof(T).GetMethodTrampoline("Invoke");
				if (method == null)
				{
					DelegateTypes = ReflectionTrampoline.EmptyTypes;
				}
				else
				{
					var parameters = method.GetParameters();
					DelegateTypes = parameters.Select(parameter => parameter.ParameterType).ToArray();
				}
			}
		}
	}
}
