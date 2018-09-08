using System;
using System.Collections.Generic;
using UnityEngine;

public class Utilities {

    public static List<T> OrderObjsByName<T>(T[] unorderedObjs) where T : MonoBehaviour {
        try {
            List<T> list = new List<T>(unorderedObjs.Length);
            foreach (T obj in unorderedObjs) {
                string name = obj.name;
                string stringIdx = name.Substring(name.LastIndexOf("_") + 1);
                int idx = Int32.Parse(stringIdx);
                //int idx = Int32.Parse(name.Substring(name.LastIndexOf("_")));
                list[idx] = obj;
            }
            return list;
        } catch (Exception e) {
            return null;
        }
    }

    public static int ConvertStringOrDefault(string arg, int defaulVal) {
        try {
            return int.Parse(arg);
        } catch (Exception e) {
            return defaulVal;
        }
    }

    public static float ConvertStringOrDefault(string arg, float defaulVal) {
        try {
            return float.Parse(arg);
        } catch (Exception e) {
            return defaulVal;
        }
    }

    public static bool ConvertStringOrDefault(string arg, bool defaulVal) {
        try {
            return bool.Parse(arg);
        } catch (Exception e) {
            return defaulVal;
        }
    }

}
