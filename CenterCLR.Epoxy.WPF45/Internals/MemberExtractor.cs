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
using System.Linq;

namespace CenterCLR.Epoxy.Internals
{
	internal static class MemberExtractor
	{
		public static MemberExtractor<T, U> Create<T, U>(Func<Type, string, Type[], T> memberInfoGetter, Func<T, U> selector)
			where T : class
			where U : class
		{
			return new MemberExtractor<T, U>(memberInfoGetter, selector);
		}
	}

	internal sealed class MemberExtractor<T, U>
		where T : class
		where U : class
	{
		private readonly Dictionary<MemberKey, U> memberEntries_ = new Dictionary<MemberKey, U>();
		private readonly Func<Type, string, Type[], T> memberInfoGetter_;
		private readonly Func<T, U> selector_;

		public MemberExtractor(Func<Type, string, Type[], T> memberInfoGetter, Func<T, U> selector)
		{
			memberInfoGetter_ = memberInfoGetter;
			selector_ = selector;
		}

		private IEnumerable<TypesByMember> TraverseHierarchy(Type derivedType, string memberName, Type[] signatureTypes)
		{
			var effectiveTypes = new List<Type>();
			var currentType = derivedType;
			do
			{
				effectiveTypes.Add(currentType);

				var memberInfo = memberInfoGetter_(currentType, memberName, signatureTypes);
				if (memberInfo != null)
				{
					yield return new TypesByMember(memberInfo, effectiveTypes);
					effectiveTypes.Clear();
				}

				currentType = currentType.GetBaseTypeTrampoline();
			}
			while (currentType != null);

			if (effectiveTypes.Count >= 1)
			{
				yield return new TypesByMember(null, effectiveTypes);
			}
		}

		public U TryGetMember(Type derivedType, string memberName, Type[] signatureTypes)
		{
			var memberKey = new MemberKey(derivedType, memberName, signatureTypes);

			U memberValue;
			if (memberEntries_.TryGetValue(memberKey, out memberValue) == true)
			{
				return memberValue;
			}

			foreach (var entry in
				from entry in TraverseHierarchy(derivedType, memberName, signatureTypes)
				from effectType in entry.EffectTypes
				let key = new MemberKey(effectType, memberName, signatureTypes)
				where memberEntries_.ContainsKey(key) == false
				select new { Key = key, Value = (entry.Member != null) ? selector_(entry.Member) : null })
			{
				memberEntries_.Add(entry.Key, entry.Value);
			}

			return memberEntries_[memberKey];
		}

		private struct TypesByMember
		{
			public readonly T Member;
			public readonly IEnumerable<Type> EffectTypes;

			public TypesByMember(T member, IEnumerable<Type> effectTypes)
			{
				this.Member = member;
				this.EffectTypes = effectTypes;
			}
		}

		private sealed class MemberKey : IEquatable<MemberKey>
		{
			public readonly Type EffectType;
			public readonly string MemberName;
			public readonly Type[] SignatureTypes;

			public MemberKey(Type effectType, string memberName, Type[] signatureTypes)
			{
				this.EffectType = effectType;
				this.MemberName = memberName;
				this.SignatureTypes = signatureTypes;
			}

			public override int GetHashCode()
			{
				return this.EffectType.GetHashCode() ^ this.MemberName.GetHashCode();
			}

			public bool Equals(MemberKey other)
			{
				return
					this.EffectType.Equals(other.EffectType) &&
					this.MemberName.Equals(other.MemberName) &&
					this.SignatureTypes.SequenceEqual(other.SignatureTypes);
			}
		}
	}
}
