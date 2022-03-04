using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public Color originalColor;
    public Color hitColor;
    public Renderer meshRenderer;
    public float flashTime;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color;
    }

    public void CallFlash()
    {
       StartCoroutine(Flash());
    }
    public IEnumerator Flash()
    {
        meshRenderer.material.color = hitColor;

        yield return new WaitForSeconds(flashTime);

        meshRenderer.material.color = originalColor;
    }
}
