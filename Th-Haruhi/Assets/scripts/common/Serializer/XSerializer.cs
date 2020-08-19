

using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using Object = System.Object;

abstract public class XSerializer
{
    internal class FieldComparer : IEqualityComparer<FieldInfo>
    {
        public bool Equals(
            FieldInfo fieldLeft, FieldInfo fieldRight)
        {
            return fieldLeft.FieldHandle == fieldRight.FieldHandle;
        }

        public int GetHashCode(FieldInfo field)
        {
            return field.FieldHandle.GetHashCode();
        }
    }

    abstract public byte[] ReadBytes(int readLen);
    abstract public int ReadBytes(byte[] bytes, int readLen = -1);

    abstract public int WriteBytes(byte[] bytes, int offset = 0, int writeLen = -1);
    abstract public byte[] GetBuffer(out Fragment[] _fragments);

    abstract public int available { get; }
    abstract public int readPosition { get; set; }
    abstract public int writePosition { get; set; }
    abstract public bool complete { get; protected set; }

    public Encoding encoding;

    public XSerializer()
    {
        encoding = Encoding.UTF8;
    }

    public T Read<T>(bool useZero = false)
    {

        return (T)Read(typeof(T));
    }

    private byte[] _uShortBytes;
    private byte[] _uIntBytes;
    private byte[] _byteBytes;
    public ushort ReadUShort()
    {
        if (_uShortBytes == null) _uShortBytes = new byte[sizeof(ushort)];
        ReadBytes(_uShortBytes);
        return BitConverter.ToUInt16(_uShortBytes, 0);
    }

    public uint ReadUInt()
    {
        if (_uIntBytes == null) _uIntBytes = new byte[sizeof(uint)];
        ReadBytes(_uIntBytes);
        return BitConverter.ToUInt32(_uIntBytes, 0);
    }

    public byte Readbyte()
    {
        if (_byteBytes == null) _byteBytes = new byte[sizeof(byte)];
        ReadBytes(_byteBytes);
        return _byteBytes[0];
    }

    public Object Read(Type type)
    {
        if (type == typeof(string))
        {
            ushort byteLen = (ushort)Read(typeof(ushort));
            if (byteLen > 0)
            {
                byte[] bytes = new byte[byteLen];
                ReadBytes(bytes);
                return encoding.GetString(bytes, 0, byteLen);
            }
            else
                return string.Empty;
        }
        else if (type == typeof(bool))
        {
            byte[] bytes = new byte[sizeof(bool)];
            ReadBytes(bytes);
            return BitConverter.ToBoolean(bytes, 0);
        }
        else if (type == typeof(byte))
        {
            return Readbyte();
        }
        else if (type == typeof(sbyte))
        {
            byte[] bytes = new byte[sizeof(sbyte)];
            ReadBytes(bytes);
            return (sbyte)bytes[0];
        }
        else if (type == typeof(char))
        {
            byte[] bytes = new byte[sizeof(char)];
            ReadBytes(bytes);
            return BitConverter.ToChar(bytes, 0);
        }
        else if (type == typeof(short))
        {
            byte[] bytes = new byte[sizeof(short)];
            ReadBytes(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }
        else if (type == typeof(ushort))
        {
            return ReadUShort();
        }
        else if (type == typeof(int))
        {
            byte[] bytes = new byte[sizeof(int)];
            ReadBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
        else if (type == typeof(uint))
        {
            return ReadUInt();
        }
        else if (type == typeof(long))
        {
            byte[] bytes = new byte[sizeof(long)];
            ReadBytes(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }
        else if (type == typeof(ulong))
        {
            byte[] bytes = new byte[sizeof(ulong)];
            ReadBytes(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }
        else if (type == typeof(float))
        {
            byte[] bytes = new byte[sizeof(float)];
            ReadBytes(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }
        else if (type == typeof(double))
        {
            byte[] bytes = new byte[sizeof(double)];
            ReadBytes(bytes);
            return BitConverter.ToDouble(bytes, 0);
        }
        else if (type.IsEnum)
        {
            return Enum.ToObject(type, Read(Enum.GetUnderlyingType(type)));
        }
        else if (type.GetCustomAttributes(typeof(JsonObjectAttribute), false).Length > 0)
        {
            return JsonMapper.ToObject(type, Read<string>());
        }
        else if (type == typeof(JsonData))
        {
            return JsonMapper.ToObject(Read<string>());
        }
        else if(type == typeof(System.DateTime))
        {
            long tick = Read<long>();
            return new System.DateTime(tick);
        }
        else if(type == typeof(byte[]))
        {
            int len = Read<int>();
            byte[] data = new byte[len];
            ReadBytes(data);
            return data;
        }
        else if(type.IsArray)
        {
            ushort arryLen = (ushort)Read(typeof(ushort));
            //Array _array = Activator.CreateInstance(type, arryLen) as Array;
            Type elementType = type.GetElementType();
            Array _array = Array.CreateInstance(elementType, arryLen);
            if(elementType == typeof(byte))
                ReadBytes((byte[])_array);
            else
                for(ushort i = 0; i < arryLen; ++i)
                    _array.SetValue(Read(elementType), i);
            return _array;
        }
        else if(typeof(IList).IsAssignableFrom(type))
        {
            IList _list = Activator.CreateInstance(type) as IList;
            ushort listLen = (ushort)Read(typeof(ushort));
            Type elementType =
                type.IsGenericType ? type.GetGenericArguments()[0] : typeof(object);
            for(ushort i = 0; i < listLen; ++i)
                _list.Add(Read(elementType));
            return _list;
        }
        else if(typeof(IDictionary).IsAssignableFrom(type))
        {
            IDictionary _dict = Activator.CreateInstance(type) as IDictionary;
            Type keyType = typeof(object);
            Type valueType = typeof(object);
            if(type.IsGenericType)
            {
                Type[] kvType = type.GetGenericArguments();
                if(kvType.Length >= 2)
                {
                    keyType = kvType[0];
                    valueType = kvType[1];
                }
            }
            ushort dictLen = (ushort)Read(typeof(ushort));
            for(ushort i = 0; i < dictLen; ++i)
                _dict.Add(Read(keyType), Read(valueType));
            return _dict;
        }
        else if (typeof(UnityEngine.Vector3).IsAssignableFrom(type))
        {
            UnityEngine.Vector3 v3 = new UnityEngine.Vector3();
            v3.x = (float)Read(typeof(float));
            v3.y = (float)Read(typeof(float));
            v3.z = (float)Read(typeof(float));
            return v3;
        }
        else if(type.IsClass || (type.IsValueType && !type.IsPrimitive))
        {
            Type _type = type;
            Stack<Type> typeStack = new Stack<Type>();
            while(_type != typeof(Object))
            {
                typeStack.Push(_type);
                _type = _type.BaseType;
            }
            HashSet<FieldInfo> fieldSet = new HashSet<FieldInfo>(new FieldComparer());
            while(typeStack.Count > 0)
            {
                _type = typeStack.Pop();
                FieldInfo[] fieldList =
                    _type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach(FieldInfo field in fieldList)
                    fieldSet.Add(field);
            }
            Object _object = Activator.CreateInstance(type);
            foreach(FieldInfo field in fieldSet)
            {
                if(field.GetCustomAttributes(typeof(JsonObjectAttribute), false).Length > 0)
                    field.SetValue(_object, JsonMapper.ToObject(field.FieldType, Read<string>()));
                else
                    field.SetValue(_object, Read(field.FieldType));
            }
            return _object;
        }
        else
            throw new Exception("invalid type");
    }

    public void Read(Object _object, Type type)
    {
        if (type == null)
            type = _object.GetType();
        if (!type.IsClass ||
            !type.IsAssignableFrom(_object.GetType()) ||
            type == typeof(string) ||
            typeof(IList).IsAssignableFrom(type) ||
            typeof(IDictionary).IsAssignableFrom(type))
            throw new Exception("type error");

        Type _type = type;
        Stack<Type> typeStack = new Stack<Type>();
        while (_type != typeof(Object))
        {
            typeStack.Push(_type);
            _type = _type.BaseType;
        }
        HashSet<FieldInfo> fieldSet = new HashSet<FieldInfo>(new FieldComparer());
        while (typeStack.Count > 0)
        {
            _type = typeStack.Pop();
            FieldInfo[] fieldList =
                _type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo field in fieldList)
                fieldSet.Add(field);
        }
        foreach (FieldInfo field in fieldSet)
            field.SetValue(_object, Read(field.FieldType));
    }

    public byte[] PeekBytes(int peekLen)
    {
        int capture = readPosition;
        byte[] bytes = ReadBytes(peekLen);
        readPosition = capture;
        return bytes;
    }

    public int PeekBytes(byte[] bytes, int peekLen = -1)
    {
        int capture = readPosition;
        peekLen = ReadBytes(bytes, peekLen);
        readPosition = capture;
        return peekLen;
    }

    public int PeekBytes(byte[] bytes, out int position, int peekLen = -1)
    {
        int capture = readPosition;
        peekLen = ReadBytes(bytes, peekLen);
        position = readPosition;
        readPosition = capture;
        return peekLen;
    }

    public T Peek<T>() 
    {
        return (T)Peek(typeof(T));
    }

    public T Peek<T>(out int position)
    {
        return (T)Peek(typeof(T), out position);
    }

    public Object Peek(Type type)
    {
        int capture = readPosition;
        Object _object = Read(type);
        readPosition = capture;
        return _object;
    }

    public Object Peek(Type type, out int position)
    {
        int capture = readPosition;
        Object _object = Read(type);
        position = readPosition;
        readPosition = capture;
        return _object;
    }

    public void Peek(Object _object, Type type)
    {
        int capture = readPosition;
        Read(_object, type);
        readPosition = capture;
    }

    public void Peek(Object _object, out int position, Type type)
    {
        int capture = readPosition;
        Read(_object, type);
        position = readPosition;
        readPosition = capture;
    }

    public void Write<T>(T _object, bool useZero)
    {
        WriteObject(_object);
    }


    public void WriteObject(Object _object)
    {
        Type type = _object.GetType();

        if (type.IsEnum)
            type = Enum.GetUnderlyingType(type);
        
        if (type.IsValueType)
        {            
            if (type == typeof(int))
            {
                Write((int) _object);
                return;
            }
            if (type == typeof(uint))
            {
                Write((uint) _object);
                return;
            }
            
            if (type == typeof(float))
            {
                Write((float) _object);
                return;
            }
            if (type == typeof(ulong))
            {
                Write((ulong) _object);
                return;
            }
            if (type == typeof(ushort))
            {
                Write((ushort) _object);
                return;
            }
            if (type == typeof(short))
            {
                Write((short) _object);
                return;
            }
            if (type == typeof(long))
            {
                Write((long) _object);
                return;
            }
            if (type == typeof(byte))
            {
                Write((byte) _object);
                return;
            }
            if (type == typeof(char))
            {
                Write((char) _object);
                return;
            }
            if (type == typeof(sbyte))
            {
                Write((sbyte) _object);
                return;
            }
            if (type == typeof(double))
            {
                Write((double) _object);
                return;
            }
            if (type == typeof(bool))
            {
                Write((bool) _object);
                return;
            }
            if (type == typeof(UnityEngine.Vector3))
            {
                Write((UnityEngine.Vector3) _object);
                return;
            }
            if (type == typeof(DateTime))
            {
                Write((DateTime) _object);
                return;
            }
        }
        if (type == typeof(byte[]))
        {
            Write(((byte[])_object).Length);
            WriteBytes((byte[]) _object);
            return;
        }
        
        if (type == typeof(string))
        {
            Write((string) _object);
            return;
        }
        
        
        if (type.IsArray)
        {
            Array _array = (Array) _object;
            Type elementType = type.GetElementType();
            ushort arryLen = (ushort) _array.Length;
            Write(arryLen);
            if (elementType == typeof(byte))
                WriteBytes((byte[]) _array);
            else
                for (ushort i = 0; i < arryLen; ++i)
                    WriteObject(_array.GetValue(i));
        }
        else if (Common.IsType(typeof(XSerializer), type))
        {
            Write(_object as XSerializer);
        }
        else if (typeof(IList).IsAssignableFrom(type))
        {
            IList _list = _object as IList;
            ushort listLen = (ushort) _list.Count;
            Write(listLen);
            for (ushort i = 0; i < listLen; ++i)
                WriteObject(_list[i]);
        }
        else if (typeof(IDictionary).IsAssignableFrom(type))
        {
            IDictionary dict = _object as IDictionary;
            IDictionaryEnumerator enumerator = dict.GetEnumerator();
            Write((ushort) dict.Count);
            while (enumerator.MoveNext())
            {
                WriteObject(enumerator.Key);
                WriteObject(enumerator.Value);
            }
        }
        else if (type == typeof(JsonData))
        {
            WriteObject(JsonMapper.ToJson(_object));
        }
        else if (type.IsClass || (type.IsValueType && !type.IsPrimitive))
        {
            //Debug.LogWarning("slow serial class:" + type.ToString());
            if (type.GetCustomAttributes(typeof(JsonObjectAttribute), false).Length <= 0)
            {
                Type _type = type;
                Stack<Type> typeStack = new Stack<Type>();
                while (_type != typeof(Object))
                {
                    typeStack.Push(_type);
                    _type = _type.BaseType;
                }
                HashSet<FieldInfo> fieldSet = new HashSet<FieldInfo>(new FieldComparer());
                while (typeStack.Count > 0)
                {
                    _type = typeStack.Pop();
                    FieldInfo[] fieldList =
                        _type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (FieldInfo field in fieldList)
                        fieldSet.Add(field);
                }

                foreach (FieldInfo field in fieldSet)
                {
                    if (field.GetValue(_object) == null)
                    {
                        Debug.LogError("FpsSerializer _object is null:"+ field);
                    }
                    WriteObject(field.GetValue(_object));
                }
            }
            else
                Write(JsonMapper.ToJson(_object));
        }
        else
            throw new Exception("invalid type" + type.ToString());

    }


    public void Write(UnityEngine.Vector3 value)
    {
        UnityEngine.Vector3 v3 = (UnityEngine.Vector3)value;
        Write(v3.x);
        Write(v3.y);
        Write(v3.z);
    }   
    
    public void Write(XSerializer value)
    {
        Fragment[] fragments;
        byte[] buffer = value.GetBuffer(out fragments);
        if (fragments != null)
            foreach (Fragment _framment in fragments)
                WriteBytes(buffer, _framment.begin, _framment.length);
    }   
    
    public void Write(System.DateTime value)
    {
        Write(((System.DateTime)value).Ticks);
    }    
    public void Write(string value)
    {
        byte[] bytes = encoding.GetBytes(value);
        Write((ushort)bytes.Length);
        WriteBytes(bytes);
    }    
    
    public void Write(bool value)
    {
        WriteBytes(BitConverter.GetBytes(value));
    }
    
    public void Write(long value)
    {
        WriteBytes(BitConverter.GetBytes(value));
    }

    public void Write(ulong value)
    {
        WriteBytes(BitConverter.GetBytes(value));
    }

    public void Write(int value)
    {
        WriteBytes(BitConverter.GetBytes(value));
    }

    public void Write(uint value)
    {
        WriteBytes(BitConverter.GetBytes(value));
    }
    
    public void Write(short value)
    {
        WriteBytes(BitConverter.GetBytes(value));
    }
    
    public void Write(ushort value)
    {
        WriteBytes(BitConverter.GetBytes(value));
    }
    
    public void Write(byte value)
    {
        WriteBytes(new byte[] { value });
    }
    
    public void Write(sbyte value)
    {
        WriteBytes(new byte[] { (byte)value });
    }
    
    public void Write(char value)
    {
        WriteBytes(BitConverter.GetBytes(value));
    }
    
    public void Write(float value)
    {
        WriteBytes(BitConverter.GetBytes(value));
    }
    
    public void Write(double value)
    {
        WriteBytes(BitConverter.GetBytes(value));
    }
    
    
    public static int SizeOf(Object _object, Encoding _encoding = null)
    {
        Type type = _object.GetType();
        if (_encoding == null)
            _encoding = Encoding.Unicode;
        if (type.IsArray)
        {
            Array array = (Array)_object;
            int arryLen = array.Length;
            int size = sizeof(ushort);
            for (int i = 0; i < arryLen; ++i)
                size += SizeOf(array.GetValue(i));
            return size;
        }
        else if (typeof(IList).IsAssignableFrom(type))
        {
            IList list = _object as IList;
            int listLen = list.Count;
            int size = sizeof(ushort);
            for (ushort i = 0; i < listLen; ++i)
                size += SizeOf(list[i]);
            return size;
        }
        else if (typeof(IDictionary).IsAssignableFrom(type))
        {
            IDictionaryEnumerator enumerator = ((IDictionary)_object).GetEnumerator();
            int size = sizeof(ushort);
            while (enumerator.MoveNext())
            {
                size += SizeOf(enumerator.Key);
                size += SizeOf(enumerator.Value);
            }
            return size;
        }
//         else if (type.GetInterface("System.Collections.Generic.ICollection`1") != null)
//         {
//             MethodInfo enumMethod = type.GetMethod("GetEnumerator");
//             int size = sizeof(ushort);
//             if (enumMethod == null)
//                 throw new Exception("invalid type");
//             IEnumerator enumerator = (IEnumerator)enumMethod.Invoke(_object, null);
//             while (enumerator.MoveNext())
//                 size += SizeOf(enumerator.Current);
//             return size;
//         }
        else if (type == typeof(string))
        {
            byte[] bytes = _encoding.GetBytes((string)_object);
            return sizeof(ushort) + bytes.Length + 1;
        }
        else if (type.IsClass)
        {
            int size = 0;
            FieldInfo[] fieldList =
                type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo field in fieldList)
                size += SizeOf(field.GetValue(_object));
            return size;
        }
        else if (type == typeof(bool))
            return sizeof(bool);
        else if (type == typeof(byte))
            return sizeof(bool);
        else if (type == typeof(sbyte))
            return sizeof(bool);
        else if (type == typeof(char))
            return sizeof(bool);
        else if (type == typeof(short))
            return sizeof(bool);
        else if (type == typeof(ushort))
            return sizeof(bool);
        else if (type == typeof(int))
            return sizeof(bool);
        else if (type == typeof(uint))
            return sizeof(bool);
        else if (type == typeof(long))
            return sizeof(bool);
        else if (type == typeof(ulong))
            return sizeof(bool);
        else if (type == typeof(float))
            return sizeof(bool);
        else if (type == typeof(double))
            return sizeof(bool);
        else
            throw new Exception("invalid type");
    }
}