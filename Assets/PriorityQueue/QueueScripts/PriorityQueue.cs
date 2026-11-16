using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using NUnit.Framework;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class PriorityQueue<TElement, TPriority>
{
    private List<(TElement Element, TPriority Priority)> heap;
    private readonly IComparer<TPriority> comparer;

    public int Count => heap.Count;

    public PriorityQueue()
    {
        heap = new List<(TElement Element, TPriority Priority)>();
        comparer = Comparer<TPriority>.Default;
    }

    public PriorityQueue(IComparer<TPriority> comparer)
    {
        heap = new List<(TElement Element, TPriority Priority)>();
        this.comparer = comparer;
    }
    public PriorityQueue(IEnumerable<(TElement Element, TPriority Priority)> items)
    {
        heap = new List<(TElement Element, TPriority Priority)>();
        comparer = Comparer<TPriority>.Default;

        if (items == null)
            throw new ArgumentNullException(nameof(items));

        foreach (var item in items)
        {
            Enqueue(item.Element, item.Priority);
        }
    }
    public PriorityQueue(IEnumerable<(TElement Element, TPriority Priority)> items, IComparer<TPriority> comparer)
    {
        heap = new List<(TElement Element, TPriority Priority)>();
        this.comparer = comparer;

        if (items == null)
            throw new ArgumentNullException(nameof(items));

        foreach (var item in items)
        {
            Enqueue(item.Item1, item.Item2);
        }
    }

    public void Clear()
    {
        heap.Clear();
    }

    public TElement Peek()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("큐가 비어있음");

        return heap[0].Element;
    }

    private void Swap(int i, int j)
    {
        var temp = heap[i];
        heap[i] = heap[j];
        heap[j] = temp;
    }

    public void Enqueue(TElement element, TPriority prioirity)
    {
        heap.Add((element, prioirity));
        int index = heap.Count - 1;
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;

            if (comparer.Compare(heap[index].Priority, heap[parentIndex].Priority) >= 0)
            {
                break;
            }
            Swap(index, parentIndex);

            index = parentIndex;
        }
    }

    public TElement Dequeue()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("큐가 비어있음");

        TElement result = heap[0].Element;
        int lastIndex = heap.Count - 1;
        heap[0] = heap[lastIndex];
        heap.RemoveAt(lastIndex);


        int index = 0;
        HeapifyDown(index);

        return result;
    }

    private void HeapifyDown(int index)
    {
        int lastIndex = heap.Count - 1;

        while (true)
        {
            int leftChildIndex = 2 * index + 1;
            int rightChildIndex = 2 * index + 2;
            int minIndex = index;

            if (leftChildIndex <= lastIndex &&
            comparer.Compare(heap[leftChildIndex].Priority, heap[minIndex].Priority) < 0)
            {
                minIndex = leftChildIndex;
            }

            if (rightChildIndex <= lastIndex &&
            comparer.Compare(heap[rightChildIndex].Priority, heap[minIndex].Priority) < 0)
            {
                minIndex = rightChildIndex;
            }

            if (index == minIndex)
            {
                break;
            }

            Swap(index, minIndex);
            index = minIndex;
        }
    }
}
