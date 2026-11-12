using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.Archive;
using Unity.Properties;
using UnityEngine.InputSystem.Interactions;

public class SinglyLinkedList<T> : IEnumerable<T>
{
    public class SinglyLinkedListNode
    {
        public T Value;
        public SinglyLinkedListNode Next;

        public SinglyLinkedListNode(T value)
        {
            Value = value;
        }
    }

    private SinglyLinkedListNode head;
    private SinglyLinkedListNode tail;

    public int Count { get; private set; }
    public T First
    {
        get
        {
            if (head == null)
            {
                throw new InvalidOperationException("리스트가 비어있음");
            }
            return head.Value;
        }
    }
    public T Last
    {
        get
        {
            if (tail == null)
            {
                throw new InvalidOperationException("리스트가 비어있음");
            }
            return tail.Value;
        }
    }

    public void AddFirst(T value)
    {
        var node = new SinglyLinkedListNode(value);
        node.Next = head;
        head = node;

        if (tail == null)
        {
            tail = node;
        }
        Count++;
    }

    public void AddLast(T value)
    {
        var node = new SinglyLinkedListNode(value);
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
    }

    public T RemoveFirst()
    {
        if (head == null)
        {
            throw new InvalidOperationException("리스트가 비어있음");
        }

        T value = head.Value;
        head = head.Next;
        if (head == null)
        {
            tail = null;
        }
        Count--;
        return value;
    }

    public bool Remove(T value)
    {
        var comparer = EqualityComparer<T>.Default;

        SinglyLinkedListNode previous = null;
        SinglyLinkedListNode current = head;

        while (current != null)
        {
            if (comparer.Equals(current.Value, value))
            {
                if (previous == null)
                {
                    head = current.Next;
                }
                else
                {
                    previous.Next = current.Next;
                }

                if (current == tail)
                {
                    tail = previous;
                }
                Count--;
                return true;
            }

            previous = current;
            current = current.Next;
        }

        return false;
    }

    public bool Contains(T value)
    {
        var comparer = EqualityComparer<T>.Default;
        SinglyLinkedListNode current = head;
        while (current != null)
        {
            if (comparer.Equals(current.Value, value))
            {
                return true;
            }
            current = current.Next;
        }
        return false;
    }

    public void Clear()
    {
        head = tail = null;
        Count = 0;
    }

    public IEnumerator<T> GetEnumerator()
    {
        SinglyLinkedListNode current = head;
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

}