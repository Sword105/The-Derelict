using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;


public class CrosshairController : MonoBehaviour
{

    // attach script to main camera under the player object

    [SerializeField] private Image crosshair; // Assign in Inspector
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color interactColor = Color.red;
    [SerializeField] private float rayDistance = 5f;

    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main; // Reference to main camera
        crosshair.color = defaultColor;
        
    }

    // Update is called once per frame
    void Update()
    {
        // Shoot a ray from the camera forward
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, rayDistance))
        {
            // Check if the raycast has hit the "Interactable" tag
            if (hit.collider.CompareTag("Interactable"))
            {
                // if yes,change the color to red
                crosshair.color = interactColor;
                return;
            }

            // If raycast does not hit an interactable object, reset to default color
            crosshair.color = defaultColor;
        }
    }
}