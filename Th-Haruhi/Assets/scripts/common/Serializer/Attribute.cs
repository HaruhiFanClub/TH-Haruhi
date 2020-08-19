


using System;
using System.Collections;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class JsonObjectAttribute : Attribute
{
    public static implicit operator bool(JsonObjectAttribute _attribute)
    {
        return _attribute != null;
    }
}