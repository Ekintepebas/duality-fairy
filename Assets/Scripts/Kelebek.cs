using UnityEngine;

public class Kelebek : MonoBehaviour
{
    [Header("Hareket ve Kaçış Ayarları")]
    public float sakinHiz = 1.5f;        // Peri yokken kendi kendine gezinme hızı
    public float panikHizi = 6.0f;       // Periyi görünce kaçış hızı
    public float algilamaMesafesi = 5.0f;// Periyi bu mesafeden görür
    public float calmTurnInterval = 2.0f;// Sakin halde yön değiştirme sıklığı

    private Transform playerTransform;
    private Vector2 rastgeleYon;
    private float calmTimer;
    private bool panicking = false;

    void Start()
    {
        // "Player" etiketli nesneyi bul
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        // Başlangıçta rastgele bir yön belirle
        rastgeleYon = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        calmTimer = calmTurnInterval;
    }

    void Update()
    {
        if (playerTransform == null) return;

        float mesafe = Vector2.Distance(transform.position, playerTransform.position);

        if (mesafe < algilamaMesafesi)
        {
            // --- PANİK DURUMU ---
            panicking = true;
            // Periden uzaklaşan yön
            Vector2 kacisYonu = (transform.position - playerTransform.position).normalized;

            // Zikzak etkisi için rastgele sapma ekle
            kacisYonu.x += Random.Range(-0.8f, 0.8f);
            kacisYonu.y += Random.Range(-0.8f, 0.8f);
            kacisYonu = kacisYonu.normalized;

            transform.Translate(kacisYonu * panikHizi * Time.deltaTime);
        }
        else
        {
            // --- SAKİN DURUMU ---
            panicking = false;
            calmTimer -= Time.deltaTime;
            // Belli aralıklarla yönü rastgele değiştir
            if (calmTimer <= 0)
            {
                rastgeleYon = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                calmTimer = calmTurnInterval;
            }

            transform.Translate(rastgeleYon * sakinHiz * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Kelebeği yakaladın, sayacı arttır ve yok et
            PlayerController2D player = collision.GetComponent<PlayerController2D>();
            if (player != null)
            {
                player.KelebekYakala();
            }
            Destroy(gameObject);
        }
    }
}