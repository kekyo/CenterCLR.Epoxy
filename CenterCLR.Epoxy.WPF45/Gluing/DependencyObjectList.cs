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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Windows.UI.Xaml;
#else
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
#endif

namespace CenterCLR.Epoxy.Gluing
{
#if NET35 || NET40 || NET45
	public class FreezableList<T, U> : Freezable,
#else
	public class DependencyObjectList<T> : DependencyObject,
#endif
		IList<T>,
		IList,
		INotifyPropertyChanged,
		INotifyCollectionChanged
#if NET35 || NET40 || NET45
		where U :
 Freezable, IList<T>, IList, INotifyPropertyChanged, INotifyCollectionChanged, new()
#endif
	{
		private readonly List<T> list_ = new List<T>();

		public event PropertyChangedEventHandler PropertyChanged;
		public event NotifyCollectionChangedEventHandler CollectionChanged;

#if NET35 || NET40 || NET45
		protected FreezableList()
#else
		protected DependencyObjectList()
#endif
		{
		}

#if NET35 || NET40 || NET45
		protected override sealed Freezable CreateInstanceCore()
		{
			return new U();
		}
#endif

		private void InvokePropertyChanged(string propertyName)
		{
			var propertyChanged = this.PropertyChanged;
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		protected virtual void OnAdded(T newItem, int index)
		{
			var collectionChanged = this.CollectionChanged;
			if (collectionChanged != null)
			{
				collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[] { newItem }, index));
			}
		}

		protected virtual void OnRemoved(T oldItem, int index)
		{
			var collectionChanged = this.CollectionChanged;
			if (collectionChanged != null)
			{
				collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] { oldItem }, index));
			}
		}

		protected virtual void OnReplaced(T newItem, T oldItem, int index)
		{
			var collectionChanged = this.CollectionChanged;
			if (collectionChanged != null)
			{
				collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new[] { newItem }, new[] { oldItem }, index));
			}
		}

		protected virtual void OnClearing()
		{
		}

		protected virtual void OnCleared()
		{
			var collectionChanged = this.CollectionChanged;
			if (collectionChanged != null)
			{
				collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		#region IList<T>
		public int Count
		{
			get
			{
				return list_.Count;
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public T this[int index]
		{
			get
			{
				return list_[index];
			}
			set
			{
				var oldItem = list_[index];
				list_[index] = value;

				this.InvokePropertyChanged("Item[]");
				this.OnReplaced(value, oldItem, index);
			}
		}

		public int IndexOf(T item)
		{
			return list_.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			list_.Insert(index, item);

			this.InvokePropertyChanged("Item[]");
			this.InvokePropertyChanged("Count");
			this.OnAdded(item, index);
		}

		public void RemoveAt(int index)
		{
			var oldItem = list_[index];
			list_.RemoveAt(index);

			this.InvokePropertyChanged("Item[]");
			this.InvokePropertyChanged("Count");
			this.OnRemoved(oldItem, index);
		}

		private int InternalAdd(T item)
		{
			var index = list_.Count;
			list_.Add(item);

			this.InvokePropertyChanged("Item[]");
			this.InvokePropertyChanged("Count");
			this.OnAdded(item, index);

			return index;
		}

		public void Add(T item)
		{
			this.InternalAdd(item);
		}

		public void Clear()
		{
			this.OnClearing();

			list_.Clear();

			this.InvokePropertyChanged("Item[]");
			this.InvokePropertyChanged("Count");
			this.OnCleared();
		}

		public bool Contains(T item)
		{
			return list_.Contains(item);
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			list_.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			var index = list_.IndexOf(item);
			if (index == -1)
			{
				return false;
			}

			list_.RemoveAt(index);

			this.InvokePropertyChanged("Item[]");
			this.InvokePropertyChanged("Count");
			this.OnRemoved(item, index);

			return true;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return list_.GetEnumerator();
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return list_.GetEnumerator();
		}
		#endregion

		#region IList
		[SuppressMessage("Microsoft.Design", "CA1033")]
		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (T)value;
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		int IList.Add(object value)
		{
			return this.InternalAdd((T)value);
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		bool IList.Contains(object value)
		{
			return this.Contains((T)value);
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		int IList.IndexOf(object value)
		{
			return this.IndexOf((T)value);
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		void IList.Insert(int index, object value)
		{
			this.Insert(index, (T)value);
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		void IList.Remove(object value)
		{
			this.Remove((T)value);
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		void ICollection.CopyTo(Array array, int index)
		{
			((IList)list_).CopyTo(array, index);
		}
		#endregion
	}
}
