using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10); // Varsayılan Z değeri

    [Header("Sarsıntı Ayarları")]
    public float sarsintiSuresi = 0.2f;
    public float sarsintiSiddeti = 0.2f;
    private Vector3 sarsintiOfseti;

    void LateUpdate()
    {
        if (player == null) return;

        // Hedef pozisyonu hesapla
        Vector3 desiredPosition = player.position + offset;
        // Yumuşak geçiş (Smooth Speed)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Hedef pozisyonu uygula
        transform.position = smoothedPosition + sarsintiOfseti;
    }

    public IEnumerator Shake()
    {
        Vector3 originalPos = Vector3.zero; // Sarsıntı sonrası merkeze dön
        float gecenSure = 0f;

        while (gecenSure < sarsintiSuresi)
        {
            float x = Random.Range(-1f, 1f) * sarsintiSiddeti;
            float y = Random.Range(-1f, 1f) * sarsintiSiddeti;

            sarsintiOfseti = new Vector3(x, y, 0);

            gecenSure += Time.deltaTime;
            yield return null;
        }

        sarsintiOfseti = Vector3.zero; // Sarsıntıyı bitir
    }
}