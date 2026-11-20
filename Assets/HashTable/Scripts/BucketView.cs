using System.Collections.Generic;

public enum BucketState
{
    Empty,
    Occupied,
    Deleted,
}
public readonly struct BucketView
{
    public int Index { get; }
    public BucketState State { get; }
    public string Key { get; }
    public string Value { get; }
    public IReadOnlyList<(string Key, string Value)> Chain { get; }
    public BucketView(int index, BucketState state, string key, string value, IReadOnlyList<(string Key, string Value)> chain)
    {
        Index = index;
        State = state;
        Key = key;
        Value = value;
        Chain = chain;
    }
}
