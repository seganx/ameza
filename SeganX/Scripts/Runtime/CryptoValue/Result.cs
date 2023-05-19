using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntResult
{
    public int value = 0;

    //////////////////////////////////////////////////////
    /// STATIC MEMBERS
    //////////////////////////////////////////////////////
    private static readonly IntResult result = new IntResult();

    public static IntResult Set(int value)
    {
        result.value = value;
        return result;
    }
}

public class FloatResult
{
    public float value = 0;

    //////////////////////////////////////////////////////
    /// STATIC MEMBERS
    //////////////////////////////////////////////////////
    private static readonly FloatResult result = new FloatResult();

    public static FloatResult Set(float value)
    {
        result.value = value;
        return result;
    }
}

public class BoolResult
{
    public bool value = false;

    //////////////////////////////////////////////////////
    /// STATIC MEMBERS
    //////////////////////////////////////////////////////
    private static readonly BoolResult result = new BoolResult();

    public static BoolResult Set(bool value)
    {
        result.value = value;
        return result;
    }
}


public class LongResult
{
    public long value = 0;

    //////////////////////////////////////////////////////
    /// STATIC MEMBERS
    //////////////////////////////////////////////////////
    private static readonly LongResult result = new LongResult();

    public static LongResult Set(long value)
    {
        result.value = value;
        return result;
    }
}
