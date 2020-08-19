
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public static class Common
{
    public static T Default<T>()
    {
        return default(T);
    }

    public static object Default(Type type)
    {
        if (type.IsValueType)
            return Activator.CreateInstance(type);
        return null;
    }

    public static Type GetType(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
            return null;
        else
            return Type.GetType(typeName);
    }

    public static bool IsType(Type to, Type from)
    {
        return from == to || from.IsSubclassOf(to);
    }

    public static Type GetNestedType(
       Type type, string nestedName, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.NonPublic)
    {
        Type[] nestedList = type.GetNestedTypes(bindingAttr);
        int c = nestedList.Length;
        for (int i = 0; i < c; ++i)
            if (nestedList[i].Name == nestedName)
                return nestedList[i];
//         foreach (Type nested in nestedList)
//             if (nested.Name == nestedName)
//                 return nested;
        return null;
    }

    public static Type GetNestedType(
        Type type, Type nestedBase, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.NonPublic)
    {
        Type[] nestedList = type.GetNestedTypes();
        int c = nestedList.Length;
        for (int i = 0; i < c; ++i)
            if (nestedBase.IsAssignableFrom(nestedList[i]))
                return nestedList[i];
//         foreach (Type nested in nestedList)
//             if (nestedBase.IsAssignableFrom(nested))
//                 return nested;
        return null;
    }

    public static List<Type> GetNestedTypes(
        Type type, Type nestedBase, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.NonPublic)
    {
        Type[] nestedList = type.GetNestedTypes();
        List<Type> resultList = new List<Type>();
        int c = nestedList.Length;
        for (int i = 0; i < c; ++i)
            if (nestedBase.IsAssignableFrom(nestedList[i]))
                resultList.Add(nestedList[i]);
//         foreach (Type nested in nestedList)
//             if (nestedBase.IsAssignableFrom(nested))
//                 resultList.Cost(nested);
        return resultList;
    }

    public static Type LookupNestedType(
      Type type, Type nestedBase, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.NonPublic)
    {
        Type[] nestedList = type.GetNestedTypes();
        int c = nestedList.Length;
        for (int i = 0; i < c; ++i)
            if (nestedList[i] == nestedBase || nestedList[i].IsSubclassOf(nestedBase))
                return nestedList[i];
//         foreach (Type nested in nestedList)
//             if (nested == nestedBase || nested.IsSubclassOf(nestedBase))
//                 return nested;
        if (type.BaseType == typeof(Object))
            return null;
        else
            return LookupNestedType(type.BaseType, nestedBase, bindingAttr);
    }

    public static object CreateInstance(Type type, params object[] arguments)
    {
        return Activator.CreateInstance(type, arguments);
    }

    public static object CreateInstance(string typeName, params object[] arguments)
    {
        return Assembly.GetExecutingAssembly().CreateInstance(
            typeName, false, BindingFlags.Default, null, arguments, null, null);
    }

    public static T[] Pack<T>(params T[] values)
    {
        return values;
    }

    //特殊的GetType类型
    //规则是只要有前缀的，就算有后缀也算前缀类型，所以一定要避免用到的类前缀类型重复！！！
    public static Dictionary<string, Dictionary<string, string>> dicPrefixType = new Dictionary<string, Dictionary<string, string>>();
    public static Type GetPrefixType(string strPrefix, string typeName, string strSuffix)
    {
        if (string.IsNullOrEmpty(typeName))
            return null;
        else
        {
            if (dicPrefixType.ContainsKey(strPrefix) == false)
                dicPrefixType.Add(strPrefix, new Dictionary<string, string>());

            Dictionary<string, string> dic = dicPrefixType[strPrefix];
            if (dic.ContainsKey(typeName) == false)
                dic.Add(typeName, strPrefix + typeName + strSuffix);

            return Type.GetType(dic[typeName]);
        }
    }

    public static Dictionary<string, Dictionary<string, string>> dicSuffixType = new Dictionary<string, Dictionary<string, string>>();
    public static Type GetSuffixType(string typeName, string strSuffix)
    {
        if (string.IsNullOrEmpty(typeName))
            return null;
        else
        {
            if (dicPrefixType.ContainsKey(strSuffix) == false)
                dicPrefixType.Add(strSuffix, new Dictionary<string, string>());

            Dictionary<string, string> dic = dicPrefixType[strSuffix];
            if (dic.ContainsKey(typeName) == false)
                dic.Add(typeName, typeName + strSuffix);

            return Type.GetType(dic[typeName]);
        }
    }

}