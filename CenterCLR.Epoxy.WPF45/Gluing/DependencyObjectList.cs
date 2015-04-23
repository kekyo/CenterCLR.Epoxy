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
#endif
#if WIN32 || SILVERLIGHT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Threading;
#endif
#if XAMARIN
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using FrameworkElement = Xamarin.Forms.Element;
#endif

namespace CenterCLR.Epoxy.Gluing
{
#if WIN32
	public abstract class DependencyObjectList<T, U> : Freezable,
#else
	public abstract class DependencyObjectList<T> : DependencyObject,
#endif
		IList<T>,
		IList,
#if !XAMARIN
		INotifyPropertyChanged,
#endif
		INotifyCollectionChanged
#if WIN32
		where T : Freezable
		where U : Freezable, IList<T>, IList, INotifyPropertyChanged, INotifyCollectionChanged, new()
#else
		where T : DependencyObject
#endif
	{
		private readonly List<T> list_ = new List<T>();

#if !XAMARIN
		public event PropertyChangedEventHandler PropertyChanged;
#endif
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		protected DependencyObjectList()
		{
		}

#if WIN32
		protected override sealed Freezable CreateInstanceCore()
		{
			return new U();
		}

		protected override bool FreezeCore(bool isChecking)
		{
			if (base.FreezeCore(isChecking) == false)
			{
				return false;
			}

			foreach (var value in list_)
			{
				var freezable = value as Freezable;
				if (freezable != null)
				{
					if (Freezable.Freeze(freezable, isChecking) == false)
					{
						return false;
					}
				}
				else
				{
					var d = value as DispatcherObject;
					if ((d != null) && (d.Dispatcher != null))
					{
						return false;
					}
				}
			}

			return true;
		}
#endif

		private void DemandReadAccess()
		{
#if WIN32
			base.VerifyAccess();
#endif
		}

		private void DemandWriteAccess()
		{
#if WIN32
			base.VerifyAccess();
			if (base.IsFrozen == true)
			{
				throw new InvalidOperationException(string.Format("{0}: Collection is already frozen.", this.GetType().FullName));
			}
#endif
		}

		private void InvokePropertyChanged(string propertyName)
		{
#if WIN32 || SILVERLIGHT || NETFX_CORE
			var propertyChanged = this.PropertyChanged;
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
#endif
#if XAMARIN
			base.OnPropertyChanged(propertyName);
#endif
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
				this.DemandReadAccess();
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
				this.DemandReadAccess();
	
				return list_[index];
			}
			set
			{
				this.DemandWriteAccess();

				var oldItem = list_[index];

#if WIN32
				this.OnFreezablePropertyChanged(oldItem, value);
#endif

				list_[index] = value;

				this.InvokePropertyChanged("Item[]");
				this.OnReplaced(value, oldItem, index);
			}
		}

		public int IndexOf(T item)
		{
			this.DemandReadAccess();

			return list_.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			this.DemandWriteAccess();

			list_.Insert(index, item);

#if WIN32
			this.OnFreezablePropertyChanged(null, item);
#endif

			this.InvokePropertyChanged("Item[]");
			this.InvokePropertyChanged("Count");
			this.OnAdded(item, index);
		}

		private int InternalAdd(T item)
		{
			this.DemandWriteAccess();

#if WIN32
			this.OnFreezablePropertyChanged(null, item);
#endif
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
			this.DemandWriteAccess();

			this.OnClearing();

#if WIN32
			foreach (var item in list_)
			{
				this.OnFreezablePropertyChanged(null, item);
			}
#endif

			list_.Clear();

			this.InvokePropertyChanged("Item[]");
			this.InvokePropertyChanged("Count");
			this.OnCleared();
		}

		public bool Contains(T item)
		{
			this.DemandReadAccess();

			return list_.Contains(item);
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			this.DemandReadAccess();

			list_.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			this.DemandWriteAccess();

			var index = list_.IndexOf(item);
			if (index == -1)
			{
				return false;
			}

#if WIN32
			this.OnFreezablePropertyChanged(item, null);
#endif

			list_.RemoveAt(index);

			this.InvokePropertyChanged("Item[]");
			this.InvokePropertyChanged("Count");
			this.OnRemoved(item, index);

			return true;
		}

		public void RemoveAt(int index)
		{
			this.DemandWriteAccess();

			var oldItem = list_[index];

#if WIN32
			this.OnFreezablePropertyChanged(oldItem, null);
#endif

			list_.RemoveAt(index);

			this.InvokePropertyChanged("Item[]");
			this.InvokePropertyChanged("Count");
			this.OnRemoved(oldItem, index);
		}

		public IEnumerator<T> GetEnumerator()
		{
			this.DemandReadAccess();

			return list_.GetEnumerator();
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		IEnumerator IEnumerable.GetEnumerator()
		{
			this.DemandReadAccess();

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
			this.DemandReadAccess();

			((IList)list_).CopyTo(array, index);
		}
		#endregion
	}
}
