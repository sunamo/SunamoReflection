# SunamoReflection

Advanced reflection utilities for .NET providing object dumping, deep copying, property/field inspection, and type analysis.

## Features

- **Object Dumping**: Serialize objects to string representations using multiple providers (Reflection, XML, YAML, JSON, ObjectDumper.NET)
- **Deep Copy**: Create deep copies of objects via reflection with circular reference handling
- **Property/Field Access**: Get and set property and field values by name with case-insensitive matching
- **Type Inspection**: Query types for constants, methods, properties, and fields
- **Assembly Analysis**: Search loaded assemblies and enumerate referenced assemblies recursively

## Installation

```bash
dotnet add package SunamoReflection
```

## Quick Start

```csharp
using SunamoReflection;

// Dump an object as string
var dump = RH.DumpAsReflection(myObject);

// Get property names
var names = RH.GetPropertyNames(typeof(MyClass));

// Deep copy an object
var copy = RHCopy.Copy(originalObject);

// Check type hierarchy
bool isDerived = RH.IsOrIsDeriveFromBaseClass(childType, parentType);
```

## Target Frameworks

**TargetFrameworks:** `net10.0;net9.0;net8.0`

## Links

- [NuGet](https://www.nuget.org/profiles/sunamo)
- [GitHub](https://github.com/sunamo/PlatformIndependentNuGetPackages)
- [Developer Site](https://sunamo.cz)
- [Contact](mailto:radek.jancik@sunamo.cz)

## License

MIT
