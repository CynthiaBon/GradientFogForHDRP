using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class GradientFogManager : MonoBehaviour
{
    [SerializeField] [Min(0.1f)] private float _fogTransitionTime = 0.1f;

    [Header("DO NOT MODIFY")] [SerializeField] private GradientsData _gradients = null;
    public static GradientFogManager Instance { get; private set; } = null;

    private DrawRenderersCustomPass _drawRenderersCustomPass = null;

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        Instance = this;
        SetCustomPass();
    }

    public void ChangeGradient(string gradientName)
    {
        Material newGradientMaterial = _gradients.GetMaterial(gradientName);
        if (newGradientMaterial != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                SetCustomPass();
#endif
            if (!Application.isPlaying)
                _drawRenderersCustomPass.overrideMaterial = newGradientMaterial;
            else
                StartCoroutine("SmoothChange", gradientName);
        }
    }

    private IEnumerator SmoothChange(string nextGradientName)
    {
        Material nextGradientMaterial = _gradients.GetMaterial(nextGradientName);

        if (nextGradientMaterial == null)
            yield break;

        GetCurrentFogMaterial().SetTexture("_NextFogGradient", nextGradientMaterial.GetTexture("_FogGradient"));

        float timer = 0f;
        while (timer / _fogTransitionTime < 0.99f)
        {
            timer = timer + Time.deltaTime;
            GetCurrentFogMaterial().SetFloat("_LerpCursor", timer / _fogTransitionTime);
            yield return null;
        }
        GetCurrentFogMaterial().SetFloat("_LerpCursor", -1f);
        GetCurrentFogMaterial().SetTexture("_NextFogGradient", null);
        _drawRenderersCustomPass.overrideMaterial = nextGradientMaterial;
    }

    private void SetCustomPass()
    {
        CustomPassVolume customPassVolume = GetComponent<CustomPassVolume>();
        _drawRenderersCustomPass = customPassVolume.customPasses[0] as DrawRenderersCustomPass;
    }

    public Material GetCurrentFogMaterial()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            SetCustomPass();
#endif

        return _drawRenderersCustomPass.overrideMaterial;
    }
}
