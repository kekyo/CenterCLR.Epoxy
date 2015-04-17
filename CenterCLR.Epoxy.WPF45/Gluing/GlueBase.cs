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

#if NETFX_CORE
using Windows.UI.Xaml;
#else
using System.Windows;
using System.Windows.Media.Animation;
#endif

namespace CenterCLR.Epoxy.Gluing
{
	public abstract class GlueBase :
#if NET35 || NET40 || NET45
 Animatable
#else
		DependencyObject
#endif
	{
		internal GlueBase()
		{
		}

		protected DependencyObject Target
		{
			get;
			private set;
		}

		internal void SetTarget(DependencyObject target)
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

	public abstract class GlueBase<T> : GlueBase
		where T : GlueBase, new()
	{
		protected GlueBase()
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
