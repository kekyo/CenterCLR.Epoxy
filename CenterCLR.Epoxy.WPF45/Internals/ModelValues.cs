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
using System.Collections.Generic;

namespace CenterCLR.Epoxy.Internals
{
	internal abstract class ModelValues
	{
		private static readonly Dictionary<Type, Factory> factories_ = new Dictionary<Type, Factory>();

		public abstract T GetValue<T>(string propertyName);
		public abstract bool SetValue<T>(T value, string propertyName);

		public static ModelValues Create(Type modelType)
		{
			Factory factory;
			if (factories_.TryGetValue(modelType, out factory) == false)
			{
				var factoryType = typeof(Factory<>).MakeGenericType(modelType);
				factory = (Factory)Activator.CreateInstance(factoryType);
				factories_.Add(modelType, factory);
			}

			return factory.Create();
		}

		private abstract class Factory
		{
			public abstract ModelValues Create();
		}

		private sealed class Factory<T> : Factory
			where T : ModelBase
		{
			public override ModelValues Create()
			{
				return new ModelValues<T>();
			}
		}
	}

	internal sealed class ModelValues<T> : ModelValues
		where T : ModelBase
	{
		private Dictionary<string, object> values_;

		public override U GetValue<U>(string propertyName)
		{
			if (values_ == null)
			{
				return default(U);
			}

			object value;
			if (values_.TryGetValue(propertyName, out value) == false)
			{
				return default(U);
			}

			return (U)value;
		}

		public override bool SetValue<U>(U value, string propertyName)
		{
			if (values_ == null)
			{
				values_ = new Dictionary<string, object>();
				values_.Add(propertyName, value);
				return true;
			}

			U oldValue;
			object rawOldValue;
			if (values_.TryGetValue(propertyName, out rawOldValue) == false)
			{
				oldValue = default(U);
			}
			else
			{
				oldValue = (U)rawOldValue;
			}

			if ((oldValue == null) && (value == null))
			{
				return false;
			}

			if ((oldValue != null) && (oldValue.Equals(value) == true))
			{
				return false;
			}

			values_[propertyName] = value;

			return true;
		}
	}
}
