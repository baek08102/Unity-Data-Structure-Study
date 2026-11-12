using System;
using System.Collections;
using System.Collections.Generic;

public class FowardLinkedListNode<T>
{
    public T Value;
    public FowardLinkedListNode<T> Next;
    public FowardLinkedList<T> List;
    public FowardLinkedListNode(T value)
    {
        Value = value;
    }
}

public class FowardLinkedList<T> : IEnumerable<T>
{
    private FowardLinkedListNode<T> head;
    private FowardLinkedListNode<T> tail;

    public int Count { get; private set; }
    public FowardLinkedListNode<T> First => head;
    public FowardLinkedListNode<T> Last => tail;

    public FowardLinkedListNode<T> AddFirst(T value)
    {
        var node = new FowardLinkedListNode<T>(value);
        node.List = this;

        node.Next = head;
        head = node;

        if (tail == null)
        {
            tail = node;
        }
        Count++;

        return node;
    }

    public FowardLinkedListNode<T> AddAfter(FowardLinkedListNode<T> node, T value)
    {
        ValidateNode(node);

        if (node == tail)
        {
            return AddLast(value);
        }

        var newNode = new FowardLinkedListNode<T>(value);
        newNode.List = this;

        newNode.Next = node.Next;
        node.Next = newNode;

        Count++;
        return newNode;
    }
    public FowardLinkedListNode<T> AddBefore(FowardLinkedListNode<T> node, T value)
    {
        ValidateNode(node);

        if (node == head)
        {
            return AddFirst(value);
        }

        var previous = head;
        while (previous.Next != node)
        {
            previous = previous.Next;
        }

        var newNode = new FowardLinkedListNode<T>(value);
        newNode.List = this;

        newNode.Next = node;
        previous.Next = newNode;

        Count++;
        return newNode;
    }

    public FowardLinkedListNode<T> AddLast(T value)
    {
        var node = new FowardLinkedListNode<T>(value);
        node.List = this;

        if (tail == null)
        {
            head = node;
            tail = node;
        }
        else
        {
            tail.Next = node;
            tail = node;
        }

        Count++;
        return node;
    }

    public void RemoveFirst()
    {
        if (head == null)
        {
            throw new InvalidOperationException("리스트가 비어있음");
        }

        Remove(head);
    }

    public void RemoveLast()
    {
        if (tail == null)
        {
            throw new InvalidOperationException("리스트가 비어있음");
        }

        Remove(tail);
    }

    public bool Remove(T value)
    {
        var node = Find(value);
        if (node == null)
        {
            return false;
        }

        Remove(node);
        return true;
    }

    public void Remove(FowardLinkedListNode<T> node)
    {
        ValidateNode(node);

        FowardLinkedListNode<T> previous = null;
        FowardLinkedListNode<T> current = head;

        while (current != node)
        {
            previous = current;
            current = current.Next;
        }

        if (previous == null)
        {
            head = node.Next;
        }
        else
        {
            previous.Next = node.Next;
        }

        if (node == tail)
        {
            tail = previous;
        }

        node.Next = null;
        node.List = null;

        Count--;
    }

    public bool Contains(T value)
    {
        if (Find(value) == null)
        {
            return false;
        }
        return true;
    }

    public FowardLinkedListNode<T> Find(T value)
    {
        var comparer = EqualityComparer<T>.Default;

        FowardLinkedListNode<T> current = head;
        while (current != null)
        {
            if (comparer.Equals(current.Value, value))
            {
                return current;
            }
            current = current.Next;
        }
        return null;
    }

    public void Clear()
    {
        var current = head;
        while (current != null)
        {
            var next = current.Next;
            current.Next = null;
            current.List = null;
            current = next;
        }

        head = tail = null;
        Count = 0;
    }

    public IEnumerator<T> GetEnumerator()
    {
        FowardLinkedListNode<T> current = head;
        while (current != null)
        {
            yield return current.Value;
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void ValidateNode(FowardLinkedListNode<T> node)
    {
        if (node == null)
        {
            throw new ArgumentNullException(nameof(node));
        }
        if (node.List != this)
        {
            throw new InvalidOperationException("이 리스트에 속한 노드가 아닙니다.");
        }
    }
}