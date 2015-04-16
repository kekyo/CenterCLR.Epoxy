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

#if WINFX_CORE
using Windows.UI.Xaml;
#else
using System.Windows;
using System.Windows.Media.Animation;
#endif

using CenterCLR.Epoxy.Gluing.Internals;

namespace CenterCLR.Epoxy.Gluing
{
	public abstract class GlueItemBase :
#if NET35 || NET40 || NET45
 Animatable,
#else
		DependencyObject,
#endif
 IInternalGlueItem
	{
		internal GlueItemBase()
		{
		}

		protected DependencyObject Target
		{
			get;
			private set;
		}

		void IInternalGlueItem.SetTarget(DependencyObject target)
		{
			var oldValue = this.Target;
			if (object.ReferenceEquals(target, oldValue) == true)
			{
				return;
			}

			this.Target = target;
			this.OnSetTarget(oldValue, this.Target);
		}

		protected abstract void OnSetTarget(DependencyObject oldTarget, DependencyObject newTarget);
	}

	public abstract class GlueItemBase<T> : GlueItemBase
		where T : GlueItemBase, new()
	{
		protected GlueItemBase()
		{
		}

#if NET35 || NET40 || NET45
		protected override sealed Freezable CreateInstanceCore()
		{
			return new T();
		}
#endif
	}
}
