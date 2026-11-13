using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CircularDoublyLinkedListNode<T>
{
    public T Value { get; set; }
    public CircularDoublyLinkedList<T> List { get; set; }

    public CircularDoublyLinkedListNode<T> next;
    public CircularDoublyLinkedListNode<T> prev;

    public CircularDoublyLinkedListNode<T> Next => next == null || next == List.First ? null : next;
    public CircularDoublyLinkedListNode<T> Previous => prev == null || this == List.First ? null : prev;

    public CircularDoublyLinkedListNode(T value)
    {
        Value = value;
    }
}

public class CircularDoublyLinkedList<T> : IEnumerable<T>
{
    private CircularDoublyLinkedListNode<T> head;

    public int Count { get; private set; }
    public CircularDoublyLinkedListNode<T> First => head;
    public CircularDoublyLinkedListNode<T> Last => head?.prev;

    public CircularDoublyLinkedListNode<T> AddFirst(T value)
    {
        var newNode = new CircularDoublyLinkedListNode<T>(value);
        newNode.List = this;

        InsertBefore(head, newNode);

        head = newNode;

        Count++;
        return newNode;
    }

    public CircularDoublyLinkedListNode<T> AddLast(T value)
    {
        var newNode = new CircularDoublyLinkedListNode<T>(value);
        newNode.List = this;


        InsertBefore(head, newNode);

        Count++;
        return newNode;
    }

    public CircularDoublyLinkedListNode<T> AddAfter(CircularDoublyLinkedListNode<T> node, T value)
    {
        ValidateNode(node);

        if (node == head.prev)
        {
            return AddLast(value);
        }

        var newNode = new CircularDoublyLinkedListNode<T>(value);
        newNode.List = this;

        InsertBefore(node.next, newNode);

        Count++;
        return newNode;
    }
    public CircularDoublyLinkedListNode<T> AddBefore(CircularDoublyLinkedListNode<T> node, T value)
    {
        ValidateNode(node);

        if (node == head)
        {
            return AddFirst(value);
        }

        var newNode = new CircularDoublyLinkedListNode<T>(value);
        newNode.List = this;

        InsertBefore(node, newNode);

        Count++;
        return newNode;
    }

    private void InsertBefore(CircularDoublyLinkedListNode<T> target, CircularDoublyLinkedListNode<T> newNode)
    {
        if (target == null)
        {
            head = newNode;
            newNode.prev = newNode.next = newNode;
            return;
        }

        newNode.prev = target.prev;
        newNode.next = target;
        target.prev.next = newNode;
        target.prev = newNode;
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
        if (head.prev == null)
        {
            throw new InvalidOperationException("리스트가 비어있음");
        }

        Remove(head.prev);
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

    public void Remove(CircularDoublyLinkedListNode<T> node)
    {
        ValidateNode(node);

        if (node == node.next)
        {
            head = null;

            node.prev = node.next = null;

            node.List = null;

            Count--;

            return;
        }

        if (node == head)
        {
            head = node.next;
        }

        node.prev.next = node.next;
        node.next.prev = node.prev;
        node.prev = node.next = null;
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

    public CircularDoublyLinkedListNode<T> Find(T value)
    {
        var comparer = EqualityComparer<T>.Default;

        CircularDoublyLinkedListNode<T> current = head;
        do
        {
            if (comparer.Equals(current.Value, value))
            {
                return current;
            }
            current = current.next;
        } while (current != head);
        return null;
    }

    public void Clear()
    {
        var current = head;
        do
        {
            var next = current.next;
            current.next = null;
            current.List = null;
            current = next;
        } while (current != head);

        head = null;
        Count = 0;
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (head == null) yield break;
        CircularDoublyLinkedListNode<T> current = head;
        do
        {
            yield return current.Value;
            current = current.next;
        } while (current != head);
    }

    public IEnumerable<T> Backward()
    {
        if (head == null) yield break;
        CircularDoublyLinkedListNode<T> current = head.prev;
        do
        {
            yield return current.Value;
            current = current.prev;
        } while (current != head.prev);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void ValidateNode(CircularDoublyLinkedListNode<T> node)
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