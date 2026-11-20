using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SimpleHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private const int DefaultCapacity = 16;
    private const double LoadFacter = 0.75;

    private KeyValuePair<TKey, TValue>[] table;
    private bool[] occupied;
    private int size;
    private int count;

    public SimpleHashTable()
    {
        table = new KeyValuePair<TKey, TValue>[DefaultCapacity];
        occupied = new bool[DefaultCapacity];
        size = DefaultCapacity;
        count = 0;
    }

    private int GetHash(TKey key)
    {
        return GetHash(key, size);
    }

    private int GetHash(TKey key, int size)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        int hash = key.GetHashCode();
        return (hash & 0x7fffffff) % size;
    }

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }
            throw new KeyNotFoundException($"{nameof(key)}, {key}");
        }
        set
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if ((double)count / size > LoadFacter)
            {
                Resize();
            }

            int index = GetHash(key);
            if (occupied[index] && table[index].Key.Equals(key))
            {
                table[index] = new KeyValuePair<TKey, TValue>(key, value);
            }
            else if (!occupied[index])
            {
                table[index] = new KeyValuePair<TKey, TValue>(key, value);
                occupied[index] = true;
                ++count;
            }
            else
            {
                throw new InvalidOperationException("해시 출동");
            }
        }
    }

    public ICollection<TKey> Keys =>
        Enumerable.Range(0, size).Where(i => occupied[i]).Select(i => table[i].Key).ToList();

    public ICollection<TValue> Values =>
        Enumerable.Range(0, size).Where(i => occupied[i]).Select(i => table[i].Value).ToList();

    public int Count => count;
    public int Capacity => size;

    public bool IsReadOnly => false;

    private void Resize()
    {
        int newSize = size * 2;
        var newTable = new KeyValuePair<TKey, TValue>[newSize];
        var newOccupied = new bool[newSize];

        for (int i = 0; i < size; ++i)
        {
            if (!occupied[i])
            {
                continue;
            }
            int newIndex = GetHash(table[i].Key, newSize);
            if (newOccupied[newIndex])
            {
                throw new InvalidOperationException("해시 충돌");
            }

            newTable[newIndex] = table[i];
            newOccupied[newIndex] = true;
        }

        size = newSize;
        table = newTable;
        occupied = newOccupied;
    }

    public void Add(TKey key, TValue value)
    {
        if ((double)count / size > LoadFacter)
        {
            Resize();
        }

        int index = GetHash(key);
        if (!occupied[index])
        {
            table[index] = new KeyValuePair<TKey, TValue>(key, value);
            occupied[index] = true;
            ++count;
        }
        else if (table[index].Key.Equals(key))
        {
            throw new ArgumentException($"키 ({key}) 중복");
        }
        else
        {
            throw new InvalidOperationException("해시 충돌");
        }
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        Array.Clear(table, 0, size);
        Array.Clear(occupied, 0, size);
        count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if (TryGetValue(item.Key, out TValue value))
        {
            return value.Equals(item.Value);
        }
        return false;
    }

    public bool ContainsKey(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));

        }
        int index = GetHash(key);
        return occupied[index] && table[index].Key.Equals(key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        int currentIndex = arrayIndex;
        foreach (var kvp in this)
        {
            array[currentIndex++] = kvp;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < size; ++i)
        {
            if (occupied[i]) yield return table[i];
        }
    }

    public bool Remove(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        int index = GetHash(key);
        if (occupied[index] && table[index].Key.Equals(key))
        {
            occupied[index] = false;
            table[index] = default;
            --count;
            return true;
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        int index = GetHash(key);
        if (occupied[index] && table[index].Key.Equals(key))
        {
            value = table[index].Value;
            return true;
        }
        value = default;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerable<BucketView> GetBucketViews()
    {
        for (int i = 0; i < size; i++)
        {
            if (occupied[i])
            {
                yield return new BucketView(i, BucketState.Occupied, table[i].Key.ToString(), table[i].Value.ToString(), null);
            }
            else
            {
                yield return new BucketView(i, BucketState.Empty, null, null, null);
            }
        }
    }
}
