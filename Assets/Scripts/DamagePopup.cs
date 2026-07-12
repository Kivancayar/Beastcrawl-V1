using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float fadeTime = 0.6f;

    private TextMeshProUGUI tmpText;
    private Color textColor;

    void Start()
    {
        // Çocukta da arasın diye bunu kullandık
        tmpText = GetComponentInChildren<TextMeshProUGUI>();

        if (tmpText != null)
        {
            textColor = tmpText.color;
        }

        // Canvas varsa kamerayı ata
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.worldCamera = Camera.main;
        }

        Destroy(gameObject, fadeTime);
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        if (tmpText != null)
        {
            textColor.a -= Time.deltaTime / fadeTime;
            tmpText.color = textColor;
        }
    }

    public void SetDamage(float damageAmount)
    {
        if (tmpText != null)
        {
            tmpText.text = damageAmount.ToString();
        }
    }
}