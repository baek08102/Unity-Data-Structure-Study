using System.Collections.Generic;
using JetBrains.Annotations;
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
        Debug.Log(node);
        if (node == null)
            return;
        nodePositions[node] = position;

        Vector3 below = position + Vector3.down * verticalSpacing;
        AssignPositionsPow(node.Left, below + Vector3.left * offset, offset * 0.5f);
        AssignPositionsPow(node.Right, below + Vector3.right * offset, offset * 0.5f);
    }
}
