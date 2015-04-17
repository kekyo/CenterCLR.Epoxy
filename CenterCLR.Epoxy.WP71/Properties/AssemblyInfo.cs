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

using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Markup;

[assembly: AssemblyTitle("CenterCLR.Epoxy.WP75")]

[assembly: ComVisible(false)]
[assembly: Guid("CD8F1255-3BFD-4D6A-8451-3BD3B4DD6936")]

[assembly: XmlnsDefinition("http://schemas.microsoft.com/xps/2005/06", "CenterCLR.Epoxy.Gluing")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/xps/2005/06", "CenterCLR.Epoxy.UnitTesting")]

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "CenterCLR.Epoxy.Gluing")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "CenterCLR.Epoxy.UnitTesting")]

[assembly: XmlnsDefinition("http://schemas.microsoft.com/netfx/2007/xaml/presentation", "CenterCLR.Epoxy.Gluing")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/netfx/2007/xaml/presentation", "CenterCLR.Epoxy.UnitTesting")]

[assembly: XmlnsDefinition("http://schemas.microsoft.com/netfx/2009/xaml/presentation", "CenterCLR.Epoxy.Gluing")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/netfx/2009/xaml/presentation", "CenterCLR.Epoxy.UnitTesting")]

[assembly: XmlnsPrefix("http://schemas.microsoft.com/xps/2005/06", "metro")]
[assembly: XmlnsPrefix("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "av")]
[assembly: XmlnsPrefix("http://schemas.microsoft.com/netfx/2007/xaml/presentation", "wpf")]
[assembly: XmlnsPrefix("http://schemas.microsoft.com/netfx/2009/xaml/presentation", "wpf")]
