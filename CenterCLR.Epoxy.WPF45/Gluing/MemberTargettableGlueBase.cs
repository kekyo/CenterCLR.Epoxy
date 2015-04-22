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
using System.ComponentModel;
using System.Reflection;
using Windows.UI.Xaml;
#endif
#if WIN32 || SILVERLIGHT
using System.ComponentModel;
using System.Reflection;
using System.Windows;
#endif
#if XAMARIN
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using FrameworkElement = Xamarin.Forms.Element;
#endif

using CenterCLR.Epoxy.Internals;

namespace CenterCLR.Epoxy.Gluing
{
	public abstract class MemberTargettableGlueBase<T> : GlueBase<T>
		where T : GlueBase<T>, new()
	{
		private static readonly MemberExtractor<FieldInfo, FieldInfo> fields_ = MemberExtractor.Create(
			(type, name, signatureTypes) => type.GetFieldTrampoline(name),
			property => property);

		public static readonly DependencyProperty TargetNameProperty = Utilities.Register<string>(
			"TargetName",
			typeof(MemberTargettableGlueBase<T>),
			null,
			OnTargetNameChanged);

		protected MemberTargettableGlueBase()
		{
		}

#if NET35 || NET40 || NET45
		[Bindable(true)]
#endif
		public string TargetName
		{
			get
			{
				return (string)base.GetValue(TargetNameProperty);
			}
			set
			{
				base.SetValue(TargetNameProperty, value);
			}
		}

		protected override FrameworkElement GetElementContext()
		{
			var elementContext = base.GetElementContext();
			if (elementContext == null)
			{
				return null;
			}

			if (string.IsNullOrEmpty(this.TargetName) == true)
			{
				return elementContext;
			}

			var current = elementContext;
			while (current != null)
			{
#if NET35 || NET40 || NET45
				var nameScope = NameScope.GetNameScope(current);
				if (nameScope != null)
				{
					var target = nameScope.FindName(this.TargetName) as FrameworkElement;
					if (target != null)
					{
						return target;
					}
				}
#endif
				var type = current.GetType();
				var field = fields_.TryGetMember(type, this.TargetName, ReflectionTrampoline.EmptyTypes);
				if (field != null)
				{
					var target = field.GetValue(current);
					return target as FrameworkElement;
				}

				current = current.GetParent() as FrameworkElement;
			}

			return null;
		}

		protected abstract void OnTargetNameChanged(string oldName, string newName);

		private static void OnTargetNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var glue = (MemberTargettableGlueBase<T>)d;
			glue.OnTargetNameChanged((string)e.OldValue, (string)e.NewValue);
		}
	}
}
