using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices.WindowsRuntime;
using JetBrains.Annotations;
using Mono.Cecil.Cil;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Tilemaps;
using UnityEngine;

// 충돌 해결 = 버킷별 LinkedList 체인
public class ChainingHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private const int DefaultCapacity = 16;
    private const double LoadFactor = 0.75;

    private LinkedList<KeyValuePair<TKey, TValue>>[] buckets;
    private int size;
    private int count;
    private EqualityComparer<TKey> keyComparer;
    private EqualityComparer<TValue> valueComparer;

    public ChainingHashTable()
    {
        buckets = new LinkedList<KeyValuePair<TKey, TValue>>[DefaultCapacity];
        size = DefaultCapacity;
        count = 0;

        keyComparer = EqualityComparer<TKey>.Default;
        valueComparer = EqualityComparer<TValue>.Default;
    }

    private int GetHash(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int hash = key.GetHashCode();
        // 부호 비트만 제거해 항상 0 이상. Math.Abs는 int.MinValue에서 음수를 그대로 돌려주므로 쓰지 않는다.
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
            throw new KeyNotFoundException();
        }
        set
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if ((double)count / size > LoadFactor)
            {
                Resize();
            }

            int index = GetHash(key);

            // [빈칸 ⑤] 같은 키가 체인에 있으면 값을 바꾸고, 없으면 체인 끝에 추가한다
            if (buckets[index] == null)
            {
                buckets[index] = new LinkedList<KeyValuePair<TKey, TValue>>();
            }

            if (ContainsKey(key))
            {
                var node = buckets[index].First;
                while (node != null)
                {
                    if (keyComparer.Equals(key, node.Value.Key))
                    {
                        node.Value = new KeyValuePair<TKey, TValue>(key, value);
                    }
                    node = node.Next;
                }
            }
            else
            {
                Add(key, value);
            }
        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            var keys = new List<TKey>();
            for (int i = 0; i < size; i++)
            {
                if (buckets[i] != null)
                {
                    foreach (var kvp in buckets[i])
                    {
                        keys.Add(kvp.Key);
                    }
                }
            }
            return keys;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            var values = new List<TValue>();
            for (int i = 0; i < size; i++)
            {
                if (buckets[i] != null)
                {
                    foreach (var kvp in buckets[i])
                    {
                        values.Add(kvp.Value);
                    }
                }
            }
            return values;
        }
    }

    public int Count => count;

    public int Capacity => size;

    public bool IsReadOnly => false;

    private void Resize()
    {
        var oldBuckets = buckets;
        var oldSize = size;

        size *= 2;
        buckets = new LinkedList<KeyValuePair<TKey, TValue>>[size];

        // [빈칸 ④] count를 0으로 돌리고, 옛 버킷의 모든 항목을 Add로 다시 넣는다
        count = 0;

        for (int i = 0; i < oldSize; ++i)
        {
            if (oldBuckets[i] == null)
            {
                continue;
            }

            foreach (var kvp in oldBuckets[i])
            {
                Add(kvp);
            }
        }
    }

    public void Add(TKey key, TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if (ContainsKey(key))
            throw new ArgumentException("키 중복");

        if ((double)count / size > LoadFactor)
        {
            Resize();
        }

        int index = GetHash(key);
        // [빈칸 ①] 버킷이 null이면 새 LinkedList를 만들고, 체인 끝에 추가한 뒤 count를 늘린다
        if (buckets[index] == null)
        {
            buckets[index] = new LinkedList<KeyValuePair<TKey, TValue>>();
        }
        buckets[index].AddLast(new KeyValuePair<TKey, TValue>(key, value));
        count++;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        buckets = new LinkedList<KeyValuePair<TKey, TValue>>[size];
        count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        int index = GetHash(item.Key);
        if (buckets[index] == null)
            return false;

        foreach (var kvp in buckets[index])
        {
            if (EqualityComparer<TKey>.Default.Equals(item.Key, kvp.Key) &&
             EqualityComparer<TValue>.Default.Equals(item.Value, kvp.Value))
            {
                return true;
            }
        }
        return false;
    }

    public bool ContainsKey(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int index = GetHash(key);
        if (buckets[index] == null)
            return false;

        foreach (var kvp in buckets[index])
        {
            if (EqualityComparer<TKey>.Default.Equals(kvp.Key, key))
            {
                return true;
            }
        }
        return false;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < count)
            throw new ArgumentException("배열 크기 부족");

        int index = arrayIndex;
        foreach (var kvp in this)
        {
            array[index++] = kvp;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < size; i++)
        {
            if (buckets[i] != null)
            {
                foreach (var kvp in buckets[i])
                {
                    yield return kvp;
                }
            }
        }
    }

    public bool Remove(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int index = GetHash(key);

        // [빈칸 ③] 체인을 LinkedListNode로 따라가며 같은 키의 노드를 지운다

        if (buckets[index] == null)
        {
            return false;
        }

        var node = buckets[index].First;
        while (node != null)
        {
            if (keyComparer.Equals(key, node.Value.Key))
            {
                buckets[index].Remove(node);
                count--;
                return true;
            }
            node = node.Next;
        }
        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (item.Key == null)
            throw new ArgumentNullException(nameof(item.Key));

        int index = GetHash(item.Key);

        // [빈칸 ③] 체인을 LinkedListNode로 따라가며 같은 키의 노드를 지운다

        if (buckets[index] == null)
        {
            return false;
        }

        var node = buckets[index].First;
        while (node != null)
        {
            if (keyComparer.Equals(item.Key, node.Value.Key) &&
            valueComparer.Equals(item.Value, node.Value.Value))
            {
                buckets[index].Remove(node);
                count--;
                return true;
            }
            node = node.Next;
        }
        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int index = GetHash(key);

        // [빈칸 ②] 버킷의 체인을 순회해 같은 키를 찾으면 value에 담고 true
        foreach (var kvp in buckets[index])
        {
            if (keyComparer.Equals(key, kvp.Key))
            {
                value = kvp.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    public IEnumerable<BucketView> GetBucketViews()
    {
        for (int i = 0; i < size; i++)
        {
            if (buckets[i] == null || buckets[i].Count == 0)
            {
                yield return new BucketView(i, BucketState.Empty, null, null, null);
                continue;
            }

            // [빈칸 ⑥] 체인의 (Key, Value)를 문자열 튜플 리스트로 모아 Occupied로 돌려준다
            var chain = new List<(string, string)>();
            foreach (var kvp in buckets[i])
            {
                // chain.Add((kvp.Key.ToString(), kvp.Value.ToString()));
                chain.Add((Convert.ToString(kvp.Key), Convert.ToString(kvp.Value)));
            }
            yield return new BucketView(i, BucketState.Occupied, null, null, chain);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}