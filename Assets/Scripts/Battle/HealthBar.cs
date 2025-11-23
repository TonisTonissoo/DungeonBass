using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text healthText;

    // It's good practice to ensure components are assigned,
    // and Awake is the best place for this.
    private void Awake()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
        if (healthText == null)
        {
            healthText = GetComponentInChildren<TMP_Text>();
        }
    }

    public void updateHealthBar(float currentHealth, float maxHealth)
    {
        // Ensure the components are not null before using them.
        if (slider != null)
        {
            slider.value = currentHealth / maxHealth;
        }
        
        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
            Debug.Log($"[HealthBar] Updated text to: {healthText.text} on {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"[HealthBar] Cannot update text - healthText is null on {gameObject.name}");
        }
    }
}
