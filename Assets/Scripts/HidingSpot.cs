using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    private bool playerInRange = false;
    private bool isHiding = false;
    private GameObject player;
    private Vector3 originalPosition;

    public Transform hidePosition; // Set this in Inspector to where the player should hide

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (isHiding)
            {
                UnhidePlayer();
            }
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (isHiding)
            {
                UnhidePlayer();
            }
            else
            {
                HidePlayer();
            }
        }
    }

    void HidePlayer()
    {
        if (player == null) return;

        originalPosition = player.transform.position;
        player.transform.position = hidePosition.position;
        player.GetComponent<CharacterController>().enabled = false; // Optional: freeze movement
        isHiding = true;

        // You can also disable player input here if needed
    }

    void UnhidePlayer()
    {
        if (player == null) return;

        player.transform.position = originalPosition;
        player.GetComponent<CharacterController>().enabled = true;
        isHiding = false;

        // Re-enable player input if you disabled it
    }
}
