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
using System.Linq;
using System.Reflection;

namespace CenterCLR.Epoxy.Internals
{
	internal static class ReflectionTrampoline
	{
		public static readonly Type[] EmptyTypes =
#if NETFX_CORE
			new Type[0];
#else
 Type.EmptyTypes;
#endif

		public static Delegate CreateDelegateTrampoline(this MethodInfo method, Type type, object instance)
		{
#if NETFX_CORE
			return method.CreateDelegate(type, instance);
#else
			return Delegate.CreateDelegate(type, instance, method);
#endif
		}

		public static MethodInfo[] GetMethodsTrampoline(this Type type)
		{
#if NETFX_CORE
			return type.GetTypeInfo().DeclaredMethods.ToArray();
#else
			return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
#endif
		}

		public static MethodInfo GetMethodTrampoline(this Type type, string methodName)
		{
#if NETFX_CORE
			return type.GetTypeInfo().GetDeclaredMethod(methodName);
#else
			return type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
#endif
		}

		public static PropertyInfo[] GetPropertiesTrampoline(this Type type)
		{
#if NETFX_CORE
			return type.GetTypeInfo().DeclaredProperties.ToArray();
#else
			return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
#endif
		}

		public static PropertyInfo GetPropertyTrampoline(this Type type, string propertyName)
		{
#if NETFX_CORE
			return type.GetTypeInfo().GetDeclaredProperty(propertyName);
#else
			return type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
#endif
		}

		public static EventInfo GetEventTrampoline(this Type type, string eventName)
		{
#if NETFX_CORE
			return type.GetTypeInfo().GetDeclaredEvent(eventName);
#else
			return type.GetEvent(eventName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
#endif
		}

		public static FieldInfo GetFieldTrampoline(this Type type, string fieldName)
		{
#if NETFX_CORE
			return type.GetTypeInfo().GetDeclaredField(fieldName);
#else
			return type.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
#endif
		}

		public static Type GetBaseTypeTrampoline(this Type type)
		{
#if NETFX_CORE
			return type.GetTypeInfo().BaseType;
#else
			return type.BaseType;
#endif
		}

		public static MethodInfo GetMethodInfoTrampoline(this Delegate dlg)
		{
#if NETFX_CORE
			return dlg.GetMethodInfo();
#else
			return dlg.Method;
#endif
		}

		public static bool IsAssignableFromTrampoline(this Type type, Type fromType)
		{
#if NETFX_CORE
			return type.GetTypeInfo().IsAssignableFrom(fromType.GetTypeInfo());
#else
			return type.IsAssignableFrom(fromType);
#endif
		}
	}
}
