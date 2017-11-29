struct ValueTuple
{
    public static ValueTuple<T1, T2> Create<T1, T2>(T1 i1, T2 i2) => new ValueTuple<T1, T2>(i1, i2);
}

struct ValueTuple<T1, T2>
{
    public ValueTuple(T1 i1, T2 i2)
    {
        Item1 = i1;
        Item2 = i2;
    }

    public T1 Item1 { get; set; }
    public T2 Item2 { get; set; }
}