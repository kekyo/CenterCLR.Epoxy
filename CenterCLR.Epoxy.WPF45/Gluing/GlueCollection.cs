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
#endif

namespace CenterCLR.Epoxy.Gluing
{
#if NET35 || NET40 || NET45
	public sealed class GlueCollection : FreezableList<GlueBase, GlueCollection>
#else
	public sealed class GlueCollection : DependencyObjectList<GlueBase>
#endif
	{
		private DependencyObject target_;

		protected override void OnAdded(GlueBase newItem, int index)
		{
			newItem.SetTarget(target_);
			base.OnAdded(newItem, index);
		}

		protected override void OnRemoved(GlueBase oldItem, int index)
		{
			oldItem.SetTarget(null);
			base.OnRemoved(oldItem, index);
		}

		protected override void OnReplaced(GlueBase newItem, GlueBase oldItem, int index)
		{
			oldItem.SetTarget(null);
			newItem.SetTarget(target_);
			base.OnReplaced(newItem, oldItem, index);
		}

		protected override void OnClearing()
		{
			base.OnClearing();

			foreach (var item in this)
			{
				item.SetTarget(null);
			}
		}

		internal void SetTarget(DependencyObject target)
		{
			if (object.ReferenceEquals(target, target_) == true)
			{
				return;
			}

			target_ = target;

			foreach (var item in this)
			{
				item.SetTarget(target_);
			}
		}
	}
}
