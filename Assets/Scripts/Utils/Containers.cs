using System;

[Serializable]
public class KeyValuePair<T1, T2>
{
    public T1 key;
    public T2 value;

    public KeyValuePair(T1 aKey, T2 aValue)
    {
        key = aKey;
        value = aValue;
    }
}

[Serializable]
public class Row<T>
{
    public T[] m_column;
}
