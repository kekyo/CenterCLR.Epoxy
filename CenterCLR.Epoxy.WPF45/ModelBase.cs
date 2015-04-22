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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

using CenterCLR.Epoxy.Internals;

namespace CenterCLR.Epoxy
{
	[DataContract]
	public abstract class ModelBase : INotifyPropertyChanged, INotifyDataErrorInfo, IDisposable
#if WIN32 || SILVERLIGHT5 || WINDOWS_PHONE71 || WINDOWS_PHONE80
		, IDataErrorInfo
#endif
	{
		private ModelValues values_;
		private PropertyChangedEventHandler propertyChanged_;
		private EventHandler<DataErrorsChangedEventArgs> dataErrorsChanged_;

		protected ModelBase()
		{
			values_ = ModelValues.Create(this.GetType());
		}

		internal ModelBase(ModelValues values)
		{
			values_ = values;
		}

		~ModelBase()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);

			values_ = null;
			propertyChanged_ = null;
			dataErrorsChanged_ = null;

			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		private List<ValidationResult> GetValidationResults()
		{
			var context = Utilities.CreateValidationContext(this, null);
			var results = new List<ValidationResult>();
			Validator.TryValidateObject(this, context, results);

			return results;
		}

		private List<ValidationResult> GetValidationResults<T>(string propertyName)
		{
			var value = values_.GetValue<T>(propertyName);

			var context = Utilities.CreateValidationContext(this, propertyName);
			var results = new List<ValidationResult>();
			Validator.TryValidateProperty(value, context, results);

			return results;
		}

		private static string JoinString(IEnumerable<string> values)
		{
#if NET35
			return string.Join(Environment.NewLine, values.ToArray());
#else
			return string.Join(Environment.NewLine, values);
#endif
		}

#if WIN32 || SILVERLIGHT5 || WINDOWS_PHONE71 || WINDOWS_PHONE80
		[SuppressMessage("Microsoft.Design", "CA1033")]
		string IDataErrorInfo.Error
		{
			get
			{
				return JoinString(this.GetValidationResults().Select(result => result.ErrorMessage));
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		string IDataErrorInfo.this[string columnName]
		{
			get
			{
				var context = Utilities.CreateValidationContext(this, null);
				var results = new List<ValidationResult>();
				if (Validator.TryValidateObject(this, context, results) == true)
				{
					return string.Empty;
				}

				return JoinString(results.Select(r => r.ErrorMessage));
			}
		}
#endif

		[SuppressMessage("Microsoft.Design", "CA1033")]
		bool INotifyDataErrorInfo.HasErrors
		{
			get
			{
				return this.GetValidationResults().Count >= 1;
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add
			{
				lock (this)
				{
					propertyChanged_ += value;
				}
			}
			remove
			{
				lock (this)
				{
					propertyChanged_ -= value;
				}
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
		{
			add
			{
				lock (this)
				{
					dataErrorsChanged_ += value;
				}
			}
			remove
			{
				lock (this)
				{
					dataErrorsChanged_ -= value;
				}
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1033")]
		IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
		{
			return this.GetValidationResults<object>(propertyName);
		}

		private void InvokePropertyChanged(string propertyName)
		{
			var propertyChanged = propertyChanged_;
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private void InvokeErrorsChanged(string propertyName)
		{
			var dataErrorsChanged = dataErrorsChanged_;
			if (dataErrorsChanged != null)
			{
				dataErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
			}
		}

		protected T GetValue<T>([CallerMemberName] string propertyName = null)
		{
			return values_.GetValue<T>(propertyName);
		}

		protected bool SetValue<T>(T value, [CallerMemberName] string propertyName = null)
		{
			if (values_.SetValue(value, propertyName) == false)
			{
				return false;
			}

			this.InvokePropertyChanged(propertyName);
			this.InvokeErrorsChanged(propertyName);

			return true;
		}
	}

	public abstract class ModelBase<T> : ModelBase
		where T : ModelBase, new()
	{
		protected ModelBase()
			: base(new ModelValues<T>())
		{
		}
	}
}
