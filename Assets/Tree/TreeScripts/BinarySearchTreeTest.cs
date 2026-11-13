using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

// 이진 탐색 트리 동작을 Console 창 로그로 확인한다
public class BinarySearchTreeTest : MonoBehaviour
{
    public TreeVisualizer treeVisualizer;
    public int nodeCount = 10;
    public int minKey = 1;
    public int maxKey = 1000;

    private int failCount;

    private void Start()
    {
        // Run(TestBinarySearchTree);

        // if (failCount == 0)
        //     Debug.Log("=== 모든 검사 통과 ===");
        // else
        //     Debug.LogError($"=== 실패 {failCount}건 ===");

        GenerateRandomTree();
    }

    [ContextMenu("GenerateRandomTree")]
    public void GenerateRandomTree()
    {
        var tree = new BinarySearchTreeTest<int, string>();
        for (int i = 0; i < nodeCount; ++i)
        {
            int key = UnityEngine.Random.Range(minKey, maxKey);
            if (!tree.ContainsKey(key))
            {
                string value = $"VALUE {key}";
                tree.Add(key, value);
            }
        }

        treeVisualizer.Visualize(tree.GetRoot());
    }

    void TestBinarySearchTree()
    {
        Debug.Log("=== BinarySearchTree 테스트 시작 ===");

        //        50
        //      /    \
        //    30      70
        //   /  \    /  \
        //  20  40  60  80
        var bst = new BinarySearchTree<int, string>();
        int[] keys = { 50, 30, 70, 20, 40, 60, 80 };

        Debug.Log("\n1. 추가 (Add):");
        foreach (int key in keys)
        {
            bst.Add(key, $"V-{key}");
        }
        Expect(bst.Count, 7, "노드 개수");
        ExpectThrows<ArgumentException>(() => bst.Add(50, "중복"), "중복 키 Add");

        Debug.Log("\n2. 탐색 (TryGetValue · 인덱서):");
        Expect(bst.ContainsKey(40), true, "40 포함");
        Expect(bst.ContainsKey(45), false, "45 포함");
        Expect(bst[60], "V-60", "bst[60]");
        ExpectThrows<KeyNotFoundException>(() => _ = bst[99], "없는 키 조회");

        Debug.Log("\n3. 인덱서로 값 갱신:");
        bst[60] = "새 값";
        Expect(bst[60], "새 값", "갱신된 bst[60]");
        Expect(bst.Count, 7, "갱신 후 개수 그대로");

        Debug.Log("\n4. 순회:");
        Expect(KeysOf(bst.InOrderTraversal()), "20 30 40 50 60 70 80", "중위 순회");
        Expect(KeysOf(bst.PreOrderTraversal()), "50 30 20 40 70 60 80", "전위 순회");
        Expect(KeysOf(bst.PostOrderTraversal()), "20 40 30 60 80 70 50", "후위 순회");
        Expect(KeysOf(bst.LevelOrderTraversal()), "50 30 70 20 40 60 80", "레벨 순회");

        Debug.Log("\n5. 삭제 (Remove):");
        Expect(bst.Remove(20), true, "리프 20 삭제");
        Expect(bst.Remove(30), true, "자식 1개 30 삭제");
        Expect(bst.Remove(50), true, "자식 2개 루트 50 삭제");
        Expect(bst.Remove(99), false, "없는 키 99 삭제");
        Expect(KeysOf(bst.InOrderTraversal()), "40 60 70 80", "삭제 후 중위 순회");
        Expect(KeysOf(bst.LevelOrderTraversal()), "60 40 70 80", "삭제 후 레벨 순회");

        Debug.Log("\n6. Clear:");
        bst.Clear();
        Expect(bst.Count, 0, "Clear 후 개수");

        Debug.Log("\n=== BinarySearchTree 테스트 완료 ===");
    }

    // 순회 결과를 "키 키 키" 문자열로 만든다
    string KeysOf(IEnumerable<KeyValuePair<int, string>> items)
    {
        return string.Join(" ", items.Select(kvp => kvp.Key));
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

    void ExpectThrows<TException>(Action action, string label) where TException : Exception
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
