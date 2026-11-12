using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DoublyLinkedListNode<T>
{
    public T Value;
    public DoublyLinkedListNode<T> Next;
    public DoublyLinkedListNode<T> Previous;
    public DoublyLinkedList<T> List;
    public DoublyLinkedListNode(T value)
    {
        Value = value;
    }
}

public class DoublyLinkedList<T> : IEnumerable<T>
{
    private DoublyLinkedListNode<T> head;
    private DoublyLinkedListNode<T> tail;

    public int Count { get; private set; }
    public DoublyLinkedListNode<T> First => head;
    public DoublyLinkedListNode<T> Last => tail;

    public DoublyLinkedListNode<T> AddFirst(T value)
    {
        var node = new DoublyLinkedListNode<T>(value);
        node.List = this;

        node.Next = head;
        head.Previous = node;
        node.Previous = null;
        head = node;

        if (tail == null)
        {
            tail = node;
        }
        Count++;

        return node;
    }

    public DoublyLinkedListNode<T> AddAfter(DoublyLinkedListNode<T> node, T value)
    {
        ValidateNode(node);

        if (node == tail)
        {
            return AddLast(value);
        }

        var newNode = new DoublyLinkedListNode<T>(value);
        newNode.List = this;

        newNode.Next = node.Next;
        node.Next = newNode;
        newNode.Next.Previous = newNode;
        newNode.Previous = node;

        Count++;
        return newNode;
    }
    public DoublyLinkedListNode<T> AddBefore(DoublyLinkedListNode<T> node, T value)
    {
        ValidateNode(node);

        if (node == head)
        {
            return AddFirst(value);
        }

        var newNode = new DoublyLinkedListNode<T>(value);
        newNode.List = this;

        newNode.Next = node;
        newNode.Previous = node.Previous;
        node.Previous.Next = newNode;
        node.Previous = newNode;

        Count++;
        return newNode;
    }

    public DoublyLinkedListNode<T> AddLast(T value)
    {
        var node = new DoublyLinkedListNode<T>(value);
        node.List = this;

        if (tail == null)
        {
            head = node;
            tail = node;
        }
        else
        {
            tail.Next = node;
            node.Previous = tail;
            node.Next = null;
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

    public void Remove(DoublyLinkedListNode<T> node)
    {
        ValidateNode(node);

        if (node == head)
        {
            head = node.Next;
            head.Previous = null;
            node.Next = null;
        }
        else if (node == tail)
        {
            tail = node.Previous;
            tail.Next = null;
            node.Previous = null;
        }
        else
        {
            node.Previous.Next = node.Next;
            node.Next.Previous = node.Previous;
        }

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

    public DoublyLinkedListNode<T> Find(T value)
    {
        var comparer = EqualityComparer<T>.Default;

        DoublyLinkedListNode<T> current = head;
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
        DoublyLinkedListNode<T> current = head;
        while (current != null)
        {
            yield return current.Value;
            current = current.Next;
        }
    }

    public IEnumerable<T> Backward()
    {
        DoublyLinkedListNode<T> current = tail;
        while (current != null)
        {
            yield return current.Value;
            current = current.Previous;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void ValidateNode(DoublyLinkedListNode<T> node)
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