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
	public abstract class PropertyAccessor
	{
		internal static readonly MemberExtractor<PropertyInfo, PropertyInfo> properties_ = MemberExtractor.Create(
			(type, name, signatureTypes) => type.GetPropertiesTrampoline().
				FirstOrDefault(property => (property.Name == name) && (property.GetIndexParameters().Select(parameter => parameter.ParameterType).SequenceEqual(signatureTypes))),
			property => property);

		internal PropertyAccessor()
		{
		}

		internal abstract void SetTarget(object target, string name);

		public static PropertyAccessor<T> Create<T>()
		{
			return new PropertyAccessor<T>();
		}
	}

	public sealed class PropertyAccessor<T> : PropertyAccessor, IPropertyAccessor
	{
		static PropertyAccessor()
		{
			Debug.Assert(typeof(Delegate).IsAssignableFromTrampoline(typeof(T)));
		}

		private readonly WeakReference wr_ = new WeakReference(null);
		private PropertyInfo property_;

		public PropertyAccessor()
		{
		}

		internal override void SetTarget(object target, string name)
		{
			if ((target == null) || (name == null))
			{
				wr_.Target = null;
				property_ = null;
				return;
			}

			var type = target.GetType();
			property_ = properties_.TryGetMember(type, name, ReflectionTrampoline.EmptyTypes);
			if (property_ != null)
			{
				wr_.Target = target;
			}
			else
			{
				wr_.Target = null;
				Debug.WriteLine(string.Format("PropertyAccessor: warning: cannot found method: Type={0}, Name={1}", type.FullName, name));
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		void IPropertyAccessor.SetProperty(PropertyInfo property, object target)
		{
			if (property == null)
			{
				wr_.Target = null;
				property_ = null;
				return;
			}

			wr_.Target = target;
			property_ = property;
		}

		public T Value
		{
			get
			{
				var target = wr_.Target;
				if (target == null)
				{
					property_ = null;
					throw new InvalidOperationException("Binding source not assigned/released.");
				}

				return (T)property_.GetValue(target, null);
			}
			set
			{
				var target = wr_.Target;
				if (target == null)
				{
					property_ = null;
					throw new InvalidOperationException("Binding source not assigned/released.");
				}

				property_.SetValue(target, value, null);
			}
		}
	}
}
