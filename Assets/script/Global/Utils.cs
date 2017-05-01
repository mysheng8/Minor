using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils 
{
    public static string PrintList<T>(List<T> list)
    {
        string text = "";
        foreach (T i in list)
        {
            text += i.ToString();
        }
        return text;
    }
}
