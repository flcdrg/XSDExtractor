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
using System.Collections.Generic;

namespace JFDI.Utils.XSDExtractor
{
    /// <summary>
    /// </summary>
    public class ConfigurationSectionFinderCoOrdinator
    {
        private readonly string[] _assemblies;

        /// <summary>
        /// </summary>
        public ConfigurationSectionFinderCoOrdinator(string[] assemblies)
        {
            _assemblies = assemblies;
        }

        /// <summary>
        ///     Gets all the types that match the criteria in any of the assemblies
        ///     listed in the array.
        /// </summary>
        public Type[] GetConfigSectionTypes(string className)
        {
            var retVal = new List<Type>();

            foreach (var s in _assemblies)
            {
                var csf = new ConfigurationSectionFinder(s);
                var types = csf.GetConfigSectionTypes(className);
                retVal.AddRange(types);
            }

            return retVal.ToArray();
        }
    }
}