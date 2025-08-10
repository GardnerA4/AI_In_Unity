using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    public static bool IsPlayerHiding { get; private set; } 

    private bool playerInRange = false;
    private bool isHiding = false;
    private GameObject player;
    private Vector3 originalPosition;

    public Transform hidePosition; 

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
        player.GetComponent<CharacterController>().enabled = false;
        isHiding = true;
        IsPlayerHiding = true; 
    }

    void UnhidePlayer()
    {
        if (player == null) return;

        player.transform.position = originalPosition;
        player.GetComponent<CharacterController>().enabled = true;
        isHiding = false;
        IsPlayerHiding = false; 
    }
}