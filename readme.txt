XSDExtractor v1.1.1 (c) 2006 Steve Ward (steve.ward.uk@gmail.com)
Released under the GNU Lesser General Public License

XSDExtractor will attempt to extract an Xml Schema (XSD) from a compiled
ConfigurationSection type. It does this by searching for
ConfigurationPropertyAttribute and ConfigurationCollectionAttribute attributes.

Warning: This program will give inconsistent results if the type being
converted has used the programmatic way of creating ConfigurationSection
sub-classes as opposed to the declaritive model using attributes.
--------------------------------------------------------------------------------

XSDExtractor History:

24 September, 2006
	Version 1.1.1 Released
	
       Fixes:
          o  Invalid schema was created when a collection was re-used more than once. XDSExtractor
		     incorrectly generated two or more complex types with the same name. Fix provided by 
		     Idael Cardoso @ CodeProject. Unit test added to recreate bug and validate bugfix

       Enhancements:
          o Nant build script added to the solution
		    

10 August, 2006
    Version 1.1 Released

      Fixes:
          o The /R switch now works correctly
          o StringValidatorAttribute's that have a blank list of invalidcharacters no longer cause a blank pattern element to be generated (which caused the XSD to be invalid)
          o xmlns properties are now ignored

      Enhancements:
          o Documentation is now added to the XSD. Standard information on the field type, the default value and whether the field is required or not is now displayed in a tooltip in the VS2005 XML editor. If the property is decorated with a System.ComponentModel.DescriptionAttribute then the description is appended to the default information also
          o Correctly handles default collections such as the HttpModules configuration section
          o Useful information is added as a comment to the XSD so that it is possible to see how the XSD was generated, who by and when

1 August, 2006
	Version 1.0 released

--------------------------------------------------------------------------------

Usage: 
XSDExtractor [/R root] [/C class] [/A assembly] [/S bool]

    /R          Name of the Xsd root element
    root        If ommited then the type name of the configurationsection
                is used instead.

    /C          Name of the class which should be converted to an Xsd. If
                ommited then all classes are examined.
    class       Full name (including namespace) of class

    /A          Name of the assembly which should be examined. If ommited then
                all assemblies in the current directory are examined.
    assembly    Name of the assembly to inspect (path information is optional)

    /S          Silence.
    bool        If true then the user is not prompted at any point.

License:
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.