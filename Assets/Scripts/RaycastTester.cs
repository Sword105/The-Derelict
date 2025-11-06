using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTester : MonoBehaviour
{
    void Update()
    {
        if (Camera.main == null)
        {
            Debug.LogWarning("No MainCamera found!");
            return;
        }

        // Draw a red ray forward from the camera
        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;
        Debug.DrawRay(origin, direction * 10f, Color.red);

        // Perform a basic raycast with no layer mask
        if (Physics.Raycast(origin, direction, out RaycastHit hit, 10f))
        {
            Debug.Log("Ray hit: " + hit.collider.name);
        }
        else
        {
            Debug.Log("Ray hit nothing.");
        }
    }
}
