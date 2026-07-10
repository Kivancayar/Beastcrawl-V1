using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
    public GameObject upgradePanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            upgradePanel.SetActive(!upgradePanel.activeSelf);
        }
    }
}