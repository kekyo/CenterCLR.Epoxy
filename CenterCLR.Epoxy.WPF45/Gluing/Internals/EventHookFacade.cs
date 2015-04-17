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
using System.Reflection;

using CenterCLR.Epoxy.Internals;

namespace CenterCLR.Epoxy.Gluing.Internals
{
	internal sealed class EventHookFacade
	{
		private static readonly MethodInfo onInvokeMethod_ = typeof(EventGlue).GetMethodTrampoline("OnInvoke");

		private static readonly MemberExtractor<EventInfo, EventHookFacade> facades_ = MemberExtractor.Create(
			(type, name, signatureTypes) => type.GetEventTrampoline(name),
			TryCreateFacade);

		private static EventHookFacade TryCreateFacade(EventInfo eventInfo)
		{
			var eventType = eventInfo.EventHandlerType;
			var method = eventType.GetMethodTrampoline("Invoke");
			if (method == null)
			{
				return null;
			}

			var parameters = method.GetParameters();
			if ((parameters.Length != 2) || (parameters[0].ParameterType != typeof(object)))
			{
				return null;
			}

			return new EventHookFacade(eventInfo, parameters[1].ParameterType);
		}

		private readonly EventInfo eventInfo_;
		private readonly Type parameter1Type_;
		private MethodInfo method_;

		private EventHookFacade(EventInfo eventInfo, Type parameter1Type)
		{
			eventInfo_ = eventInfo;
			parameter1Type_ = parameter1Type;
		}

		public static EventHookFacade TryGetFacade(Type derivedType, string name)
		{
			return facades_.TryGetMember(derivedType, name, ReflectionTrampoline.EmptyTypes);
		}

		public void AddHandler(EventGlue parent, object target)
		{
			if (method_ == null)
			{
				method_ = onInvokeMethod_.MakeGenericMethod(parameter1Type_);
			}

			var d = method_.CreateDelegateTrampoline(eventInfo_.EventHandlerType, parent);
			eventInfo_.AddEventHandler(target, d);
		}

		public void RemoveHandler(EventGlue parent, object target)
		{
			if (method_ == null)
			{
				method_ = onInvokeMethod_.MakeGenericMethod(parameter1Type_);
			}

			var d = method_.CreateDelegateTrampoline(eventInfo_.EventHandlerType, parent);
			eventInfo_.RemoveEventHandler(target, d);
		}
	}
}
