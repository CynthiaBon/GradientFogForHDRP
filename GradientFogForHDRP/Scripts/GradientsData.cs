using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GradientsData", menuName = "AmbianceTools/GradientsData")]
public class GradientsData : ScriptableObject
{
    public List<GradientDictionary> Data = null;

    public bool Contains(string gradientName)
    {
        GradientDictionary gradientData = Data.Find((data => data.GradientName == gradientName));
        return gradientData != null;
    }

    public Material GetMaterial(string gradientName)
    {
        GradientDictionary gradientData = Data.Find((data => data.GradientName == gradientName));
        if (gradientData != null)
            return gradientData.GradientMaterial;
        Debug.LogWarning("Gradient not found");
        return null;
    }
}