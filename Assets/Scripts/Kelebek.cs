using UnityEngine;

public class Kelebek : MonoBehaviour
{
    // PERİ İLE TEMAS EDİLDİĞİNDE ÇALIŞAN FONKSİYON
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Eğer kelebeğe çarpan nesnenin etiketi "Player" (Peri) ise:
        if (collision.CompareTag("Player"))
        {
            // Perinin üzerindeki ana koda (PlayerController2D) ulaş
            PlayerController2D player = collision.GetComponent<PlayerController2D>();
            
            // Eğer koda başarıyla ulaştıysak sayacı 1 arttır
            if (player != null)
            {
                player.KelebekYakala();
            }
            
            // Temas gerçekleştikten sonra kelebeği sahneden tamamen sil (yok et)
            Destroy(gameObject);
        }
    }
}