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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
#else
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
#endif

using CenterCLR.Epoxy.Gluing.Internals;

namespace CenterCLR.Epoxy.Gluing
{
	public sealed class GlueItemCollection :
#if NET35 || NET40 || NET45
 Freezable,
#else
 DependencyObject,
#endif
 IList<IGlueItem>,
		IList,
		INotifyPropertyChanged,
		INotifyCollectionChanged,
		IInternalGlueItem
	{
		private DependencyObject target_;
		private readonly ObservableCollection<IInternalGlueItem> list_ = new ObservableCollection<IInternalGlueItem>();

		public GlueItemCollection()
		{
			((INotifyPropertyChanged)list_).PropertyChanged += this.OnPropertyChanged;
			((INotifyCollectionChanged)list_).CollectionChanged += this.OnCollectionChanged;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var propertyChanged = this.PropertyChanged;
			if (propertyChanged != null)
			{
				propertyChanged(this, e);
			}
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (IInternalGlueItem item in e.NewItems)
					{
						item.SetTarget(target_);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (IInternalGlueItem item in e.OldItems)
					{
						item.SetTarget(null);
					}
					break;
				case NotifyCollectionChangedAction.Replace:
					foreach (IInternalGlueItem item in e.OldItems)
					{
						item.SetTarget(null);
					}
					foreach (IInternalGlueItem item in e.NewItems)
					{
						item.SetTarget(target_);
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					// TODO: oldItems cannot reset
					foreach (IInternalGlueItem item in this)
					{
						item.SetTarget(null);
					}
					break;
			}

			var collectionChanged = this.CollectionChanged;
			if (collectionChanged != null)
			{
				collectionChanged(this, e);
			}
		}

#if NET35 || NET40 || NET45
		protected override Freezable CreateInstanceCore()
		{
			return new GlueItemCollection();
		}
#endif

		#region IInternalGlueItem
		void IInternalGlueItem.SetTarget(DependencyObject target)
		{
			if (object.ReferenceEquals(target, target_) == true)
			{
				return;
			}

			target_ = target;

			foreach (IInternalGlueItem item in list_)
			{
				item.SetTarget(target_);
			}
		}
		#endregion

		#region IList<IGlueItem>
		public int Count
		{
			get
			{
				return list_.Count;
			}
		}

		bool ICollection<IGlueItem>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public IGlueItem this[int index]
		{
			get
			{
				return list_[index];
			}
			set
			{
				list_[index] = (IInternalGlueItem)value;
			}
		}

		public int IndexOf(IGlueItem item)
		{
			return list_.IndexOf((IInternalGlueItem)item);
		}

		public void Insert(int index, IGlueItem item)
		{
			list_.Insert(index, (IInternalGlueItem)item);
		}

		public void RemoveAt(int index)
		{
			list_.RemoveAt(index);
		}

		public void Add(IGlueItem item)
		{
			list_.Add((IInternalGlueItem)item);
		}

		public void Clear()
		{
			list_.Clear();
		}

		public bool Contains(IGlueItem item)
		{
			return list_.Contains((IInternalGlueItem)item);
		}

		void ICollection<IGlueItem>.CopyTo(IGlueItem[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(IGlueItem item)
		{
			return list_.Remove((IInternalGlueItem)item);
		}

		public IEnumerator<IGlueItem> GetEnumerator()
		{
			return list_.Cast<IGlueItem>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return list_.GetEnumerator();
		}
		#endregion

		#region IList
		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return list_[index];
			}
			set
			{
				list_[index] = (IInternalGlueItem)value;
			}
		}

		int IList.Add(object value)
		{
			return ((IList)list_).Add((IInternalGlueItem)value);
		}

		bool IList.Contains(object value)
		{
			return list_.Contains((IInternalGlueItem)value);
		}

		int IList.IndexOf(object value)
		{
			return list_.IndexOf((IInternalGlueItem)value);
		}

		void IList.Insert(int index, object value)
		{
			list_.Insert(index, (IInternalGlueItem)value);
		}

		void IList.Remove(object value)
		{
			list_.Remove((IInternalGlueItem)value);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
