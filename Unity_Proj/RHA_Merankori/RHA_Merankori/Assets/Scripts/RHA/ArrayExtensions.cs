using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtensions
{
    public static T[] Add<T>(this T[] array, T item)
    {
        if (array == null)
            return new T[] { item };

        int len = array.Length;
        T[] result = new T[len + 1];
        Array.Copy(array, result, len);
        result[len] = item;
        return result;
    }
}
