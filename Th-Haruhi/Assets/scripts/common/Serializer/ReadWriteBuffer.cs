using System;

public class ReadWriteBuffer : XSerializer
{
    byte[] buffer;
    int begin;
    int end;
    int readRecord;
    int writeRecord;

    public override int readPosition
    {
        get
        {
            return begin;
        }
        set
        {
            begin = value;
        }
	}
	
	public int endPosition
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

    public override int writePosition
    {
        get
        {
            return available;
        }
        set
        {
            int _length = buffer.Length;
            if (value > _length)
                throw new Exception("invalid write position");
            if (value == _length)
                end = -1;
            else
            {
                int rigtLen = _length - begin;
                if (value < rigtLen)
                    end = begin + value;
                else
                    end = value - rigtLen;
            }
        }
    }

    public override int available
    {
        get
        {
            if (end == -1)
                return buffer.Length;
            else if (end >= begin)
                return end - begin;
            else
                return buffer.Length - begin + end;
        }
    }

    public int capacity
    {
        get
        {
            return buffer != null ? buffer.Length : 0;
        }
    }

    public override bool complete { get; protected set; }

    public ReadWriteBuffer()
    {
        end = 0;
        begin = 0;
        buffer = null;
    }

    public override byte[] ReadBytes(int readLen)
    {
        int _length = available;
        bool _complete = readLen <= _length;
        if (!_complete)
            readLen = _length;
        if (readLen <= 0)
            return null;
        byte[] bytes = new byte[readLen];
        ReadBytes(bytes);
        complete = _complete;
        return bytes;
    }

    public override int ReadBytes(byte[] bytes, int readLen = -1)
    {
        if (readLen < 0)
            if ((readLen = bytes.Length) <= 0)
                return 0;
        int _length = available;
        if (end == -1)
            end = begin;
        if (!(complete = readLen <= _length))
            readLen = _length;
        int copyLen = buffer.Length - begin;
        if (copyLen > readLen)
            copyLen = readLen;
        Buffer.BlockCopy(buffer, begin, bytes, 0, copyLen);
        if ((begin += copyLen) >= buffer.Length)
            begin = 0;
        int leavLen = readLen - copyLen;
        if (leavLen > 0)
        {
            Buffer.BlockCopy(buffer, begin, bytes, copyLen, leavLen);
            begin += leavLen;
        }
        readRecord += readLen;
        return readLen;
    }


    public override int WriteBytes(byte[] bytes, int offset = 0, int writeLen = -1)
    {
        int byteLen = bytes.Length - offset;
        if (writeLen < 0 || writeLen > byteLen)
            writeLen = byteLen;
        if (writeLen <= 0)
            return 0;
        byteLen = writeLen;
        if (byteLen > capacity - available)
            Reserve(available + byteLen);
        int copyLen = buffer.Length - end;
        if (copyLen > byteLen)
            copyLen = byteLen;
        Buffer.BlockCopy(bytes, offset, buffer, end, copyLen);
        if ((end += copyLen) >= buffer.Length)
            end = 0;
        if ((byteLen -= copyLen) > 0)
            Buffer.BlockCopy(bytes, offset + copyLen, buffer, end, byteLen);
        if ((end += byteLen) == begin)
            end = -1;
        writeRecord += writeLen;
        return writeLen;
    }

    public int SeekRead(int seekLen)
    {
        if (seekLen < 0)
            throw new Exception("can not reverse seek read");
        if (seekLen == 0)
            return 0;
        int _length = available;
        if (end == -1)
            end = begin;
        if (!(complete = seekLen <= _length))
            seekLen = _length;
        int moveLen = buffer.Length - begin;
        if (moveLen > seekLen)
            moveLen = seekLen;
        if ((begin += moveLen) >= buffer.Length)
            begin = 0;
        begin += seekLen - moveLen;
        return seekLen;
    }

    public override byte[] GetBuffer(out Fragment[] _fragments)
    {
        if (end == -1)
        {
            if (begin == 0)
                _fragments = new Fragment[] { new Fragment(begin, available) };
            else
                _fragments = new Fragment[] 
                { 
                    new Fragment(begin, buffer.Length - begin),
                    new Fragment(0, begin)
                };
        }
        else if (end > begin)
            _fragments = new Fragment[] { new Fragment(begin, end - begin) };
        else if (end < begin)
        {
            if (end > 0)
                _fragments = new Fragment[] 
                { 
                    new Fragment(begin, buffer.Length - begin),
                    new Fragment(0, end)
                };
            else
                _fragments = 
                    new Fragment[] { new Fragment(begin, buffer.Length - begin) };
        }
        else
            _fragments = null;
        return buffer;
    }

    public void BeginReadRecord()
    {
        readRecord = 0;
    }

    public int EndReadRecord()
    {
        return readRecord;
    }

    public void BeginWriteRecord()
    {
        writeRecord = 0;
    }

    public int EndWriteRecord()
    {
        return writeRecord;
    }

    public void Clear()
    {
        end = 0;
        begin = 0;
    }

    public void Reserve(int _length)
    {
        int _capacity = capacity + capacity / 2;
        if (_capacity < _length)
            _capacity = _length;
        byte[] _buffer = new byte[_capacity];

        if (end == -1)
        {
            int copyLen = buffer.Length - begin;
            Buffer.BlockCopy(buffer, begin, _buffer, 0, copyLen);
            if (begin > 0)
                Buffer.BlockCopy(buffer, 0, _buffer, copyLen, begin);
        }
        else if (end > begin)
        {
            Buffer.BlockCopy(buffer, begin, _buffer, 0, end - begin);
        }
        else if (end < begin)
        {
            int copyLen = buffer.Length - begin;
            Buffer.BlockCopy(buffer, begin, _buffer, 0, copyLen);
            if (end > 0)
                Buffer.BlockCopy(buffer, 0, _buffer, copyLen, end);
        }

        end = available;
        begin = 0;
        buffer = _buffer;
    }
}

