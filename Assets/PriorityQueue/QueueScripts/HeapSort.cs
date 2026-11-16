using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine.TextCore.Text;

public static class HeapSort
{
    public static void Sort<T>(T[] array)
    {
        Sort(array, null);
    }

    public static void Sort<T>(T[] array, IComparer<T> comparer)
    {
        // TODO 1: null 검사
        if (array == null)
            throw new ArgumentNullException(nameof(array));

        if (comparer == null)
            comparer = Comparer<T>.Default;

        for (int i = array.Length / 2 - 1; i >= 0; --i)
        {
            HeapifyDown(array, i, array.Length, comparer);
        }

        int n = array.Length;
        for (int end = n - 1; end > 0; end--)
        {
            T temp = array[0];
            array[0] = array[end];
            array[end] = temp;
            HeapifyDown(array, 0, end, comparer);
        }

        // // PriorityQueue<T, T> queue = new PriorityQueue<T, T>(comparer);
        // foreach (T item in array)
        // {
        //     queue.Enqueue(item, item);
        // }

        // // TODO 3: 하나씩 꺼내 배열 앞부터 채운다
        // for (int i = 0; i < queue.Count; i++)
        // {
        //     array[i] = queue.Dequeue();
        // }
    }

    private static void HeapifyDown<T>(T[] array, int index, int size, IComparer<T> comparer)
    {
        while (true)
        {
            int leftChildIndex = 2 * index + 1;
            int rightChildIndex = 2 * index + 2;
            int largestIndex = index;

            if (leftChildIndex <= size &&
            comparer.Compare(array[leftChildIndex], array[largestIndex]) > 0)
            {
                largestIndex = leftChildIndex;
            }

            if (rightChildIndex <= size &&
            comparer.Compare(array[rightChildIndex], array[largestIndex]) > 0)
            {
                largestIndex = rightChildIndex;
            }

            if (index == largestIndex)
            {
                break;
            }
            // Swap(index, minIndex);

            T temp = array[index];
            array[index] = array[largestIndex];
            array[largestIndex] = temp;

            index = largestIndex;
        }
    }
}