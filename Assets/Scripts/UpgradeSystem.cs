using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public MovePlayer player;
    public int upgradeCost = 3;

    public void UpgradeSpeed()
    {
        if (GameManager.score >= upgradeCost)
        {
            GameManager.score -= upgradeCost;
            player.moveSpeed += 1f;
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
            Debug.Log("Damage arttı! Yeni Damage: " + player.laserDamage);
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }

    public void UpgradeHealth()
    {
        if (GameManager.score >= upgradeCost)
        {
            GameManager.score -= upgradeCost;
            player.playerHealth += 10f;
            Debug.Log("Health arttı! Yeni Health: " + player.playerHealth);
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }
}