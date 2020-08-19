
using System;

public class BufferWriter : XSerializer
{
    protected int end;
    protected byte[] buffer;
	public uint encodeType;

	public void Clear()
	{
		writePosition = 0;
		encodeType = 0;
	}

    public override int available
    {
        get
        {
            return end;
        }
    }

    public int capacity
    {
        get
        {
            return buffer != null ? buffer.Length : 0;
        }
    }

    public override int readPosition
    {
        get
        {
            throw new Exception("writer can not do read");
        }
        set
        {
            throw new Exception("writer can not do read");
        }
    }

    public override int writePosition
    {
        get
        {
            return end;
        }
        set
        {
            end = value;
        }
    }

    public override bool complete
    {
        get
        {
            return true;
        }
        protected set
        {
        }
    }

    public override byte[] ReadBytes(int readLen)
    {
        throw new Exception("writer can not do read");
    }

    public override int ReadBytes(byte[] bytes, int readLen = -1)
    {
        throw new Exception("writer can not do read");
    }

    public override int WriteBytes(byte[] bytes, int offset = 0, int writeLen = -1)
    {
        int byteLen = bytes.Length - offset;
        if (writeLen < 0 || writeLen > byteLen)
            writeLen = byteLen;
        if (writeLen <= 0)
            return 0;
        if (writeLen > capacity - end)
            Reserve(available + writeLen);
        Buffer.BlockCopy(bytes, offset, buffer, end, writeLen);
        end += writeLen;
        return writeLen;
	}
	
	public bool OverrideBytes(System.Object _object, int offset, int writeLen)
	{
		Type type = _object.GetType();
		if (type == typeof(bool))
			Buffer.BlockCopy(BitConverter.GetBytes((bool)_object), 0, buffer, offset, writeLen);
		else if (type == typeof(byte) || type == typeof(sbyte))
			Buffer.BlockCopy(new byte[] { (byte)_object }, 0, buffer, offset, writeLen);
		else if (type == typeof(char))
			Buffer.BlockCopy(BitConverter.GetBytes((char)_object), 0, buffer, offset, writeLen);
		else if (type == typeof(short))
			Buffer.BlockCopy(BitConverter.GetBytes((short)_object), 0, buffer, offset, writeLen);
		else if (type == typeof(ushort))
			Buffer.BlockCopy(BitConverter.GetBytes((ushort)_object), 0, buffer, offset, writeLen);
		else if (type == typeof(int))
			Buffer.BlockCopy(BitConverter.GetBytes((int)_object), 0, buffer, offset, writeLen);
		else if (type == typeof(uint))
			Buffer.BlockCopy(BitConverter.GetBytes((uint)_object), 0, buffer, offset, writeLen);
		else if (type == typeof(long))
			Buffer.BlockCopy(BitConverter.GetBytes((long)_object), 0, buffer, offset, writeLen);
		else if (type == typeof(ulong))
			Buffer.BlockCopy(BitConverter.GetBytes((ulong)_object), 0, buffer, offset, writeLen);
		else if (type == typeof(float))
			Buffer.BlockCopy(BitConverter.GetBytes((float)_object), 0, buffer, offset, writeLen);
		else if (type == typeof(double))
			Buffer.BlockCopy(BitConverter.GetBytes((double)_object), 0, buffer, offset, writeLen);
		else 
		{
			throw new Exception("invalid type");
		}
		return true;
	}
	
	public void Reserve(int _length)
	{
		int _capacity = capacity + capacity / 2;
		if (_capacity < _length)
			_capacity = _length;
		byte[] _buffer = new byte[_capacity];
        if (buffer != null)
            Buffer.BlockCopy(buffer, 0, _buffer, 0, end);
        buffer = _buffer;
    }

    public override byte[] GetBuffer(out Fragment[] _fragments)
    {
        _fragments = new Fragment[] { new Fragment(0, end) };
        return buffer;
	}
	
	public byte[] GetData()
	{
		return buffer;
	}

}

