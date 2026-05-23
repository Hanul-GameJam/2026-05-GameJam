using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ScrollBackground : MonoBehaviour
{
    public float scrollSpeed;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        float offset = Time.time * scrollSpeed;

        rend.material.mainTextureOffset = new Vector2(offset, 0);
    }
}
