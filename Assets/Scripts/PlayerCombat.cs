using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Gerekli Atamalar")]
    public MovePlayer movePlayer;
    public Transform firePoint;

    void Start()
    {
        if (movePlayer == null)
        {
            movePlayer = FindFirstObjectByType<MovePlayer>();
        }
    }

    void Update()
    {
        // Lazerle ilgili tüm yetki ve kodlar karışıklık olmasın diye MovePlayer scriptine taşındı.
    }
}