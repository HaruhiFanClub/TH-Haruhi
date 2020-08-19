

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CollectionUtility
{
    public static void Insert<T>(ICollection<T> dest, T[] src)
    {
        if (dest != null && src != null)
            foreach (T value in src)
                dest.Add(value);
    }

    public static void Insert<T>(ICollection<T> dest, ICollection<T> src)
    {
        if (dest != null && src != null)
            foreach (T value in src)
                dest.Add(value);
    }

    public static void Remove<T>(ICollection<T> dest, ICollection<T> src)
    {
        if (dest != null && src != null)
            foreach (T value in src)
                dest.Remove(value);
    }

    public static T[] Remove<T>(T[] array, params T[] values)
    {
        List<T> list = new List<T>();
        foreach (T t in array)
            if (Array.IndexOf<T>(values, t) == -1)
                list.Add(t);
        return list.ToArray();
    }

    public static HashSet<T> Except<T>(HashSet<T> first, HashSet<T> second)
    {
        HashSet<T> dest = new HashSet<T>();
        foreach (T t in first)
            if (!second.Contains(t))
                dest.Add(t);
        return dest;
    }

    public static void Swap(IList dest, int left, int right)
    {
        System.Object temp = dest[left];
        dest[left] = dest[right];
        dest[right] = temp;
    }

    public static bool Contains<T>(T[] array, T value)
    {
        return Array.IndexOf<T>(array, value) != -1;
    }

    public static bool Contains(IList list, object value)
    {
        return list.Contains(value);
    }

    public static bool NullOrEmpty(IList list)
    {
        return list == null || list.Count <= 0;
    }

    public static bool NullOrEmpty(ICollection collection)
    {
        return collection == null || collection.Count <= 0;
    }

    public static T[] Resize<T>(T[] src, int length) where T : new()
    {
        T[] array = new T[length];
        int copyLen = length;
        if (copyLen > src.Length)
            copyLen = src.Length;
        for (int i = 0; i < copyLen; ++i)
            array[i] = src[i];
        return array;
    }

    public static T Random<T>(T[] _array)
    {
        return _array[UnityEngine.Random.Range(0, _array.Length)];
    }

    public static bool ArrayEqual<T>(T[] x, T[] y)
    {
        if (x == null && y == null)
            return true;
        if (x == null && y != null)
            return false;
        if (x != null && y == null)
            return false;
        if (x.Length != y.Length)
            return false;
        for (int i = 0; i < x.Length; ++i)
            if (!x[i].Equals(y[i]))
                return false;
        return true;
    }

    public static int ArrayHash<T>(T[] x)
    {
        if (x == null)
            return 0;
        int hash = 0;
        for (int i = 0; i < x.Length; ++i)
            hash ^= x[i].GetHashCode();
        return hash;
    }

    public static Dictionary<Key, Value> Copy<Key, Value>(Dictionary<Key, Value> dst, Dictionary<Key, Value> src)
    {
        dst.Clear();
        Dictionary<Key, Value>.Enumerator enumsrc = src.GetEnumerator();
        while (enumsrc.MoveNext())
            dst.Add(enumsrc.Current.Key, enumsrc.Current.Value);
        return dst;
    }

    public static Dictionary<Key, Value> Clone<Key, Value>(Dictionary<Key, Value> src)
    {
        Dictionary<Key, Value> dst = new Dictionary<Key, Value>();
        Dictionary<Key, Value>.Enumerator enumsrc = src.GetEnumerator();
        while (enumsrc.MoveNext())
            dst.Add(enumsrc.Current.Key, enumsrc.Current.Value);
        return dst;
    }

    public static void Different<T>(HashSet<T> src, HashSet<T> dst, HashSet<T> more, HashSet<T> less)
    {
        HashSet<T>.Enumerator enumset = dst.GetEnumerator();
        while (enumset.MoveNext())
        {
            T value = enumset.Current;
            if (!src.Contains(value))
                less.Add(value);
        }

        enumset = src.GetEnumerator();
        while (enumset.MoveNext())
        {
            T value = enumset.Current;
            if (!dst.Contains(value))
                more.Add(value);
        }
    }
}