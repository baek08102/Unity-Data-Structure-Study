using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using JetBrains.Annotations;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class BinarySearchTree<TKey, TValue> : IDictionary<TKey, TValue>
where TKey : IComparable<TKey>
{
    protected TreeNode<TKey, TValue> root;

    public BinarySearchTree()
    {
        root = null;
    }

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException($"키 {key}를 찾을 수 없음.");
            }
        }

        set
        {
            root = AddOrUpdate(root, key, value);
        }
    }

    public ICollection<TKey> Keys => throw new System.NotImplementedException();

    public ICollection<TValue> Values => throw new System.NotImplementedException();

    public int Count => CountNodes(root);

    protected virtual int CountNodes(TreeNode<TKey, TValue> node)
    {
        if (node == null)
        {
            return 0;
        }
        return 1 + CountNodes(node.Left) + CountNodes(node.Right);
    }

    public bool IsReadOnly => false;

    public virtual void Add(TKey key, TValue value)
    {
        root = Add(root, key, value);
    }

    public virtual void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    protected virtual TreeNode<TKey, TValue> Add(TreeNode<TKey, TValue> node, TKey key, TValue value)
    {
        if (node == null)
        {
            return new TreeNode<TKey, TValue>(key, value);
        }

        int compare = key.CompareTo(node.Key);
        if (compare < 0)
        {
            node.Left = AddOrUpdate(node.Left, key, value);
        }
        else if (compare > 0)
        {
            node.Right = AddOrUpdate(node.Right, key, value);
        }
        else
        {
            throw new ArgumentException($"키 {key}는 이미 존재합니다.");
        }
        UpdateHeight(node);
        return node;
    }

    protected virtual TreeNode<TKey, TValue> AddOrUpdate(TreeNode<TKey, TValue> node, TKey key, TValue value)
    {
        if (node == null)
        {
            return new TreeNode<TKey, TValue>(key, value);
        }

        int compare = key.CompareTo(node.Key);
        if (compare < 0)
        {
            node.Left = Add(node.Left, key, value);
        }
        else if (compare > 0)
        {
            node.Right = Add(node.Right, key, value);
        }
        else
        {
            node.Value = value;
        }

        return node;
    }

    protected virtual TreeNode<TKey, TValue> Remove(TreeNode<TKey, TValue> node, TKey key)
    {
        int compare = key.CompareTo(node.Key);
        if (compare < 0)
        {
            node.Left = Remove(node.Left, key);
        }
        else if (compare > 0)
        {
            node.Right = Remove(node.Right, key);
        }
        else
        {
            if (node.Left == null)
            {
                return node.Right;
            }
            if (node.Right == null)
            {
                return node.Left;
            }

            var minNode = FindMin(node.Right);
            node.Key = minNode.Key;
            node.Value = minNode.Value;

            node.Right = Remove(node.Right, minNode.Key);
        }

        UpdateHeight(node);
        return node;
    }

    protected virtual void UpdateHeight(TreeNode<TKey, TValue> node)
    {
        node.Height = Mathf.Max(Height(node.Left), Height(node.Right)) + 1;
    }

    protected virtual int Height(TreeNode<TKey, TValue> node)
    {
        return node == null ? 0 : node.Height;
    }

    protected virtual TreeNode<TKey, TValue> FindMin(TreeNode<TKey, TValue> node)
    {
        while (node.Left != null)
        {
            node = node.Left;
        }
        return node;
    }


    public void Clear()
    {
        root = null;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ContainsKey(item.Key);
    }

    public bool ContainsKey(TKey key)
    {
        return TryGetValue(key, out _);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        여기 하셈
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return InOrderTraversal();
    }

    public bool Remove(TKey key)
    {
        if (!ContainsKey(key))
        {
            return false;
        }

        root = Remove(root, key);
        return true;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return TryGetValue(root, key, out value);
    }

    protected virtual bool TryGetValue(TreeNode<TKey, TValue> node, TKey key, out TValue value)
    {
        if (node == null)
        {
            value = default;
            return false;
        }

        int compare = key.CompareTo(node.Key);

        if (compare == 0)
        {
            value = node.Value;
            return true;
        }
        else if (compare < 0)
        {
            return TryGetValue(node.Left, key, out value);
        }
        else
        {
            return TryGetValue(node.Right, key, out value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public virtual IEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversal()
    {
        IEnumerable<KeyValuePair<TKey, TValue>> TraverseHelper(TreeNode<TKey, TValue> node)
        {
            if (node == null)
            {
                yield break;
            }

            foreach ()

        }

        throw new NotImplementedException();
    }


    public virtual IEnumerable<KeyValuePair<TKey, TValue>> PreOrderTraversal()
    {
        throw new NotImplementedException();
    }

    public virtual IEnumerable<KeyValuePair<TKey, TValue>> PostOrderTraversal()
    {
        throw new NotImplementedException();
    }

    private
}

