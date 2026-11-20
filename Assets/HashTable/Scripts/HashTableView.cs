using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum HashTableTypes
{
    Simple,
    Chaining,
    OpenAddressing,
}

public enum ProbingStrategies
{
    Liner,
    Quadratic,
    DoubleHash,
}

public class HashTableView : MonoBehaviour
{
    public Transform bucketRoot;
    public HashTableBucketView bucketPrefab;
    public TMP_Dropdown hashtableTyepDropdown;
    public TMP_Dropdown probingStrategyDropdown;
    public TMP_InputField keyInputFiled;
    public TMP_InputField valueInputFiled;
    public Button addButton;
    public Button removeButton;
    public Button clearButton;
    public TMP_Text logText;
    public ScrollRect logScrollRect;

    private HashTableTypes currentHashTableType = HashTableTypes.Simple;
    private ProbingStrategies currentProbingStrategy = ProbingStrategies.Liner;

    private SimpleHashTable<string, int> simpleHashTable;
    private ChainingHashTable<string, string> chainingHashTable;
    private OpenAddressingHashTable<string, string> openAddressingHashTable;

    // 리니어
    // 오픈어드레싱

    private List<HashTableBucketView> bucketViews = new List<HashTableBucketView>();

    private void Start()
    {
        hashtableTyepDropdown.onValueChanged.AddListener(OnHashTableTypeChanged);
        probingStrategyDropdown.onValueChanged.AddListener(OnProbingStarategyChaned);
        addButton.onClick.AddListener(OnAddButtonClicked);
        removeButton.onClick.AddListener(OnRemoveButtonClicked);
        clearButton.onClick.AddListener(OnClearButtonClicked);

        probingStrategyDropdown.interactable =
            currentHashTableType == HashTableTypes.OpenAddressing;

        InitHashTable();
        UpdateBucketViews();
    }

    private void InitHashTable()
    {
        switch (currentHashTableType)
        {
            case HashTableTypes.Simple:
                simpleHashTable = new SimpleHashTable<string, int>();
                break;
            case HashTableTypes.Chaining:
                chainingHashTable = new ChainingHashTable<string, string>();
                break;
            case HashTableTypes.OpenAddressing:
                openAddressingHashTable = new OpenAddressingHashTable<string, string>();
                break;
        }

        CreateBucketViews();
    }

    private int GetCapacity()
    {
        switch (currentHashTableType)
        {
            case HashTableTypes.Simple:
                return simpleHashTable.Capacity;
            case HashTableTypes.Chaining:
                return chainingHashTable.Capacity;
            case HashTableTypes.OpenAddressing:
                return openAddressingHashTable.Capacity;
        }
        return 0;
    }

    private void CreateBucketViews()
    {
        foreach (var bucket in bucketViews)
        {
            if (bucket != null)
            {
                Destroy(bucket.gameObject);
            }
        }
        bucketViews.Clear();

        int capacity = GetCapacity();
        for (int i = 0; i < capacity; ++i)
        {
            var bucketView = Instantiate(bucketPrefab, bucketRoot);
            bucketView.SetIndex(i);
            bucketView.SetEmpty();
            bucketViews.Add(bucketView);
        }
    }

    private IEnumerable<BucketView> GetBucketViews()
    {
        switch (currentHashTableType)
        {
            case HashTableTypes.Simple:
                return simpleHashTable.GetBucketViews();
            case HashTableTypes.Chaining:
                return chainingHashTable.GetBucketViews();
            case HashTableTypes.OpenAddressing:
                return openAddressingHashTable.GetBucketViews();
        }
        return Array.Empty<BucketView>();
    }

    private void UpdateBucketViews()
    {
        int capacity = GetCapacity();
        if (capacity != bucketViews.Count)
        {
            AddLog($"리사이즈 감지: {bucketViews.Count} -> {capacity}");
            CreateBucketViews();
        }

        var views = GetBucketViews();
        foreach (var view in views)
        {
            var bucketView = bucketViews[view.Index];
            switch (view.State)
            {
                case BucketState.Empty:
                    bucketView.SetEmpty();
                    break;
                case BucketState.Occupied:
                    if (view.Chain != null)
                    {
                        var items = new List<string>();
                        foreach (var entry in view.Chain)
                        {
                            items.Add($"K: {entry.Key} V: {entry.Value}");
                        }
                        bucketView.SetChainedItem(items);
                    }
                    else
                    {
                        bucketView.SetOccupied(view.Key, view.Value);
                    }
                    break;
                case BucketState.Deleted:
                    bucketView.SetDeleted();
                    break;
            }
        }
    }

    private List<string> logMessages = new List<string>();

    private void AddLog(string message)
    {
        logMessages.Insert(0, message);

        if (logMessages.Count > 20)
        {
            logMessages.RemoveAt(logMessages.Count - 1);
        }

        logText.text = string.Join("\n", logMessages);
    }

    private void OnHashTableTypeChanged(int index)
    {
        currentHashTableType = (HashTableTypes)index;

        probingStrategyDropdown.interactable =
            currentHashTableType == HashTableTypes.OpenAddressing;

        InitHashTable();
        UpdateBucketViews();
        AddLog($"해시 테이블 변경: {currentHashTableType}");
    }

    private void OnProbingStarategyChaned(int index)
    {
        currentProbingStrategy = (ProbingStrategies)index;
        if (currentHashTableType == HashTableTypes.OpenAddressing)
        {
            InitHashTable();
            UpdateBucketViews();
            AddLog($"충돌 해결 전략 변경 : {currentProbingStrategy}");
        }
    }

    private void OnAddButtonClicked()
    {
        if (string.IsNullOrEmpty(keyInputFiled.text))
        {
            keyInputFiled.text = UnityEngine.Random.Range(-99999, 99999).ToString();
        }

        if (string.IsNullOrEmpty(valueInputFiled.text))
        {
            valueInputFiled.text = UnityEngine.Random.Range(-99999, 99999).ToString();
        }

        string key = valueInputFiled.text;
        if (int.TryParse(valueInputFiled.text, out int value))
        {
            try
            {
                switch (currentHashTableType)
                {
                    case HashTableTypes.Simple:
                        simpleHashTable.Add(key, value);
                        break;
                    case HashTableTypes.Chaining:
                        chainingHashTable.Add(key, value.ToString());
                        break;
                    case HashTableTypes.OpenAddressing:
                        openAddressingHashTable.Add(key, value.ToString());
                        break;
                }

                AddLog($"ADD: {key}, {value}");
                UpdateBucketViews();
                keyInputFiled.text = string.Empty;
                valueInputFiled.text = string.Empty;
            }
            catch (Exception e)
            {
                AddLog($"ADD 실패: {key}, {value} {e.Message}");
                keyInputFiled.text = string.Empty;
                valueInputFiled.text = string.Empty;
            }
        }
        else
        {
            AddLog("ADD 실패: 유효한 숫자를 입력하세요.");
        }
    }

    private void OnRemoveButtonClicked()
    {
        string key = keyInputFiled.text;
        bool removed = false;
        switch (currentHashTableType)
        {
            case HashTableTypes.Simple:
                removed = simpleHashTable.Remove(key);
                break;
            case HashTableTypes.Chaining:
                removed = chainingHashTable.Remove(key);
                break;
            case HashTableTypes.OpenAddressing:
                removed = openAddressingHashTable.Remove(key);
                break;
        }

        if (removed)
        {
            AddLog($"REMOVE: {key} (성공)");
            UpdateBucketViews();
            keyInputFiled.text = string.Empty;
            valueInputFiled.text = string.Empty;
        }
        else
        {
            AddLog($"REMOVE: {key} (키를 찾을수 없음)");
        }
    }

    private void OnClearButtonClicked()
    {
        switch (currentHashTableType)
        {
            case HashTableTypes.Simple:
                simpleHashTable.Clear();
                break;
            case HashTableTypes.Chaining:
                chainingHashTable.Clear();
                break;
            case HashTableTypes.OpenAddressing:
                openAddressingHashTable.Clear();
                break;
        }
        AddLog("CLEAR");
        UpdateBucketViews();
    }
}
