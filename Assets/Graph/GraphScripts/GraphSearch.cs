using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSearch
{
    private Graph graph;
    public List<GraphNode> path = new List<GraphNode>();

    public void Init(Graph graph)
    {
        this.graph = graph;
    }

    public void DFS(GraphNode node)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var stack = new Stack<GraphNode>();
        var visited = new HashSet<GraphNode>();

        stack.Push(node);
        visited.Add(node);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            path.Add(current);

            foreach (var adjacent in current.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                {
                    continue;
                }

                stack.Push(adjacent);
                visited.Add(adjacent);
            }
        }
    }
    public void BFS(GraphNode node)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var queue = new Queue<GraphNode>();
        var visited = new HashSet<GraphNode>();

        queue.Enqueue(node);
        visited.Add(node);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            path.Add(current);

            foreach (var adjacent in current.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                {
                    continue;
                }

                queue.Enqueue(adjacent);
                visited.Add(adjacent);
            }
        }
    }

    public void DFSRecursive(GraphNode node)
    {
        path.Clear();
        graph.ResetNodePrevious();

        DFSRecursive(node, new HashSet<GraphNode>());
    }

    protected void DFSRecursive(GraphNode node, HashSet<GraphNode> visited)
    {
        path.Add(node);
        visited.Add(node);

        foreach (var adjacent in node.adjacents)
        {
            if (!adjacent.CanVisit || visited.Contains(adjacent))
            {
                continue;
            }
            DFSRecursive(adjacent, visited);
        }
    }

    public bool PathFindingBFS(GraphNode startNode, GraphNode endNode)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var queue = new Queue<GraphNode>();
        var visited = new HashSet<GraphNode>();

        queue.Enqueue(startNode);
        visited.Add(startNode);

        bool success = false;
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current == endNode)
            {
                Debug.Log("endnode 찾음");
                success = true;
                Debug.Log($"endnode 찾고 밖으로 나갈거임 {success}");
                break;
            }

            foreach (var adjacent in current.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                {
                    continue;
                }

                adjacent.previous = current;

                visited.Add(adjacent);
                queue.Enqueue(adjacent);
            }
        }


        if (!success)
        {
            Debug.Log("endnode 못찾아서 실패찍을거임");
            return false;
        }

        GraphNode step = endNode;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }


        path.Reverse();
        return true;
    }

    public bool Dijkstra(GraphNode startNode, GraphNode endNode)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var visited = new HashSet<GraphNode>();
        var pQueue = new PriorityQueue<GraphNode, int>();
        var distances = new int[graph.nodes.Length];

        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = int.MaxValue;
        }

        distances[startNode.id] = 0;


        pQueue.Enqueue(startNode, distances[startNode.id]);

        bool success = false;

        while (pQueue.Count > 0)
        {
            var curNode = pQueue.Dequeue();

            if (visited.Contains(curNode))
                continue;


            if (curNode == endNode)
            {
                success = true;
                break;
            }

            visited.Add(curNode);


            foreach (var adjacent in curNode.adjacents)
            {

                if (!adjacent.CanVisit || visited.Contains(adjacent))
                    continue;

                var newDistance = distances[curNode.id] + adjacent.weight;

                if (distances[adjacent.id] > newDistance)
                {
                    distances[adjacent.id] = newDistance;
                    adjacent.previous = curNode;
                    pQueue.Enqueue(adjacent, newDistance);
                }
            }
        }

        // 경로 반환하는 로직 
        if (!success)
        {
            Debug.Log("endnode 못찾아서 실패찍을거임");
            return false;
        }

        GraphNode step = endNode;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }


        path.Reverse();
        return true;
    }

    // // 매개변수에 showFullSearch를 추가해 밖에서 원하는 모드를 선택할 수 있게 합니다.
    // public bool Dijkstra(GraphNode startNode, GraphNode endNode, bool showFullSearch = true)
    // {
    //     path.Clear();
    //     graph.ResetNodePrevious();

    //     var visited = new HashSet<GraphNode>();
    //     var pQueue = new PriorityQueue<GraphNode, int>();
    //     var distances = new int[graph.nodes.Length];

    //     // 1. 전체 탐색 순서를 담아둘 임시 리스트
    //     var fullSearchHistory = new List<GraphNode>();

    //     for (int i = 0; i < distances.Length; i++)
    //     {
    //         distances[i] = int.MaxValue;
    //     }

    //     distances[startNode.id] = 0;
    //     pQueue.Enqueue(startNode, distances[startNode.id]);

    //     bool success = false;
    //     while (pQueue.Count > 0)
    //     {
    //         var curNode = pQueue.Dequeue();

    //         if (visited.Contains(curNode))
    //             continue;

    //         // 2. 큐에서 꺼낼 때마다 방문 순서대로 임시 리스트에 차곡차곡 담습니다.
    //         fullSearchHistory.Add(curNode);

    //         if (curNode == endNode)
    //         {
    //             success = true;
    //             break;
    //         }

    //         visited.Add(curNode);

    //         foreach (var adjacent in curNode.adjacents)
    //         {
    //             if (!adjacent.CanVisit || visited.Contains(adjacent))
    //                 continue;

    //             var newDistance = distances[curNode.id] + adjacent.weight;
    //             if (distances[adjacent.id] > newDistance)
    //             {
    //                 distances[adjacent.id] = newDistance;
    //                 adjacent.previous = curNode;
    //                 pQueue.Enqueue(adjacent, newDistance);
    //             }
    //         }
    //     }

    //     if (!success)
    //     {
    //         Debug.Log("endnode 못찾아서 실패찍을거임");
    //         return false;
    //     }

    //     // 3. 말씀하신 대로 마지막에 외부 UI가 가져다 쓸 path에 무엇을 담을지 결정합니다.
    //     if (showFullSearch)
    //     {
    //         // 텔레포트(스위칭) 과정을 포함한 전체 탐색 궤적을 덮어씌움
    //         path.AddRange(fullSearchHistory);
    //     }
    //     else
    //     {
    //         // 기존 로직: 최적 경로만 역추적하여 담음
    //         GraphNode step = endNode;
    //         while (step != null)
    //         {
    //             path.Add(step);
    //             step = step.previous;
    //         }
    //         path.Reverse();
    //     }

    //     return true;
    // }

    public bool AStar(GraphNode startNode, GraphNode endNode)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var visited = new HashSet<GraphNode>();
        var pQueue = new PriorityQueue<GraphNode, int>();
        var distances = new int[graph.nodes.Length];

        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = int.MaxValue;
        }

        distances[startNode.id] = 0;


        pQueue.Enqueue(startNode, distances[startNode.id] + Heuristic(startNode, endNode));

        bool success = false;

        while (pQueue.Count > 0)
        {
            var curNode = pQueue.Dequeue();

            if (visited.Contains(curNode))
                continue;


            if (curNode == endNode)
            {
                success = true;
                break;
            }

            visited.Add(curNode);


            foreach (var adjacent in curNode.adjacents)
            {

                if (!adjacent.CanVisit || visited.Contains(adjacent))
                    continue;

                var newDistance = distances[curNode.id] + adjacent.weight;

                if (distances[adjacent.id] > newDistance)
                {
                    distances[adjacent.id] = newDistance;
                    adjacent.previous = curNode;
                    pQueue.Enqueue(adjacent, newDistance + Heuristic(adjacent, endNode));
                }
            }
        }

        // 경로 반환하는 로직 
        if (!success)
        {
            Debug.Log("endnode 못찾아서 실패찍을거임");
            return false;
        }

        GraphNode step = endNode;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }


        path.Reverse();
        return true;
    }

    private int Heuristic(GraphNode a, GraphNode b)
    {
        int ax = a.id % graph.cols;
        int ay = a.id / graph.cols;

        int bx = b.id % graph.cols;
        int by = b.id / graph.cols;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }
}
