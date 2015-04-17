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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#else
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
#endif

using CenterCLR.Epoxy.Gluing.Internals;
using CenterCLR.Epoxy.Internals;

namespace CenterCLR.Epoxy.Gluing
{
#if NETFX_CORE
	[ContentProperty(Name = "ItemTemplate")]
#else
	[ContentProperty("ItemTemplate")]
#endif
#if NET35 || NET40 || NET45
	[DefaultProperty("ItemTemplate")]
#endif
	public sealed class ItemsGlue : GlueBase<ItemsGlue>
	{
		private static readonly MemberExtractor<PropertyInfo, PropertyInfo> properties_ = MemberExtractor.Create(
			(type, name, signatureTypes) => type.GetPropertyTrampoline(name),
			property => ((typeof(IList).IsAssignableFromTrampoline(property.PropertyType) == true) && (property.GetIndexParameters().Length == 0)) ? property : null);

		public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
			"Name",
			typeof(string),
			typeof(ItemsGlue),
			new PropertyMetadata(null, OnChanged));

		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
			"ItemsSource",
			typeof(IEnumerable),
			typeof(ItemsGlue),
			new PropertyMetadata(null, OnItemsSourceChanged));

#if NET35 || NET40 || NET45
		public static readonly DependencyProperty ItemStringFormatProperty = DependencyProperty.Register(
			"ItemStringFormat",
			typeof(string),
			typeof(ItemsGlue),
			new PropertyMetadata(null, OnChanged));
#endif

		public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
			"ItemTemplate",
			typeof(DataTemplate),
			typeof(ItemsGlue),
			new PropertyMetadata(null, OnChanged));

#if NET35 || NET40 || NET45 || NETFX_CORE
		public static readonly DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.Register(
			"ItemTemplateSelector",
			typeof(DataTemplateSelector),
			typeof(ItemsGlue),
			new PropertyMetadata(null, OnChanged));
#endif
		private IList targetList_;

		public ItemsGlue()
		{
		}

#if NET35 || NET40 || NET45
		[Bindable(true)]
#endif
		public string Name
		{
			get
			{
				return (string)base.GetValue(NameProperty);
			}
			set
			{
				base.SetValue(NameProperty, value);
			}
		}

#if NET35 || NET40 || NET45
		[Bindable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
		public IEnumerable ItemsSource
		{
			get
			{
				return (IEnumerable)base.GetValue(ItemsSourceProperty);
			}
			set
			{
				base.SetValue(ItemsSourceProperty, value);
			}
		}

#if NET35 || NET40 || NET45
		[Bindable(true)]
		public string ItemStringFormat
		{
			get
			{
				return (string)base.GetValue(ItemStringFormatProperty);
			}
			set
			{
				base.SetValue(ItemStringFormatProperty, value);
			}
		}
#endif

#if NET35 || NET40 || NET45
		[Bindable(true)]
#endif
		public DataTemplate ItemTemplate
		{
			get
			{
				return (DataTemplate)base.GetValue(ItemTemplateProperty);
			}
			set
			{
				base.SetValue(ItemTemplateProperty, value);
			}
		}

#if NET35 || NET40 || NET45 || NETFX_CORE
#if NET35 || NET40 || NET45
		[Bindable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
		public DataTemplateSelector ItemTemplateSelector
		{
			get
			{
				return (DataTemplateSelector)base.GetValue(ItemTemplateSelectorProperty);
			}
			set
			{
				base.SetValue(ItemTemplateProperty, value);
			}
		}
#endif

		private ItemContentControl CreateItemByValue(object value, int initialIndex)
		{
			var item = new ItemContentControl();
			item.SetContentValue(this, value);
			return item;
		}

		protected override void OnSetTarget(DependencyObject oldTarget, DependencyObject newTarget)
		{
			if (targetList_ != null)
			{
				targetList_.Clear();
				targetList_ = null;
			}

			if ((newTarget == null) || string.IsNullOrEmpty(this.Name))
			{
				return;
			}

			var type = newTarget.GetType();

			var property = properties_.TryGetMember(type, this.Name, ReflectionTrampoline.EmptyTypes);
			if (property == null)
			{
				return;
			}

			targetList_ = (IList)property.GetValue(newTarget, null);

			if ((targetList_ != null) && (this.ItemsSource != null) && (this.ItemTemplate != null))
			{
				var realIndex = 0;
				foreach (var value in this.ItemsSource)
				{
					targetList_.Add(this.CreateItemByValue(value, realIndex));
					realIndex++;
				}
			}
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if ((targetList_ == null) || (this.ItemTemplate == null))
			{
				return;
			}

			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					for (var index = 0; index < e.NewItems.Count; index++)
					{
						var realIndex = index + e.NewStartingIndex;
						targetList_.Insert(realIndex, this.CreateItemByValue(e.NewItems[index], realIndex));
					}
					break;

				case NotifyCollectionChangedAction.Remove:
					for (var index = e.OldItems.Count - 1; index >= 0; index--)
					{
						var realIndex = index + e.OldStartingIndex;
						var item = targetList_[realIndex] as ItemContentControl;
						if (item != null)
						{
							item.ResetContentValue();
						}
						targetList_.RemoveAt(realIndex);
					}
					break;

				case NotifyCollectionChangedAction.Reset:
					{
						targetList_.Clear();
						var realIndex = 0;
						foreach (var value in this.ItemsSource)
						{
							targetList_.Add(this.CreateItemByValue(value, realIndex));
							realIndex++;
						}
					}
					break;

				case NotifyCollectionChangedAction.Replace:
					for (var index = 0; index < e.NewItems.Count; index++)
					{
						var realIndex = index + e.NewStartingIndex;
						((ItemContentControl)targetList_[realIndex]).SetContentValue(e.NewItems[index]);
					}
					break;

#if NET35 || NET40 || NET45 || WINDOWS_PHONE80 || NETFX_CORE
				case NotifyCollectionChangedAction.Move:
					{
						var item = (ItemContentControl)targetList_[e.OldStartingIndex];
						targetList_.RemoveAt(e.OldStartingIndex);
						targetList_.Insert(e.NewStartingIndex, item);
					}
					break;
#endif
			}
		}

		private void OnChanged(DependencyPropertyChangedEventArgs e)
		{
			this.OnSetTarget(base.Target, base.Target);
		}

		private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var itemsGlue = (ItemsGlue)d;
			itemsGlue.OnChanged(e);
		}

		private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
		{
			var oldValue = e.OldValue as INotifyCollectionChanged;
			if (oldValue != null)
			{
				oldValue.CollectionChanged -= this.OnCollectionChanged;
			}

			this.OnSetTarget(base.Target, base.Target);

			var newValue = e.NewValue as INotifyCollectionChanged;
			if (newValue != null)
			{
				newValue.CollectionChanged += this.OnCollectionChanged;
			}
		}

		private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var itemsGlue = (ItemsGlue)d;
			itemsGlue.OnItemsSourceChanged(e);
		}
	}
}
