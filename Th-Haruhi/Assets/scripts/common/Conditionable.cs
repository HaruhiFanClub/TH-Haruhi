
public class Conditionable
{
    public static implicit operator bool(Conditionable condition)
    {
        return condition != null;
    }

    public static bool operator == (Conditionable conl, Conditionable conr)
    {
        return object.Equals(conl, conr);
    }

    public static bool operator != (Conditionable conl, Conditionable conr)
    {
        return !object.Equals(conl, conr);
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public struct Fragment
{
    public int begin;
    public int length;

    public Fragment(int _begin = 0, int _length = 0)
    {
        begin = _begin;
        length = _length;
    }
}