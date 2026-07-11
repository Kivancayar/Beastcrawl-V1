using UnityEngine;
using System.Collections;

public class UpgradeSystem : MonoBehaviour
{
    public MovePlayer player;
    public int upgradeCost = 1;

    public void UpgradeSpeed()
    {
        if (GameManager.score >= upgradeCost)
        {
            GameManager.score -= upgradeCost;
            player.moveSpeed += 1f;
            player.StartCoroutine(FlashColor(Color.cyan));
            Debug.Log("Speed arttı! Yeni Speed: " + player.moveSpeed);
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }

    public void UpgradeDamage()
    {
        if (GameManager.score >= upgradeCost)
        {
            GameManager.score -= upgradeCost;
            player.laserDamage += 1f;
            player.StartCoroutine(FlashColor(Color.red));
            Debug.Log("Damage arttı! Yeni Damage: " + player.laserDamage);
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }

    // İŞTE BU KISIM 👇 BUNU KOPYALA
    public void UpgradeHealth()
    {
        if (GameManager.score >= upgradeCost)
        {
            GameManager.score -= upgradeCost;
            player.maxHealth += 10f; // MAX CAN ARTSIN
            player.playerHealth += 10f; // ŞU ANKİ CAN DA DOLSUN
            player.healthSlider.maxValue = player.maxHealth; // Barın maxı güncellensin
            player.StartCoroutine(FlashColor(Color.green));
            Debug.Log("Health arttı! Yeni Health: " + player.maxHealth);
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }

    IEnumerator FlashColor(Color flashColor)
    {
        player.sr.color = flashColor;
        yield return new WaitForSeconds(2f);
        player.sr.color = player.originalColor;
    }
}