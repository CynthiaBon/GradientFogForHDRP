using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GradientDictionary 
{
    public string GradientName = null;
    public Material GradientMaterial = null;

    public GradientDictionary(string gradientName, Material gradientMaterial)
    {
        GradientName = gradientName;
        GradientMaterial = gradientMaterial;
    }
}
