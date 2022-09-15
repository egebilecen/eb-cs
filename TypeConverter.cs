using System;

namespace EB_Utility
{
    public static class TypeConverter
    {
        public static string GetAssemblyTypeString(string nonAssemblyTypeName)
        {
            if(nonAssemblyTypeName == "sbyte"  || nonAssemblyTypeName == "sint8")  return "System.SByte";
            if(nonAssemblyTypeName == "byte"   || nonAssemblyTypeName == "int8")   return "System.Byte";


            if(nonAssemblyTypeName == "short"  || nonAssemblyTypeName == "int16")  return "System.Int16";
            if(nonAssemblyTypeName == "ushort" || nonAssemblyTypeName == "uint16") return "System.UInt16";

            if(nonAssemblyTypeName == "int"    || nonAssemblyTypeName == "int32")  return "System.Int32";
            if(nonAssemblyTypeName == "uint"   || nonAssemblyTypeName == "uint32") return "System.UInt32";

            if(nonAssemblyTypeName == "long"   || nonAssemblyTypeName == "int64")  return "System.Int64";
            if(nonAssemblyTypeName == "ulong"  || nonAssemblyTypeName == "uint64") return "System.UInt64";

            if(nonAssemblyTypeName == "nint")    return "System.IntPtr";
            if(nonAssemblyTypeName == "nuint")   return "System.UIntPtr";

            if(nonAssemblyTypeName == "float")   return "System.Single";
            if(nonAssemblyTypeName == "double")  return "System.Double";
            if(nonAssemblyTypeName == "decimal") return "System.Decimal";

            if(nonAssemblyTypeName == "string")  return "System.String";
            if(nonAssemblyTypeName == "bool")    return "System.Boolean";
            if(nonAssemblyTypeName == "char")    return "System.Char";
            if(nonAssemblyTypeName == "object")  return "System.Object";

            return null;
        }

        public static Type GetTypeFromString(string nonAssemblyTypeName)
        {
            return Type.GetType(GetAssemblyTypeString(nonAssemblyTypeName));
        }

        public static dynamic Convert(object obj, string convertionType)
        {
            return System.Convert.ChangeType(obj, GetTypeFromString(convertionType));
        }

        public static T Convert<T>(object obj)
        {
            return (T)System.Convert.ChangeType(obj, typeof(T));
        }
    }
}
