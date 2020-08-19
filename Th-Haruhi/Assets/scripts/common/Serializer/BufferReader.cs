
using System;

public class BufferReader : XSerializer
{
    int begin;
    byte[] buffer;

    public override int available
    {
        get
        {
            return buffer != null ?
                buffer.Length - begin : 0;
        }
    }

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

    public override int writePosition
    {
        get
        {
            throw new Exception("reader can not do write");
        }
        set
        {
            throw new Exception("reader can not do write");
        }
    }

    public override bool complete { get; protected set; }

    public BufferReader(byte[] _buffer)
    {
        begin = 0;
        buffer = _buffer;
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

    private int throwOverFlow(int _errType)
    {
        switch (_errType)
        {
            case 1:
                throw new Exception("<<<!!!!! data buffer read exception accured! [empty buffer]  !!!!!>>>");
                break;
            case 2:
                throw new Exception("<<<!!!!! data buffer read exception accured! [index overflow] !!!!!>>>");
                break;
        }
        return 0;
    }


    public override int ReadBytes(byte[] bytes, int readLen = -1)
    {
        if (readLen < 0)
            if ((readLen = bytes.Length) <= 0)
                return throwOverFlow(1);
        int _length = available;
        if (!(complete = readLen <= _length))
            if ((readLen = _length) <= 0)
                return throwOverFlow(2);
        Buffer.BlockCopy(buffer, begin, bytes, 0, readLen);
        begin += readLen;
        return readLen;
    }
    

    public override int WriteBytes(byte[] bytes, int offset, int writeLen)
    {
        throw new Exception("reader can not do write");
    }

    public override byte[] GetBuffer(out Fragment[] _fragments)
    {
        throw new Exception("buffer reader can not get buffer");
    }
    public byte[] GetData()
    {
        return buffer;
    }
}