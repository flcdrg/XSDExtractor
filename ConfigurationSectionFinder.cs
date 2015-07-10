#region License

/*
JFDI the .Net Job Framework (http://jfdi.sourceforge.net)
Copyright (C) 2006  Steven Ward (steve.ward.uk@gmail.com)

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
*/

#endregion

using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace JFDI.Utils.XSDExtractor
{
    /// <summary>
    ///     Responsible for finding all the types in the assembly which
    ///     inherit from the ConfigurationSection class
    /// </summary>
    public class ConfigurationSectionFinder
    {
        private readonly string _assemblyLocation;

        public ConfigurationSectionFinder(string assemblyLocation)
        {
            _assemblyLocation = assemblyLocation;
        }

        public Type[] GetConfigSectionTypes(string className)
        {
            var ass = Assembly.LoadFrom(_assemblyLocation);
            var types = ass.GetTypes();

            return
                types.Where(
                    t =>
                        (!string.IsNullOrEmpty(className) && t.FullName == className) || string.IsNullOrEmpty(className))
                    .Where(t => t.IsSubclassOf(typeof(ConfigurationSection)))
                    .ToArray();
        }
    }
}