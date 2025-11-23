using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private List<Slider> stageSliders;
    [SerializeField] private Slider easeSlider;
    [SerializeField] private TMP_Text healthText;

    [Header("Animation Settings")]
    [SerializeField] private float easeSpeed = 0.5f;
    [SerializeField] private float stageBreakDelay = 0.25f;

    [Header("Stage Colors")]
    [SerializeField] private List<Color> stageColors = new List<Color>()
    {
        new Color(0.2f, 1f, 0.2f),  // Stage 0: Green (first/top stage)
        new Color(1f, 0.9f, 0.2f),  // Stage 1: Yellow (middle stage)
        new Color(1f, 0.2f, 0.2f)   // Stage 2: Red (final/bottom stage)
    };
    [SerializeField] private Color easeSliderColor = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Grey/transparent

    private int totalStages;
    private float maxHealth;
    private Coroutine easeCoroutine;
    private bool isInitialized = false;

    public void SetupStages(int numStages)
    {
        totalStages = numStages;
        
        // Null check for stageSliders
        if (stageSliders == null || stageSliders.Count == 0)
        {
            Debug.LogError("BossHealthBar: stageSliders list is empty or null!");
            return;
        }

        // Enable/disable and color the appropriate stage sliders
        for (int i = 0; i < stageSliders.Count; i++)
        {
            if (stageSliders[i] != null)
            {
                stageSliders[i].gameObject.SetActive(i < totalStages);
                
                // Apply color to the slider's fill area
                if (i < totalStages)
                {
                    Image fillImage = stageSliders[i].fillRect?.GetComponent<Image>();
                    if (fillImage != null)
                    {
                        // Use the color directly from the list based on index
                        fillImage.color = i < stageColors.Count ? stageColors[i] : Color.white;
                    }
                    else
                    {
                        Debug.LogWarning($"BossHealthBar: Stage slider {i} doesn't have a Fill Image component!");
                    }
                }
            }
        }

        // Color the ease slider (background effect)
        if (easeSlider != null)
        {
            Image easeImage = easeSlider.fillRect?.GetComponent<Image>();
            if (easeImage != null)
            {
                easeImage.color = easeSliderColor;
            }
        }

        // Ensure the health text component is enabled
        if (healthText != null)
        {
            healthText.enabled = true;
        }
        else
        {
            Debug.LogWarning("BossHealthBar: healthText is not assigned in the Inspector!");
        }

        isInitialized = true;
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        // Safety check
        if (!gameObject.activeInHierarchy || !isInitialized || stageSliders == null || stageSliders.Count == 0 || totalStages == 0)
        {
            return;
        }

        this.maxHealth = maxHealth;
        float healthPerStage = maxHealth / totalStages;

        // Determine which stage we're currently on by finding which "bracket" the health falls into
        int currentStageNumber;
        
        if (currentHealth <= 0)
        {
            currentStageNumber = 0; // Dead
        }
        else
        {
            currentStageNumber = Mathf.FloorToInt((currentHealth - 0.01f) / healthPerStage) + 1;
            currentStageNumber = Mathf.Clamp(currentStageNumber, 1, totalStages);
        }
        
        // Calculate health within the current stage
        float stageMinHealth = (currentStageNumber - 1) * healthPerStage;
        float currentStageHealth = currentHealth - stageMinHealth;
        
        // Clamp to ensure it's within valid range
        currentStageHealth = Mathf.Clamp(currentStageHealth, 0, healthPerStage);
        
        // Debug logging
        Debug.Log($"[BossHealthBar {Time.time:F2}] Total HP: {currentHealth}/{maxHealth}, Stage: {currentStageNumber}/{totalStages}, Stage HP: {currentStageHealth:F1}/{healthPerStage:F1}");
        
        // Update all sliders
        for (int i = 0; i < totalStages; i++)
        {
            if (i < stageSliders.Count && stageSliders[i] != null)
            {
                // Calculate which stage this slider represents (from top)
                int stageNumber = totalStages - i;
                
                // Calculate health thresholds for this stage
                float stageMinHealthCalc = (stageNumber - 1) * healthPerStage;
                
                // How much of the current health falls within this stage's range?
                float healthInThisStage = Mathf.Clamp(currentHealth - stageMinHealthCalc, 0, healthPerStage);
                
                // Convert to 0-1 range for the slider
                float sliderValue = healthInThisStage / healthPerStage;
                
                // If this stage is completely empty, deactivate it
                if (sliderValue <= 0f)
                {
                    stageSliders[i].gameObject.SetActive(false);
                }
                else
                {
                    // Make sure it's active if it has health
                    if (!stageSliders[i].gameObject.activeSelf)
                    {
                        stageSliders[i].gameObject.SetActive(true);
                    }
                    stageSliders[i].value = sliderValue;
                }
            }
        }

        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(currentStageHealth)} / {Mathf.CeilToInt(healthPerStage)}";
            print($"[BossHealthBar] Health Text Updated: {healthText.text}");
        }

        // Only start the coroutine if the GameObject is still active
        if (easeSlider != null && gameObject.activeInHierarchy)
        {
            if (easeCoroutine != null)
            {
                StopCoroutine(easeCoroutine);
            }
            easeCoroutine = StartCoroutine(AnimateEaseSlider(currentHealth));
        }
    }

    private IEnumerator AnimateEaseSlider(float currentHealth)
    {
        if (easeSlider == null || maxHealth == 0) yield break;

        float targetValue = currentHealth / maxHealth;
        float initialValue = easeSlider.value;
        float timer = 0;

        // Wait for a moment before the bar starts draining
        yield return new WaitForSeconds(stageBreakDelay);

        while (timer < easeSpeed)
        {
            timer += Time.deltaTime;
            easeSlider.value = Mathf.Lerp(initialValue, targetValue, timer / easeSpeed);
            yield return null;
        }

        easeSlider.value = targetValue;
    }
}