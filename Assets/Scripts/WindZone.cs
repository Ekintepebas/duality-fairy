using UnityEngine;

// Bu script rüzgar alanının kendisine (ExampleWindZone) eklenir.
public class WindZone : MonoBehaviour
{
    [Header("Rüzgar Ayarları")]
    public Vector2 specificWindForce = new Vector2(5f, 0f); // Bu alanın sabit gücü ve yönü
    public float specificWindStrength = 1f; // Gücü

    [Header("Görsel Ayar")]
    public GameObject localWindSpriteObject; // image_8.png'den oluşturacağın görsel nesnesi

    private void Start()
    {
        // Oyun başında görsel gizli olsun
        if (localWindSpriteObject != null)
        {
            localWindSpriteObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Eğer giren nesne PlayerController2D'ye sahipse
        PlayerController2D player = collision.GetComponent<PlayerController2D>();
        if (player != null)
        {
            // Oyuncu rüzgardan etkilensin
            player.isInWindZone = true;
            player.windForce = specificWindForce * specificWindStrength;

            // Rüzgar görselini göster (image_8.png)
            if (localWindSpriteObject != null)
            {
                localWindSpriteObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController2D player = collision.GetComponent<PlayerController2D>();
        if (player != null)
        {
            // Oyuncu rüzgar alanından çıkınca sıfırla
            player.isInWindZone = false;
            player.windForce = Vector2.zero;

            // Rüzgar görselini gizle
            if (localWindSpriteObject != null)
            {
                localWindSpriteObject.SetActive(false);
            }
        }
    }
}
  
