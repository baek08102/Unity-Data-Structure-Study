using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class GraphTest : MonoBehaviour
{

    public enum Algorithm
    {
        DFS,
        BFS,
        DFSRecursive,
        PathFindingBFS,
        Dijkstra,
        AStar,
    }

    public UiGraphNode nodePrefab;
    public List<UiGraphNode> uiNodes;
    public Transform uiNodeRoot;

    private Graph graph;
    public Algorithm algorithm;
    public int startIdx;
    public int endIdx;

    private void Start()
    {
        int[,] map = new int[5, 5]
        {
            {1,1,1,3,1},
            {1,1,2,1,1},
            {1,0,1,1,4},
            {1,1,1,2,1},
            {1,2,1,4,0}
        };

        graph = new Graph();
        graph.Init(map);

        InitUiNode(graph);
    }

    private void InitUiNode(Graph graph)
    {
        foreach (var node in graph.nodes)
        {
            var uiNode = Instantiate(nodePrefab, uiNodeRoot);
            uiNode.SetNode(node);
            uiNode.Reset();

            uiNodes.Add(uiNode);
        }
    }

    private void ResetUiNodes()
    {
        foreach (var uiNode in uiNodes)
        {
            uiNode.Reset();
        }
    }

    [ContextMenu("Search")]
    public void Search()
    {
        if (graph == null)
        {
            return;
        }

        var search = new GraphSearch();
        search.Init(graph);

        switch (algorithm)
        {
            case Algorithm.DFS:
                search.DFS(graph.nodes[startIdx]);
                break;
            case Algorithm.BFS:
                search.BFS(graph.nodes[startIdx]);
                break;
            case Algorithm.DFSRecursive:
                search.DFSRecursive(graph.nodes[startIdx]);
                break;
            case Algorithm.PathFindingBFS:
                search.PathFindingBFS(graph.nodes[startIdx], graph.nodes[endIdx]);
                break;
            case Algorithm.Dijkstra:
                search.Dijkstra(graph.nodes[startIdx], graph.nodes[endIdx]);
                break;
            case Algorithm.AStar:
                search.AStar(graph.nodes[startIdx], graph.nodes[endIdx]);
                break;
        }

        ResetUiNodes();

        for (int i = 0; i < search.path.Count; i++)
        {
            var node = search.path[i];
            float t = search.path.Count > 1 ? (float)i / (search.path.Count - 1) : 0f;
            var color = Color.Lerp(Color.red, Color.green, t);
            uiNodes[node.id].SetColor(color);
            uiNodes[node.id].SetText($"Id: {node.id}\nWeight: {node.weight}\n방문: {i}");
        }
    }


}
