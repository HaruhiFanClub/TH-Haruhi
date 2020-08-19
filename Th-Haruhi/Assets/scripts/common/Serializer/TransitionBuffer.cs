
using System;
using System.Runtime.InteropServices;

public class TransitionBuffer
{
    byte[] buffer;

    public TransitionBuffer(int capacity = 1024)
    {
        buffer = new byte[capacity];
    }

    public void RequireCapsity(int capacity)
    {
        int _capacity = buffer.Length;
        if (capacity <= _capacity)
            return;
        int c =
            _capacity + _capacity / 2;
        if (capacity < c)
            capacity = c;
        byte[] newbuf = new byte[capacity];
        Buffer.BlockCopy(buffer, 0, newbuf, 0, _capacity);
        buffer = newbuf;
    }

    public void CopyFrom(Array source, int offset, int count, int elementSize = 0)
    {
        if (elementSize <= 0)
            elementSize =
                Marshal.SizeOf(source.GetType().GetElementType());
        int copySize = elementSize * count;
        if (copySize + offset > buffer.Length)
            RequireCapsity(copySize + offset);
        GCHandle handle = GCHandle.Alloc(source, GCHandleType.Pinned);
        IntPtr ptr = handle.AddrOfPinnedObject();
        Marshal.Copy(ptr, buffer, offset, copySize);
        handle.Free();
    }

    public void CopyTo(Array destination, int offset, int count, int elementSize = 0)
    {
        if (elementSize <= 0)
            elementSize =
                Marshal.SizeOf(destination.GetType().GetElementType());
        GCHandle handle =
            GCHandle.Alloc(destination, GCHandleType.Pinned);
        IntPtr ptr = handle.AddrOfPinnedObject();
        Marshal.Copy(buffer, offset, ptr, elementSize * count);
        handle.Free();
    }

    public void Expand(int stride, int expandSize)
    {
        int c = expandSize / stride;
        int i = 1;
        while (i < c)
            Buffer.BlockCopy(buffer, 0, buffer, i++ * stride, stride);
        c = expandSize - c * stride;

        if (c > 0)
            Buffer.BlockCopy(buffer, 0, buffer, i * stride, c);
    }

    static TransitionBuffer sharedBuffer
    {
        get
        {
            if (sharedBuffer_ == null)
                sharedBuffer_ = new TransitionBuffer();
            return sharedBuffer_;
        }
    }
    static TransitionBuffer sharedBuffer_;

    public static void Copy(Array source, Array destination, int count = -1, int elementSize = 0)
    {
        if (count < 0)
            count = source.Length;
        sharedBuffer.CopyFrom(source, 0, count, elementSize);
        sharedBuffer.CopyTo(destination, 0, count, elementSize);
    }

    public static byte[] ArrayToBytes(Array array, int count = -1, int elementSize = 0)
    {
        if (count < 0)
            count = array.Length;
        if (elementSize <= 0)
            elementSize =
                Marshal.SizeOf(array.GetType().GetElementType());
        int byteLen = elementSize * count;
        byte[] bytes = new byte[byteLen];
        GCHandle handle = GCHandle.Alloc(array, GCHandleType.Pinned);
        IntPtr ptr = handle.AddrOfPinnedObject();
        Marshal.Copy(ptr, bytes, 0, byteLen);
        handle.Free();
        return bytes;
    }

    public static T[] BytesToArray<T>(byte[] bytes, int count = -1, int elementSize = 0)
    {
        if (count < 0)
            count = bytes.Length;
        if (elementSize <= 0)
            elementSize = Marshal.SizeOf(typeof(T));
        int arrayLen = count / elementSize;
        T[] array = new T[arrayLen];
        GCHandle handle =
          GCHandle.Alloc(array, GCHandleType.Pinned);
        IntPtr ptr = handle.AddrOfPinnedObject();
        Marshal.Copy(bytes, 0, ptr, elementSize * arrayLen);
        handle.Free();
        return array;
    }
}