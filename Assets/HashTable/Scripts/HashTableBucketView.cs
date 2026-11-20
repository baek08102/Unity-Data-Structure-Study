using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HashTableBucketView : MonoBehaviour
{
    public Image backgroundImage;
    public TMP_Text indexText;
    public TMP_Text contentText;

    private int bucketIndex;

    private static readonly Color EmptyColor = Color.grey;
    private static readonly Color OccupiedColor = Color.green;
    private static readonly Color DeletdColor = Color.orange;

    public void SetIndex(int index)
    {
        bucketIndex = index;
        indexText.text = $"I: {bucketIndex}";

    }

    public void SetEmpty()
    {
        backgroundImage.color = EmptyColor;
        contentText.text = string.Empty;
    }

    public void SetOccupied(string key, string value)
    {
        backgroundImage.color = OccupiedColor;
        contentText.text = $"K: {key}\tV: {value}";
    }

    public void SetDeleted()
    {
        backgroundImage.color = DeletdColor;
        contentText.text = "(deleted)";
    }

    public void SetChainedItem(List<string> items)
    {
        if (items == null || items.Count == 0)
        {
            SetEmpty();
            return;
        }
        backgroundImage.color = OccupiedColor;
        contentText.text = string.Join(" -> ", items);
    }
}
