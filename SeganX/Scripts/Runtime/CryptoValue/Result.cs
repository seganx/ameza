using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Result
{
    public int[] i = new int[8];
    public float[] f = new float[8];
    public bool[] b = new bool[8];

    private Result()
    {
        for (int k = 0; k < 8; k++)
        {
            i[k] = Random.Range(-1000, 1000);
            f[k] = Random.Range(-1000, 1000.0f);
            b[k] = Random.Range(-1000, 1000.0f) > 0;
        }
    }

    public static Result SetInteger(int index, int value)
    {
        result.i[index] = value;
        return result;
    }

    public static Result SetFloat(int index, float value)
    {
        result.f[index] = value;
        return result;
    }

    public static Result SetBool(int index, bool value)
    {
        result.b[index] = value;
        return result;
    }

    //////////////////////////////////////////////////////
    /// STATIC MEMBERS
    //////////////////////////////////////////////////////
    private static readonly Result result = new Result();
}
