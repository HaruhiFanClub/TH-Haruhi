

using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public static class Formatter
{
    static class Token
    {
        internal const char listStart = '[';
        internal const char listEnd = ']';
        internal const char stringScope = '"';
        internal const char stringEscape = '\\';
        internal const char dictionaryStart = '{';
        internal const char dictionaryEnd = '}';
        internal const char fragmentSeparator = ',';
        internal const char dictionarySeparator = '=';
    }

    public static T ToObject<T>(string fmtstr)
    {
        return (T)ToObject(typeof(T), fmtstr);
    }

    public static Object ToObject(Type type, string fmtstr)
    {
        Object fmtObject = Format(fmtstr);
        return fmtObject != null ? ToObject(type, fmtObject) : null;
    }

    static Object ToObject(Type type, Object fmtObject)
    {
        Object _object = null;

        if (type.IsArray)
        {
            List<Object> list = fmtObject as List<Object>;
            if (list != null)
            {
                int count = list.Count;
                Array _array = Activator.CreateInstance(type, count) as Array;
                Type elementType = type.GetElementType();
                for (int i = 0; i < count; ++i)
                    _array.SetValue(ToObject(elementType, list[i]), i);
                _object = _array;
            }
        }
        else if (typeof(IList).IsAssignableFrom(type))
        {
            List<Object> list = fmtObject as List<Object>;
            if (list != null)
            {
                int count = list.Count;
                IList _list = Activator.CreateInstance(type) as IList;
                Type elementType =
                    type.IsGenericType ? type.GetGenericArguments()[0] : typeof(Object);
                for (int i = 0; i < count; ++i)
                {
                    _list.Add(ToObject(elementType, list[i]));
                }
                _object = _list;
            }
        }
        else if (typeof(IDictionary).IsAssignableFrom(type))
        {
            Dictionary<string, Object> dict = fmtObject as Dictionary<string, Object>;
            if (dict != null)
            {
                IDictionary _dict = Activator.CreateInstance(type) as IDictionary;
                if (_dict != null && dict.Count > 0)
                {
                    Type keyType = typeof(Object);
                    Type valueType = typeof(Object);

                    if (type.IsGenericType)
                    {
                        Type[] kvType = type.GetGenericArguments();
                        if (kvType.Length >= 2)
                        {
                            keyType = kvType[0];
                            valueType = kvType[1];
                        }
                    }

                    Dictionary<string, Object>.Enumerator e = dict.GetEnumerator();
                    while (e.MoveNext())
                    {
                        Object key = ConvertType(e.Current.Key, keyType);
                        if (key != null)
                            _dict.Add(key, ToObject(valueType, e.Current.Value));
                    }

                }
                _object = _dict;
            }
        }
        else if (type == typeof(string))
        {
            _object = fmtObject as string;
        }
        else if (type.IsEnum)
        {
            string str = fmtObject as string;
            if (!string.IsNullOrEmpty(str))
            {
                _object = ConvertType(fmtObject, type);
            }
        }
        else if (type.IsClass || (type.IsValueType && !type.IsPrimitive))
        {
            Dictionary<string, Object> dict = fmtObject as Dictionary<string, Object>;
            if (dict != null)
            {
                _object = Activator.CreateInstance(type);
                if (_object != null && dict.Count > 0)
                    AssignObject(_object, dict);
            }
        }
        else
        {
            _object = ConvertType(fmtObject, type);
        }
        return _object;
    }

    public static void AssignObject(Object _object, string fmtstr, Type type = null)
    {
        Dictionary<string, Object> dict = Format(fmtstr) as Dictionary<string, Object>;
        if (dict != null && dict.Count > 0)
            AssignObject(_object, dict, type);
    }

    public static Object ConvertType(Object objValue, Type type)
    {
        Object objRet = null;
        try
        {
            objRet = type.IsEnum ? Enum.Parse(type, objValue.ToString(), true) : Convert.ChangeType(objValue, type);
        }
        catch (Exception exp)
        {
            Debug.LogError(string.Format("objValue:{0} type:{1} enum:{2} <=> {3} {4}", objValue.ToString(), type, type.IsEnum, exp.Message, exp.StackTrace));
        }

        return objRet;
    }

    public static void AssignObject(Object _object, FieldInfo field, string fmtstr, Type type = null)
    {
        if (type == null)
            type = field.FieldType;
        Object value = null;

        if (type.IsPrimitive)
        {
            value = ConvertType(fmtstr, type);
        }
        else if (type == typeof(string))
        {
            value = fmtstr;
        }
        else if (type.IsEnum)
        {
            if (!string.IsNullOrEmpty(fmtstr))
            {
                value = ConvertType(fmtstr, type);
            }
        }
        else
            value = ToObject(type, fmtstr);

        if (value != null)
            field.SetValue(_object, value);
    }

    static void AssignObject(Object _object, Dictionary<string, Object> dict, Type type = null)
    {
        if (type == null)
            type = _object.GetType();
        bool isObject = type.IsClass || (type.IsValueType && !type.IsPrimitive);
        if (!isObject ||
            !type.IsAssignableFrom(_object.GetType()) ||
            type == typeof(string) ||
            typeof(IList).IsAssignableFrom(type) ||
            typeof(IDictionary).IsAssignableFrom(type))
            throw new Exception("type error");

        FieldInfo[] fieldList =
            type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        int c = fieldList.Length;
        for (int i = 0; i < c; ++i)
        {
            Object value = null;
            dict.TryGetValue(fieldList[i].Name, out value);
            if (value != null)
                fieldList[i].SetValue(_object, ToObject(fieldList[i].FieldType, value));
        }

//             foreach (FieldInfo field in fieldList)
//             {
//                 Object value = null;
//                 dict.TryGetValue(field.Name, out value);
//                 if (value != null)
//                     field.SetValue(_object, ToObject(field.FieldType, value));
//             }

        //PropertyInfo[] properyList = 
        //    type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        //foreach (PropertyInfo property in properyList)
        //{
        //    MethodInfo setMothod = property.GetSetMethod(true);
        //    if (setMothod != null)
        //    {
        //        Object value = null;
        //        dict.TryGetValue(property.Name, out value);
        //        if (value != null)
        //            setMothod.Invoke(_object, new Object[] {ToObject(property.PropertyType, value)});    
        //    }
        //}
    }

    public static Object Format(string fmtstr)
    {
        int end;
        return Format(fmtstr, 0, out end);
    }

    static Object Format(string fmtstr, int begin, out int end)
    {
        int length = fmtstr.Length;
        while (begin < length && char.IsWhiteSpace(fmtstr, begin))
            ++begin;
        end = begin;
        if (begin >= length)
            return null;
        Object _object = null;

        switch (fmtstr[begin])
        {
            case Token.dictionaryStart :
                {
                    Dictionary<string, Object> dict = new Dictionary<string, object>();
                    if (++end < length)
                        while (true)
                        {
                            begin = end;
                            if ((end = fmtstr.IndexOf(Token.dictionarySeparator, end)) == -1)
                                end = length;
                            else
                                dict[fmtstr.Substring(begin, end - begin).Trim()] = Format(fmtstr, ++end, out end);
                            while (end < length && char.IsWhiteSpace(fmtstr, end))
                                ++end;
                            if (end >= length || fmtstr[end] == Token.dictionaryEnd)
                                break;
                        }
                    if (end < length && fmtstr[end] == Token.dictionaryEnd)
                        ++end;
                    _object = dict;
                }
                break;
            case Token.listStart :
                {
                    List<Object> list = new List<Object>();
                    if (++end < length)
                        while (true)
                        {
                            if (fmtstr[end] == Token.listEnd && end == length - 1)
                                break;
                            begin = end;
                            list.Add(Format(fmtstr, end, out end));
                            while (end < length && char.IsWhiteSpace(fmtstr, end))
                                ++end;
                            if (end >= length || fmtstr[end] == Token.listEnd)
                                break;
                        }
                    if (end < length && fmtstr[end] == Token.listEnd)
                        ++end;
                    _object = list;
                }
                break;
            case Token.stringScope :
                {
                    StringBuilder strbuild = new StringBuilder();
                    while (++end < length)
                    {
                        char c = fmtstr[end];
                        if (c == Token.stringEscape)
                        {
                            if (++end < length)
                            {
                                if (fmtstr[end] == Token.stringScope)
                                    strbuild.Append(Token.stringScope);
                                else
                                {
                                    strbuild.Append(c);
                                    strbuild.Append(fmtstr[end]);
                                }
                            }
                            else
                            {
                                strbuild.Append(c);
                                break;
                            }
                        }
                        else if (c == Token.stringScope)
                            break;
                        else
                            strbuild.Append(c);
                    }
                    if (end < length && fmtstr[end] == Token.stringScope)
                        ++end;
                    _object = strbuild.ToString();
                }
                break;
            default :
                {
                    while (++end < length)
                    {
                        char c = fmtstr[end];
                        if (c == Token.listEnd ||
                            c == Token.dictionaryEnd ||
                            c == Token.fragmentSeparator)
                            break;
                    }
                    _object = fmtstr.Substring(begin, end - begin);
                }
                break;
        }

        while (end < length && char.IsWhiteSpace(fmtstr, end))
            ++end;
        if (end < length && fmtstr[end] == Token.fragmentSeparator)
            ++end;
        return _object;
    }

    public static void Serialize(TextWriter writer, System.Object _object)
    {

    }
}