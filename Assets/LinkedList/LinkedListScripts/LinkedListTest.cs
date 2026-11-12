using System;
using System.Collections.Generic;
using UnityEngine;

public class LinkedListTest : MonoBehaviour
{
    private int failCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Run(TestForwardLinkedList);
    }

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

    private void TestSinglyLinkedList()
    {
        Debug.Log("=== SinglyLinkedList 테스트 시작 ===");

        var list = new SinglyLinkedList<int>();

        Debug.Log("\n1. 추가 (AddLast / AddFirst):");
        list.AddLast(20);
        list.AddLast(30);
        list.AddFirst(10);
        Expect(
            $"{string.Join(" -> ", list)} (개수: {list.Count})",
            "10 -> 20 -> 30 (개수: 3)",
            "내용"
        );
        Expect($"First: {list.First}, Last: {list.Last}", "First: 10, Last: 30", "양 끝");

        Debug.Log("\n2. 검색 (Contains):");
        Expect(list.Contains(20), true, "Contains(20)");
        Expect(list.Contains(99), false, "Contains(99)");

        Debug.Log("\n3. 제거 (Remove):");
        Expect(
            $"{list.Remove(30)} → {string.Join(" -> ", list)}, Last: {list.Last}",
            "True → 10 -> 20, Last: 20",
            "Remove(30)"
        );
        Expect(
            $"{list.Remove(99)} → {string.Join(" -> ", list)}",
            "False → 10 -> 20",
            "Remove(99)"
        );

        Debug.Log("\n4. 앞에서 꺼내기 (RemoveFirst):");
        Expect($"{list.RemoveFirst()} (남은 개수: {list.Count})", "10 (남은 개수: 1)", "꺼낸 값");
        Expect($"{list.RemoveFirst()} (남은 개수: {list.Count})", "20 (남은 개수: 0)", "꺼낸 값");

        Debug.Log("\n5. 빈 리스트 예외:");
        ExpectThrows<InvalidOperationException>(() => list.RemoveFirst(), "예외 발생");

        Debug.Log("\n6. Clear 후 재사용:");
        list.AddLast(1);
        list.AddLast(2);
        list.Clear();
        list.AddLast(3);
        Expect($"{string.Join(" -> ", list)} (개수: {list.Count})", "3 (개수: 1)", "내용");

        Debug.Log("\n=== SinglyLinkedList 테스트 완료 ===");
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

    private void TestForwardLinkedList()
    {
        Debug.Log("=== ForwardLinkedList 테스트 시작 ===");

        var list = new FowardLinkedList<string>();

        Debug.Log("\n1. 추가 (AddLast / AddFirst):");
        var b = list.AddLast("B");
        list.AddLast("D");
        list.AddFirst("A");
        Expect($"{string.Join(" -> ", list)} (개수: {list.Count})", "A -> B -> D (개수: 3)", "내용");

        Debug.Log("\n2. 노드 기준 삽입 (AddAfter / AddBefore):");
        list.AddAfter(b, "C");
        list.AddBefore(list.First, "시작");
        list.AddBefore(list.Last, "D 앞");
        Expect(string.Join(" -> ", list), "시작 -> A -> B -> C -> D 앞 -> D", "내용");

        Debug.Log("\n3. 노드로 제거 - 이전 노드를 찾아 순회 (Remove(node)):");
        list.Remove(b);
        Expect(string.Join(" -> ", list), "시작 -> A -> C -> D 앞 -> D", "B 노드 제거");

        Debug.Log("\n4. 값으로 제거 / 양 끝 제거:");
        Expect(list.Remove("D 앞"), true, "Remove(\"D 앞\")");
        list.RemoveFirst();
        list.RemoveLast();
        Expect($"{string.Join(" -> ", list)} (개수: {list.Count}), Last: {list.Last.Value}", "A -> C (개수: 2), Last: C", "남은 내용");

        Debug.Log("\n5. 다른 리스트의 노드 검증:");
        var other = new FowardLinkedList<string>();
        var foreign = other.AddLast("X");
        ExpectThrows<InvalidOperationException>(() => list.Remove(foreign), "예외 발생");

        Debug.Log("\n6. 이미 제거된 노드 재사용 검증:");
        ExpectThrows<InvalidOperationException>(() => list.AddAfter(b, "Z"), "예외 발생");

        Debug.Log("\n=== ForwardLinkedList 테스트 완료 ===");
    }

}


