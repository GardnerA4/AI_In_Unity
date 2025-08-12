using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    public static bool IsPlayerHiding { get; private set; }

    private bool playerInRange = false;
    private bool isHiding = false;
    private GameObject player;
    private Vector3 originalPosition;

    [Header("Hide Offset")]
    public Vector3 hideOffset = Vector3.zero; // offset from the hiding spot's position

    private Vector3 hidePosition;

    private void Awake()
    {
        // By default, hide position is at this object’s position + offset
        hidePosition = transform.position + hideOffset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
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

    private void Update()
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

    private void HidePlayer()
    {
        if (player == null) return;

        originalPosition = player.transform.position;
        player.transform.position = hidePosition;
        player.GetComponent<CharacterController>().enabled = false;
        isHiding = true;
        IsPlayerHiding = true;
    }

    private void UnhidePlayer()
    {
        if (player == null) return;

        player.transform.position = originalPosition;
        player.GetComponent<CharacterController>().enabled = true;
        isHiding = false;
        IsPlayerHiding = false;
    }
}
