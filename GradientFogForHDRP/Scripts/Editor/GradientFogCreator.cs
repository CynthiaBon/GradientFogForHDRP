using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GradientFogCreator
{
    [MenuItem("AmbianceTools/GradientFog")]
    public static void CreateColoredFog()
    {
        GameObject coloredFogPrefab = AssetDatabase.LoadAssetAtPath("Assets/GradientFogForHDRP/Prefabs/GradientFog.prefab", typeof(GameObject)) as GameObject;
        GameObject coloredFog = GameObject.Instantiate(coloredFogPrefab, Vector3.zero, Quaternion.identity);
        coloredFog.name = "GradientFog";
    }
}
