using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[CustomEditor(typeof(GradientFogManager))]
public class GradientFogManagerEditor : Editor
{
    private GradientFogManager _self = null;

    //Editor Variable
    private static string _gradientName = null;
    private static Gradient _fogGradient = null;
    private static string _usedGradientName = null;

    //Script variable
    private GradientsData _gradientData = null;
    private Vector2 _scrollPosition = Vector2.zero;

    private void OnEnable()
    {
        _self = target as GradientFogManager;

        _fogGradient = new Gradient();
        _gradientData = AssetDatabase.LoadAssetAtPath("Assets/GradientFogForHDRP/GradientMaterials/GradientsData.asset", typeof(GradientsData)) as GradientsData;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        GradientCreationDisplay();
        DrawLine();
        ChangeUsedGradient();
        DrawLine();
        GradientMaterialsDisplay();
    }

    #region Creation

    private void GradientCreationDisplay()
    {
        _gradientName = EditorGUILayout.TextField("Gradient name", _gradientName);
        _fogGradient = EditorGUILayout.GradientField("Fog gradient", _fogGradient);

        if (GUILayout.Button("Create or replace material from gradient") && _gradientName != "" && _gradientName != null)
            CreateGradientMaterial();
        if (GUILayout.Button("Clear data") && _gradientData != null && _gradientData.Data != null)
            ClearData();
    }

    private void CreateGradientMaterial()
    {
        string gradientTexturePath = "/GradientFogForHDRP/GradientMaterials/Textures/" + "T_" + _gradientName + ".png";
        string gradientMaterialPath = "Assets/GradientFogForHDRP/GradientMaterials/Materials/" + "M_" + _gradientName + ".mat";

        Texture2D gradientTexture = GetTextureFromGradient();
        File.WriteAllBytes(Application.dataPath + gradientTexturePath, gradientTexture.EncodeToPNG());

        if (!_gradientData.Contains(_gradientName))
        {
            Material newGradientMaterial = new Material(Shader.Find("Renderers/GradientFog"));
            AssetDatabase.CreateAsset(newGradientMaterial, gradientMaterialPath);
        }

        AssetDatabase.Refresh();

        gradientTexture = AssetDatabase.LoadAssetAtPath("Assets" + gradientTexturePath, typeof(Texture2D)) as Texture2D;
        Material gradientMaterial = AssetDatabase.LoadAssetAtPath(gradientMaterialPath, typeof(Material)) as Material;

        gradientMaterial.SetTexture("_FogGradient", gradientTexture);

        if (!_gradientData.Contains(_gradientName))
            FillData(gradientMaterial);
    }

    private Texture2D GetTextureFromGradient()
    {
        Texture2D gradientTexture = new Texture2D(100, 1);
        for (int i = 0; i < 100; i++)
        {
            gradientTexture.SetPixel(i, 0, _fogGradient.Evaluate((float)i / 100.0f));
        }
        gradientTexture.wrapMode = TextureWrapMode.Clamp;
        gradientTexture.alphaIsTransparency = true;

        gradientTexture.Apply();
        return gradientTexture;
    }

    private void FillData(Material gradientMaterial)
    {
        if (_gradientData == null)
            return;

        if (_gradientData.Data == null)
            _gradientData.Data = new List<GradientDictionary>();

        _gradientData.Data.Add(new GradientDictionary(_gradientName, gradientMaterial));
        EditorUtility.SetDirty(_gradientData);
        AssetDatabase.Refresh();
    }

    #endregion Creation

    #region Data

    private void ChangeUsedGradient()
    {
        GUILayout.Label(_self.GetCurrentFogMaterial() != null ? "Current fog gradient: " + _self.GetCurrentFogMaterial().name : "No gradient selected");
        _usedGradientName = EditorGUILayout.TextField("Gradient to use", _usedGradientName);
        if (GUILayout.Button("Change gradient"))
        {
            _self.ChangeGradient(_usedGradientName);
            EditorUtility.SetDirty(_self.gameObject);
        }
    }

    private void GradientMaterialsDisplay()
    {
        if (_gradientData == null || _gradientData.Data == null || _gradientData.Data.Count == 0)
            return;

        GUILayout.BeginVertical();
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true, GUILayout.ExpandHeight(true));
        for (int i = 0; i < _gradientData.Data.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            _gradientData.Data[i].GradientName = EditorGUILayout.TextField(_gradientData.Data[i].GradientName);
            GUILayout.Label(_gradientData.Data[i].GradientMaterial.GetTexture("_FogGradient"));
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void ClearData()
    {
        for (int i = 0; i < _gradientData.Data.Count; i++)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_gradientData.Data[i].GradientMaterial.GetTexture("_FogGradient")));
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_gradientData.Data[i].GradientMaterial));
        }

        _gradientData.Data.Clear();
    }

    #endregion Data

    private void DrawLine()
    {
        GUILayout.Label("");
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("");
    }
}
