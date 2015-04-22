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
#endif
#if WIN32 || SILVERLIGHT
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
#if XAMARIN
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Windows;
using Xamarin.Forms;
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using FrameworkElement = Xamarin.Forms.Element;
#endif

using CenterCLR.Epoxy.Gluing.Internals;
using CenterCLR.Epoxy.Internals;

namespace CenterCLR.Epoxy.Gluing
{
#if NETFX_CORE
	[ContentProperty(Name = "ItemTemplate")]
#endif
#if WIN32 || SILVERLIGHT || XAMARIN
	[ContentProperty("ItemTemplate")]
#endif
#if WIN32
	[DefaultProperty("ItemTemplate")]
#endif
	public sealed class ItemsGlue : GlueBase<ItemsGlue>
	{
		private static readonly MemberExtractor<PropertyInfo, PropertyInfo> properties_ = MemberExtractor.Create(
			(type, name, signatureTypes) => type.GetPropertyTrampoline(name),
			property => ((typeof(IList).IsAssignableFromTrampoline(property.PropertyType) == true) && (property.GetIndexParameters().Length == 0)) ? property : null);

		public static readonly DependencyProperty NameProperty = Utilities.Register<string>(
			"Name",
			typeof(ItemsGlue),
			null,
			OnChanged);

		public static readonly DependencyProperty ItemsSourceProperty = Utilities.Register<IEnumerable>(
			"ItemsSource",
			typeof(ItemsGlue),
			null,
			OnItemsSourceChanged);
#if WIN32
		public static readonly DependencyProperty ItemStringFormatProperty = Utilities.Register<string>(
			"ItemStringFormat",
			typeof(ItemsGlue),
			null,
            OnChanged);
#endif

		public static readonly DependencyProperty ItemTemplateProperty = Utilities.Register<DataTemplate>(
			"ItemTemplate",
			typeof(ItemsGlue),
			null,
			OnChanged);

#if WIN32 || NETFX_CORE
		public static readonly DependencyProperty ItemTemplateSelectorProperty = Utilities.Register<DataTemplateSelector>(
			"ItemTemplateSelector",
			typeof(ItemsGlue),
			null,
			OnChanged);
#endif
		private IList targetList_;

		public ItemsGlue()
		{
		}

#if WIN32
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

#if WIN32
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

#if WIN32
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

#if WIN32
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

#if WIN32 || NETFX_CORE
#if WIN32
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

		protected override void OnSetElementContext(FrameworkElement oldTarget, FrameworkElement newTarget)
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

#if WIN32 || WINDOWS_PHONE80 || NETFX_CORE || XAMARIN
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
			var elementContext = base.GetElementContext();
			this.OnSetElementContext(elementContext, elementContext);
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

			var elementContext = base.GetElementContext();
			this.OnSetElementContext(elementContext, elementContext);

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
