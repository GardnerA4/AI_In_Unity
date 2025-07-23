using UnityEngine;


public class AIVision : MonoBehaviour
{
    public Transform player;
    public float visionRange = 15f;
    public float viewAngle = 90f;
    public LayerMask obstacleMask;

    public bool CanSeePlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;

        if (Vector3.Distance(transform.position, player.position) < visionRange)
        {
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.position, obstacleMask))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
