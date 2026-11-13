using TMPro;
using UnityEngine;

public class NodeVisualizer : MonoBehaviour
{
    public SpriteRenderer nodeRenderer;
    public TextMeshPro nodeText;
    public LineRenderer leftLineRenderer;
    public LineRenderer rightLineRenderer;

    public void SetNode<TKey, TValue>(TreeNode<TKey, TValue> node)
    {
        nodeText.text = $"K: {node.Key}\nV: {node.Value}\nH: {node.Height}";
    }

    public void SetEdge(LineRenderer line, Vector3 target)
    {
        Vector3 back = Vector3.forward * 0.1f;
        line.positionCount = 2;
        line.SetPosition(0, transform.position + back);
        line.SetPosition(1, target + back);
    }
}
