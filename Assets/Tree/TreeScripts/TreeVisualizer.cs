using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework.Interfaces;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;

public enum SpacingTypes
{
    PoW,
    LevelOrder,
    InOrder,
}

public class TreeVisualizer : MonoBehaviour
{
    public NodeVisualizer nodePrefab;

    public SpacingTypes spacingType = SpacingTypes.PoW;
    public float verticalSpacing = 2f;
    public float horizontalSpacing = 2f;

    private readonly Dictionary<object, Vector3> nodePositions = new Dictionary<object, Vector3>();
    private Vector3 origin;

    public void Visualize<TKey, TValue>(TreeNode<TKey, TValue> root)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        nodePositions.Clear();

        if (root == null)
            return;

        switch (spacingType)
        {
            case SpacingTypes.PoW:
                AssignPositionsPow(
                    root,
                    Vector3.zero,
                    horizontalSpacing * Mathf.Pow(2, root.Height - 3)
                );
                break;
            case SpacingTypes.InOrder:
                {
                    int xIndex = 0;
                    AssginPositionsInOrder(root, 0, ref xIndex);
                }

                break;

            case SpacingTypes.LevelOrder:
                AssginPositionsLevelOrder(root);
                break;
        }

        origin = transform.position - nodePositions[root];
        InstantiateSubTree(root);
    }

    private Vector3 PositionOf(object node)
    {
        return origin + nodePositions[node];
    }

    private void InstantiateSubTree<TKey, TValue>(TreeNode<TKey, TValue> node)
    {
        if (node == null)
            return;

        var view = Instantiate(nodePrefab, PositionOf(node), Quaternion.identity, transform);
        view.SetNode(node);
        if (node.Left != null)
        {
            view.SetEdge(view.leftLineRenderer, PositionOf(node.Left));
        }
        else
        {
            view.leftLineRenderer.enabled = false;
        }

        if (node.Right != null)
        {
            view.SetEdge(view.rightLineRenderer, PositionOf(node.Right));
        }
        else
        {
            view.rightLineRenderer.enabled = false;
        }

        InstantiateSubTree(node.Left);
        InstantiateSubTree(node.Right);
    }

    private void AssignPositionsPow<TKey, TValue>(
        TreeNode<TKey, TValue> node,
        Vector3 position,
        float offset
    )
    {
        if (node == null)
            return;
        nodePositions[node] = position;

        Vector3 below = position + Vector3.down * verticalSpacing;
        AssignPositionsPow(node.Left, below + Vector3.left * offset, offset * 0.5f);
        AssignPositionsPow(node.Right, below + Vector3.right * offset, offset * 0.5f);
    }

    private void AssginPositionsLevelOrder<TKey, TValue>(TreeNode<TKey, TValue> root)
    {
        var queue = new Queue<TreeNode<TKey, TValue>>();
        queue.Enqueue(root);

        for (int depth = 0; queue.Count > 0; depth++)
        {
            int count = queue.Count;
            for (int i = 0; i < count; ++i)
            {
                var node = queue.Dequeue();
                float x = (i - (count - 1) * 0.5f) * horizontalSpacing;
                nodePositions[node] = new Vector3(x, -depth * verticalSpacing, 0f);

                if (node.Left != null)
                {
                    queue.Enqueue(node.Left);
                }
                if (node.Right != null)
                {
                    queue.Enqueue(node.Right);
                }
            }
        }
    }

    private void AssginPositionsInOrder<TKey, TValue>(
        TreeNode<TKey, TValue> node,
        int depth,
        ref int xIndex
        )
    {
        if (node == null)
            return;

        AssginPositionsInOrder(node.Left, depth + 1, ref xIndex);
        nodePositions[node] = new Vector3(xIndex * horizontalSpacing, -depth * verticalSpacing, 0f);
        xIndex++;
        AssginPositionsInOrder(node.Right, depth + 1, ref xIndex);
    }
}
