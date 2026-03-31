using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data;

internal class LoopingArray<T> : IEnumerable<T>
{
    private readonly T[] array;

    public int Length => array.Length;

    public LoopingArray(IEnumerable<T> src)
    {
        if (src == null) throw new ArgumentNullException(nameof(src));
        array = [.. src]; // what are these new fangled features

        //if (array.Length == 0) throw new ArgumentException("Source collection cannot be empty.", nameof(src));
    }

    public T this[int index]
    {
        get => array[NormalizeIndex(index)];
        set => array[NormalizeIndex(index)] = value;
    }

    private int NormalizeIndex(int index)
    {
        int modIndex = index % array.Length;
        return modIndex < 0 ? modIndex + array.Length : modIndex;
    }
    public IEnumerator<T> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
