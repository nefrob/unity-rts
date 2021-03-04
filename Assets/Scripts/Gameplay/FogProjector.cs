/**
 * Ref: https://andrewhungblog.wordpress.com/2018/06/23/implementing-fog-of-war-in-unity/
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogProjector : MonoBehaviour
{
    [SerializeField] private Material projMaterial = null;
    [SerializeField] private float blendSpeed = 5;
    [SerializeField] private int textureScale = 2;
    private float blendAmount;

    [SerializeField] private RenderTexture visionTex = null;
    private RenderTexture prevTex;
    private RenderTexture curTex;
    private Projector projector;

    private void Awake ()
    {
        blendAmount = 0.0f;
        prevTex = GenerateTexture();
        curTex = GenerateTexture();

        // Setup projector material
        projector = GetComponent<Projector>();
        projector.enabled = true;
        projector.material = new Material(projMaterial);
        projector.material.SetTexture("_PrevTex", prevTex);
        projector.material.SetTexture("_CurTex", curTex);

        StartNewBlend();
    }

    RenderTexture GenerateTexture()
    {
        RenderTexture rt = new RenderTexture(
            visionTex.width * textureScale,
            visionTex.height * textureScale,
            0,
            visionTex.format) { filterMode = FilterMode.Bilinear };
        rt.antiAliasing = visionTex.antiAliasing;

        return rt;
    }

    public void StartNewBlend()
    {
        StopCoroutine(BlendFog());
        blendAmount = 0;
        
        // Swap the textures
        Graphics.Blit(curTex, prevTex);
        Graphics.Blit(visionTex, curTex);

        StartCoroutine(BlendFog());
    }

    IEnumerator BlendFog()
    {
        while (blendAmount < 1)
        {
            blendAmount += Time.deltaTime * blendSpeed;
            projector.material.SetFloat("_Blend", blendAmount);
            yield return null;
        }

        // Swap the textures and start a new blend
        StartNewBlend();
    }
}
