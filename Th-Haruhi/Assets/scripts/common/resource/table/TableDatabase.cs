﻿

using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Diagnostics;

public static class TableDatabase
{
    static readonly char fieldDilimiter;
    static readonly BindingFlags fieldFlags;
    static readonly int mainKey;
    static string tableExtension;
    static Hashtable hashTables;

    static TableDatabase()
    {
        mainKey = 0;
        fieldDilimiter = '\t';
        fieldFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        tableExtension = ".sos";
        hashTables = Hashtable.Synchronized(new Hashtable());
    }

    public static Table LoadByString(Type type, string reader, string tableName, string md5 = "")
    {
        string textAll = reader;

        int lineIndex = 0;
        string sep = "\r\n";
        string text = string.Empty;
        while (true)
        {
            int n = textAll.IndexOf(sep, lineIndex, StringComparison.Ordinal);
            if (n == -1)
                break;
            int len = n - lineIndex;
            text = textAll.Substring(lineIndex, len);
            lineIndex = n + 2;
            if (string.IsNullOrEmpty(text) || text[0] != '#')
                break;
        }
        if (string.IsNullOrEmpty(text))
            return null;

        string[] fieldNames = text.Split(fieldDilimiter);
        int count = fieldNames.Length;
        if (count <= mainKey)
            return null;

        for (int i = 0; i < fieldNames.Length; ++i)
        {
            if (fieldNames[i].StartsWith("sz_") || fieldNames[i].StartsWith("js_"))
            {
                fieldNames[i] = fieldNames[i].Remove(0, 3);
            }
            else if (fieldNames[i].StartsWith("b_"))
            {
                fieldNames[i] = fieldNames[i].Remove(0, 2);
            }

            if (fieldNames[i].EndsWith("__s"))
            {
                fieldNames[i] = fieldNames[i].Remove(fieldNames[i].Length - 3, 3);
            }
        }

        FieldInfo[] fieldInfos = new FieldInfo[count];
        for (int i = 0; i < count; ++i)
        {
            fieldInfos[i] = type.GetField(fieldNames[i], fieldFlags);
        }

        FieldInfo keyField = fieldInfos[mainKey];
        if (keyField == null)
        {
            UnityEngine.Debug.LogError(string.Format("{0} mainkey 不存在!!!", tableName));
            return null;
        }

        Table table = new Table();
        while (true)
        {
            int n = textAll.IndexOf(sep, lineIndex, StringComparison.Ordinal);
            if (n == -1)
                break;
            int len = n - lineIndex;
            text = textAll.Substring(lineIndex, len);
            lineIndex = n + 2;
            if (text.Length <= 0 || text[0] == '#')
                continue;
            string[] valueStrs = text.Split(fieldDilimiter);
            if (valueStrs.Length <= 0 || string.IsNullOrEmpty(valueStrs[mainKey]))
                continue;

            object section = Activator.CreateInstance(type);

            for (int i = 0; i < valueStrs.Length && i < fieldInfos.Length; ++i)
            {
                string str = valueStrs[i];
                FieldInfo info = fieldInfos[i];
                if (info != null && !string.IsNullOrEmpty(str))
                {
                    str = str.Replace("\\n", "\n").Replace("\\t", "\t");
                    SetField(section, info, str);
                }
            }

            object key = keyField.GetValue(section);
            if (key != null)
            {
                //若已有key存在，则报错
#if UNITY_EDITOR
                if (table.GetSection(key) != null)
                    UnityEngine.Debug.LogError("Table Key is not the only! {\"tableName\": " + tableName + ",  \"key\": " + key + "}" + "");
#endif
                table.SetSection(key, section);
            }
        }
        table.type = type;
        table.md5 = md5 != "" ? md5 : CalcMD5CRC(reader);
        return table;
    }

    public static Table Load(string tableName)
    {
        Object cacheObj = hashTables[tableName];
        Table table = null;

        if (cacheObj != null)
        {
            table = (Table)cacheObj;
            return table;
        }
        try
        {
            UnityEngine.Debug.LogError("读取配置失败，配置是否未提前缓存?   " + tableName);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("GetTextResource Exception failed: " + tableName + " error: " + e);
        }
        return table;
    }

    public static Table CacheTable(Type type, string tableName, string code)
    {
        string tabPath = tableName + tableExtension;
        if (code != null)
        {
            code = RemoveComment(code);
            var table = LoadByString(type, code, tableName);
            hashTables[tableName] = table;
            return table;
        }
        else
        {
            UnityEngine.Debug.LogError("GetTextResource failed:" + tabPath);
        }
        return null;
    }


    public static TableT<T> Load<T>(string tableName) where T : class
    {
        Table table = Load(tableName);
        var t = table ? new TableT<T>(table) : null; 
        if(t == null)
        {
            UnityEngine.Debug.LogError("配置表不存在:" + tableName);
        }
        return t;
    }

    public static void UnLoad(string tableName)
    {
        hashTables.Remove(tableName);
    }

    static void SetField(object section, FieldInfo fieldInfo, string valueStr)
    {
        Type type = fieldInfo.FieldType;
        if (type.IsArray || typeof(IList).IsAssignableFrom(type))
        {
            valueStr = TrimSpace(valueStr, "[", "]");
        }
        Formatter.AssignObject(section, fieldInfo, valueStr);
    }

    private static string TrimSpace(string valueStr, string splitCh1, string splitCh2)
    {
        int i = 0;
        int length = valueStr.Length;
        while (i < length)
        {
            if (!char.IsWhiteSpace(valueStr, i))
                break;
            ++i;
        }
        if (i >= length || valueStr[i] != splitCh1[0])
            valueStr = splitCh1 + valueStr + splitCh2;
        return valueStr;
    }

    //去tab表注释
    public static string RemoveComment(string txtTab, bool remove__s = false)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(txtTab);
        string text = txtTab;
        if (buffer.Length >= 3 && buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
            text = Encoding.UTF8.GetString(buffer, 3, buffer.Length - 3);

        StringBuilder sb = new StringBuilder();
        List<string> cols = new List<string>();
        foreach (var line in text.Split(new string[] { "\r\n" }, StringSplitOptions.None))
        {
            if (line.StartsWith("#"))
                continue;
            if (line == "")
                continue;

            string[] list = line.Split(new string[] { "\t" }, StringSplitOptions.None);
            if (cols.Count == 0)
                cols.AddRange(list);

            if (list.Length != cols.Count)
            {
                UnityEngine.Debug.LogError("wrong line:" + line + " " + list.Length + "!=" + cols.Count);
            }

            for (int i = 0; i < cols.Count; i++)
            {
                string colname = cols[i];
                if (colname != "" && !(remove__s && colname.EndsWith("__s")))
                {
                    sb.Append(list[i]);
                    sb.Append("\t");
                }
            }
            sb.Append("\r\n");
        }

        //excel
        sb.Replace("\"", "");
        return sb.ToString();
    }

    public static string CalcMD5CRC(string input)
    {
        // step 1, calculate MD5 hash from input  
        var md5 = MD5.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = md5.ComputeHash(inputBytes);

        // step 2, convert byte array to hex string  
        var sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("X2"));
        }
        return sb.ToString();
    }

}