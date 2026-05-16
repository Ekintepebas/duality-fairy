using UnityEngine;

public class WindZone : MonoBehaviour
{
    [Header("Wind Settings")]
    public Vector2 windDirection = Vector2.left;
    public float windStrength = 3f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerController2D player = collision.GetComponent<PlayerController2D>();

        if (player != null)
        {
            player.windForce = windDirection.normalized * windStrength;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController2D player = collision.GetComponent<PlayerController2D>();

        if (player != null)
        {
            player.windForce = Vector2.zero;
        }
    }
}