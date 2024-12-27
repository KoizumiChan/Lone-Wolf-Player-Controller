using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 5f; // Maximum distance the player can interact from
    public TextMeshProUGUI floatingText; // Reference to your TextMeshProUGUI component
    private Camera playerCamera; // Camera reference

    private Vector3 targetPosition; // The target position for the floating text
    public float textSmoothTime = 0.1f; // Time it takes for the text to reach the target position
    private bool isLookingAtObject = false; // Whether the player is currently looking at an object

    private void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        float radius = 0.5f; // This is the radius of the sphere being cast. Adjust as needed.

        // Calculate the starting point of the raycast
        Vector3 raycastOrigin = transform.position - (radius * playerCamera.transform.forward);

        // Cast a sphere forward from the raycastOrigin but in the direction of the camera
        Ray interactionRay = new Ray(raycastOrigin, playerCamera.transform.forward);
        RaycastHit hit;

        // Create a layer mask that includes only the Interactable layer
        int layerMask = LayerMask.GetMask("Interactable");

        // If the sphere hits something within the interaction range
        if (Physics.SphereCast(interactionRay, radius, out hit, interactionRange, layerMask))
        {
            // Log a message with the name of the hit object
            //Debug.Log("SphereCast has hit: " + hit.collider.transform.parent.gameObject.name);

            // Update the target position and content
            Interactable interactable = hit.collider.transform.parent.GetComponent<Interactable>();
            description interactableDesc = hit.collider.transform.parent.GetComponent<description>();
            targetPosition = Camera.main.WorldToScreenPoint(hit.point);
            floatingText.text = interactableDesc.objectName;

            // If the player just started looking at an object, initialize the text at the target position
            if (!isLookingAtObject)
            {
                floatingText.transform.position = targetPosition;
                isLookingAtObject = true;
            }
        }
        else
        {
            // If the sphere doesn't hit anything, hide the text and reset isLookingAtObject
            floatingText.text = "";
            isLookingAtObject = false;
        }

        // Smoothly move the floating text towards the target position if the player is looking at an object
        if (isLookingAtObject)
        {
            floatingText.transform.position = Vector3.Lerp(floatingText.transform.position, targetPosition, textSmoothTime);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            // If the sphere hits something within the interaction range
            if (Physics.SphereCast(interactionRay, radius, out hit, interactionRange, layerMask))
            {
                // Check if the hit object has an Interactable component
                Interactable interactable = hit.collider.transform.parent.gameObject.GetComponent<Interactable>();
                if (interactable != null)
                {
                    // If it does, call the Interact function
                    interactable.Interact(GetComponent<PlayerInput>());
                }
                Health healthScript = hit.collider.transform.parent.gameObject.GetComponent<Health>();
                if (healthScript != null)
                {
                    // If it does, call the Interact function
                    healthScript.Interact(GetComponent<PlayerInput>());
                }
            }
        }
    }
}