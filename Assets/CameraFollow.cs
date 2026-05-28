using UnityEngine;
using System.Collections; // Bunu eklemeyi unutma, Coroutine (IEnumerator) için şart!

public class CameraFollow : MonoBehaviour
{
    [Header("Takip Edilecek Player")]
    public Transform player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    [Header("Sarsıntı Ayarları")]
    public float sarsintiSuresi = 0.2f;
    public float sarsintiSiddeti = 0.2f;
    private Vector3 sarsintiOfseti;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Kameranın pozisyonunu hem takip hem sarsıntı ofsetine göre ayarla
        transform.position = smoothedPosition + sarsintiOfseti;
    }

    // Sarsıntı fonksiyonu
    public IEnumerator Shake()
    {
        float gecenSure = 0f;

        while (gecenSure < sarsintiSuresi)
        {
            float x = Random.Range(-1f, 1f) * sarsintiSiddeti;
            float y = Random.Range(-1f, 1f) * sarsintiSiddeti;

            sarsintiOfseti = new Vector3(x, y, 0);

            gecenSure += Time.deltaTime;
            yield return null;
        }

        sarsintiOfseti = Vector3.zero;
    }
}