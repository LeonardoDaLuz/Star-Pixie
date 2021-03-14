using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollTexture : MonoBehaviour {
    public int materialIndex = 0;
    public Vector2 uvAnimationRate = new Vector2(1.0f, 0.0f);
    public string textureName = "_MainTex";
    Vector2 uvOffset = Vector2.zero;
    Image renderer;
    void Start()
    {
        renderer = GetComponent<Image>();
    }
    void LateUpdate()
    {
        uvOffset += (uvAnimationRate * Time.deltaTime);
        if (renderer.enabled)
        {
            renderer.material.SetTextureOffset(textureName, uvOffset);
        }
    }
}
