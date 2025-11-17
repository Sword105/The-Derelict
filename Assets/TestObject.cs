using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneColorChanger : MonoBehaviour
{
    public Color newColor = Color.red; // Expose a color variable in the Inspector

    void Start()
    {
        // Get the Renderer component of the GameObject
        Renderer planeRenderer = GetComponent<Renderer>();

        // Check if the Renderer exists and has a material
        if (planeRenderer != null && planeRenderer.material != null)
        {
            // Assign the new color to the material's main color (albedo)
            planeRenderer.material.color = newColor;
        }
    }
}
