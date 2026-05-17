using UnityEngine;

public class End : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController2D player = other.GetComponent<PlayerController2D>();

            if (player != null)
            {
                if (player.yakalananKelebekSayisi >= player.hedefKelebekSayisi)
                {
                    Debug.Log("BİTİŞ NOKTASINA ULAŞILDI! Görev başarılı!");
                    // SceneManager.LoadScene("NextScene");
                }
                else
                {
                    int kalan = player.hedefKelebekSayisi - player.yakalananKelebekSayisi;
                    Debug.Log("Henüz bitmedi! " + kalan + " kelebek daha yakala!");
                }
            }
        }
    }

    void Start() { }
    void Update() { }
}
