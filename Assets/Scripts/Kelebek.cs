using UnityEngine;

public class Kelebek : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController2D player = collision.GetComponent<PlayerController2D>();

            if (player != null)
            {
                player.KelebekYakala();
            }

            Destroy(gameObject);
        }
    }
}