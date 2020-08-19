

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class Table : Conditionable
{
	public Type type;
	public string md5;
    protected Dictionary<System.Object, System.Object> sections;

    public System.Object this[System.Object id]
    {
        get
        {
            return GetSection(id);
        }
        set
        {
            SetSection(id, value);
        }
    }

    public int count
    {
        get
        {
            return sections.Count;
        }
    }

    public Table()
    {
        sections = new Dictionary<System.Object, System.Object>();
    }
   
    public System.Object GetSection(System.Object id)
    {
        System.Object _section;
        if(id == null)
        {
            UnityEngine.Debug.LogError("GetSection Id Obj = null");
        }
        sections.TryGetValue(id, out _section);
        return _section;
    }

    /// <returns>
    /// 若有当前id,则返回当前id,不然则返回小于当前id中的最大id
    /// </returns>
    public System.Object GetSectionEqualOrLess(System.Object id)
    {
        System.Object _section;
        sections.TryGetValue(id, out _section);
        if(_section != null) return _section;

        int maxKey = 0;
        int maxId = System.Convert.ToInt32(id);
        //查找最近的那一个
        foreach(System.Object key in sections.Keys)
        {
            int tempKey = System.Convert.ToInt32(key);
            if(tempKey > maxKey && tempKey < maxId) maxKey = tempKey;
        }
        sections.TryGetValue(maxKey, out _section);
        return _section;
    }

    public void SetSection(System.Object id, System.Object _section)
    {
        if (id != null)
            sections[id] = _section;
    }

    public IEnumerator<System.Object> GetEnumerator()
    {
        return sections.Values.GetEnumerator();
    }
}

public class TableT<T> : Conditionable where T : class
{
    Table table;

    public TableT(Table _table)
    {
        table = _table;
    }

    public T this[System.Object id]
    {
        get
        {
            return table ? (table[id] as T) : null;
        }
        set
        {
            if (table)
                table[id] = value;
        }
    }

    public int count
    {
        get
        {
            return table.count;
        }
    }

    public T GetSection(System.Object id)
    {
        return table ? table.GetSection(id) as T : null;
    }
    /// <returns>
    /// 若有当前id,则返回当前id,不然则返回小于当前id中的最大id
    /// </returns>
    public T GetSectionEqualOrLess(System.Object id)
    {
        return table ? table.GetSectionEqualOrLess(id) as T : null;
    }

    public void SetSection(System.Object id, T _section)
    {
        if (table)
            table.SetSection(id, _section);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator<T>(table.GetEnumerator());
    }

    [Serializable]
    public struct Enumerator<Q> : IEnumerator<Q> where Q : class
    {
        IEnumerator<System.Object> enumerator;

        public Enumerator(IEnumerator<System.Object> _enumerator)
        {
            enumerator = _enumerator;
        }

        public Q Current
        {
            get
            {
                return enumerator.Current as Q;
            }
        }

        System.Object IEnumerator.Current
        {
            get 
            { 
                return Current; 
            }
        }

        public void Dispose()
        {
            enumerator.Dispose();
        }

        public bool MoveNext()
        {
            return enumerator.MoveNext();
        }

        public void Reset()
        {
            enumerator.Reset();
        }
    }
}