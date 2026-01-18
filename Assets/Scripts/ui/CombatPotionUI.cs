using TMPro;
using UnityEngine;

public class CombatPotionUI : MonoBehaviour
{
    [SerializeField] private TMP_Text potionText;

    private void OnEnable()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnStatsChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnStatsChanged -= Refresh;
    }

    private void Refresh()
    {
        if (PlayerStats.Instance == null) return;

        if (potionText != null)
            potionText.text = $"Potions: {PlayerStats.Instance.healingPotions}";
    }
}
