using System;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueueTest : MonoBehaviour
{
    private int failCount;

    void Start()
    {
        Run(TestPriorityQueue);

        Run(TestHeapSort);
        if (failCount == 0)
            Debug.Log("=== 모든 검사 통과 ===");
        else
            Debug.LogError($"=== 실패 {failCount}건 ===");
    }

    void TestPriorityQueue()
    {
        Debug.Log("=== PriorityQueue 테스트 시작 ===");

        // 우선순위 큐 생성 (작은 숫자가 높은 우선순위)
        var pq = new PriorityQueue<string, int>();

        Debug.Log("\n1. 요소 추가 (Enqueue):");
        pq.Enqueue("낮은 우선순위", 10);
        Debug.Log("추가: '낮은 우선순위' (우선순위: 10)");

        pq.Enqueue("높은 우선순위", 1);
        Debug.Log("추가: '높은 우선순위' (우선순위: 1)");

        pq.Enqueue("중간 우선순위", 5);
        Debug.Log("추가: '중간 우선순위' (우선순위: 5)");

        Expect(pq.Count, 3, "현재 큐 크기");

        Debug.Log("\n2. 최우선 요소 확인 (Peek):");
        Expect(pq.Peek(), "높은 우선순위", "최우선 요소");

        Debug.Log("\n3. 요소 제거 (Dequeue):");
        string[] expectedOrder = { "높은 우선순위", "중간 우선순위", "낮은 우선순위" };
        foreach (string expected in expectedOrder)
        {
            Expect(pq.Dequeue(), expected, "제거된 요소");
        }
        Expect(pq.Count, 0, "남은 개수");

        Debug.Log("\n4. 숫자 우선순위 테스트:");
        var numberPQ = new PriorityQueue<int, int>();

        int[] priorities = { 30, 10, 50, 20, 40 };
        foreach (int priority in priorities)
        {
            numberPQ.Enqueue(priority * 100, priority);
            Debug.Log($"추가: {priority * 100} (우선순위: {priority})");
        }

        Debug.Log("\n정렬된 순서로 제거:");
        int[] expectedValues = { 1000, 2000, 3000, 4000, 5000 };
        foreach (int expected in expectedValues)
        {
            Expect(numberPQ.Dequeue(), expected, "값");
        }

        Debug.Log("\n5. Clear · 빈 큐 테스트:");
        pq.Enqueue("테스트1", 1);
        pq.Enqueue("테스트2", 2);
        Expect(pq.Count, 2, "Clear 전 크기");
        pq.Clear();
        Expect(pq.Count, 0, "Clear 후 크기");
        ExpectThrows<InvalidOperationException>(() => pq.Dequeue(), "빈 큐 Dequeue");
        ExpectThrows<InvalidOperationException>(() => pq.Peek(), "빈 큐 Peek");

        Debug.Log("\n6. Max-Heap 테스트 (커스텀 Comparer):");
        var maxPQ = new PriorityQueue<int, int>(
            Comparer<int>.Create((a, b) => b.CompareTo(a))
        );

        int[] maxPriorities = { 30, 10, 50, 20, 40 };
        foreach (int priority in maxPriorities)
        {
            maxPQ.Enqueue(priority * 100, priority);
            Debug.Log($"추가: {priority * 100} (우선순위: {priority})");
        }

        Debug.Log("\n내림차순으로 제거:");
        int[] expectedMaxValues = { 5000, 4000, 3000, 2000, 1000 };
        foreach (int expected in expectedMaxValues)
        {
            Expect(maxPQ.Dequeue(), expected, "값");
        }

        Debug.Log("\n7. BuildHeap 테스트 (원소 목록으로 생성):");
        var items = new List<(string, int)> { ("E", 50), ("B", 20), ("D", 40), ("A", 10), ("C", 30) };
        var builtPQ = new PriorityQueue<string, int>(items);
        Expect(builtPQ.Count, 5, "생성 직후 크기");

        var dequeued = new List<string>();
        while (builtPQ.Count > 0)
        {
            dequeued.Add(builtPQ.Dequeue());
        }
        Expect(string.Join(", ", dequeued), "A, B, C, D, E", "꺼낸 순서");

        var builtMaxPQ = new PriorityQueue<string, int>(
            items,
            Comparer<int>.Create((a, b) => b.CompareTo(a))
        );
        dequeued.Clear();
        while (builtMaxPQ.Count > 0)
        {
            dequeued.Add(builtMaxPQ.Dequeue());
        }
        Expect(string.Join(", ", dequeued), "E, D, C, B, A", "Max-Heap 꺼낸 순서");

        ExpectThrows<ArgumentNullException>(
            () => new PriorityQueue<string, int>((IEnumerable<(string, int)>)null),
            "null 목록"
        );

        Debug.Log("\n=== PriorityQueue 테스트 완료 ===");
    }
    void TestHeapSort()
    {
        Debug.Log("=== HeapSort 테스트 시작 ===");

        Debug.Log("\n1. 기본 비교자 (오름차순):");
        int[] numbers = { 30, 10, 50, 20, 40, 10 };
        HeapSort.Sort(numbers);
        Expect(string.Join(", ", numbers), "10, 10, 20, 30, 40, 50", "정렬 결과");

        Debug.Log("\n2. 커스텀 비교자 (내림차순):");
        int[] reversed = { 30, 10, 50, 20, 40, 10 };
        HeapSort.Sort(reversed, Comparer<int>.Create((a, b) => b.CompareTo(a)));
        Expect(string.Join(", ", reversed), "50, 40, 30, 20, 10, 10", "정렬 결과");

        Debug.Log("\n3. 문자열 정렬:");
        string[] words = { "dog", "apple", "cat", "banana" };
        HeapSort.Sort(words);
        Expect(string.Join(", ", words), "apple, banana, cat, dog", "정렬 결과");

        Debug.Log("\n4. 경계 입력:");
        int[] empty = { };
        HeapSort.Sort(empty);
        Expect(empty.Length, 0, "빈 배열 길이");
        int[] single = { 7 };
        HeapSort.Sort(single);
        Expect(single[0], 7, "원소 1개");
        ExpectThrows<ArgumentNullException>(() => HeapSort.Sort<int>(null), "null 배열");

        Debug.Log("\n5. 무작위 1000개를 Array.Sort 결과와 비교:");
        var random = new System.Random(42);
        int[] randomNumbers = new int[1000];
        for (int i = 0; i < randomNumbers.Length; i++)
        {
            randomNumbers[i] = random.Next(-500, 500);
        }
        int[] expectedNumbers = (int[])randomNumbers.Clone();
        Array.Sort(expectedNumbers);

        HeapSort.Sort(randomNumbers);
        int mismatchIndex = -1;
        for (int i = 0; i < randomNumbers.Length; i++)
        {
            if (randomNumbers[i] != expectedNumbers[i])
            {
                mismatchIndex = i;
                break;
            }
        }
        Expect(mismatchIndex, -1, "처음 어긋난 인덱스");

        Debug.Log("\n=== HeapSort 테스트 완료 ===");
    }

    // 테스트가 예상 못 한 예외로 멈추면 실패로 집계하고 원인을 남긴다
    void Run(Action test)
    {
        try
        {
            test();
        }
        catch (Exception e)
        {
            failCount++;
            Debug.LogError($"[실패] {test.Method.Name} 중단 - 예상하지 못한 예외: {e}");
        }
    }

    void Expect<T>(T actual, T expected, string label)
    {
        if (EqualityComparer<T>.Default.Equals(actual, expected))
        {
            Debug.Log($"[통과] {label}: {actual}");
        }
        else
        {
            failCount++;
            Debug.LogError($"[실패] {label}: {actual} (기대값: {expected})");
        }
    }

    void ExpectThrows<TException>(Action action, string label)
        where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException e)
        {
            Debug.Log($"[통과] {label}: {e.Message}");
            return;
        }

        failCount++;
        Debug.LogError($"[실패] {label}: {typeof(TException).Name}이 발생하지 않음");
    }
}