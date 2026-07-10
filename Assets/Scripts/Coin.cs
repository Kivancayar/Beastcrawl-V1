using UnityEngine;

public class Coin : MonoBehaviour
{
    private AudioSource coinSound;

    private void Start()
    {
        coinSound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // En kritik yer: Bu mesaj konsolda hiç çıkmıyorsa, trigger hiç tetiklenmiyor demektir.
        Debug.Log("TETİKLENDİ! Temas edilen obje: " + collision.gameObject.name);

        if (collision.CompareTag("Player"))
        {
            Debug.Log("BAŞARILI: 'Player' etiketi doğrulandı, skor artıyor.");

            GameManager.score += 1;

            if (coinSound != null && coinSound.clip != null)
            {
                AudioSource.PlayClipAtPoint(coinSound.clip, transform.position);
            }

            Destroy(gameObject);
        }
        else
        {
            // Eğer "Player" etiketi yoksa, konsolda objenin gerçek tag'ini göreceksin.
            Debug.Log("HATA: Çarptığım objenin etiketi 'Player' değil. Mevcut etiket: " + collision.tag);
        }
    }
}