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

using System.Collections.Generic;

namespace System.ComponentModel.DataAnnotations
{
	internal static class Validator
	{
		public static bool TryValidateObject(object instance, ValidationContext validationContext, ICollection<ValidationResult> validationResults)
		{
			return true;
		}

		public static bool TryValidateProperty(object value, ValidationContext validationContext, ICollection<ValidationResult> validationResults)
		{
			return true;
		}
	}
}
