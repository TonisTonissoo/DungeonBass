using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text healthText;
    public void updateHealthBar(float currentHealth, float maxHealth)
    {
        slider.value = currentHealth / maxHealth;
        healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
    }
    void Update()
    {
           
    }
}
