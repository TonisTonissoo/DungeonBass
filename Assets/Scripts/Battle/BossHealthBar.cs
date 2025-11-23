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
        
        if (stageSliders == null || stageSliders.Count == 0)
        {
            Debug.LogError("BossHealthBar: stageSliders list is empty or null!");
            return;
        }

        for (int i = 0; i < stageSliders.Count; i++)
        {
            if (stageSliders[i] != null)
            {
                stageSliders[i].gameObject.SetActive(i < totalStages);
                
                if (i < totalStages)
                {
                    Image fillImage = stageSliders[i].fillRect?.GetComponent<Image>();
                    if (fillImage != null)
                    {
                        fillImage.color = i < stageColors.Count ? stageColors[i] : Color.white;
                    }
                    else
                    {
                        Debug.LogWarning($"BossHealthBar: Stage slider {i} doesn't have a Fill Image component!");
                    }
                }
            }
        }

        if (easeSlider != null)
        {
            Image easeImage = easeSlider.fillRect?.GetComponent<Image>();
            if (easeImage != null)
            {
                easeImage.color = easeSliderColor;
            }
        }

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
        if (!gameObject.activeInHierarchy || !isInitialized || stageSliders == null || stageSliders.Count == 0 || totalStages == 0)
        {
            return;
        }

        this.maxHealth = maxHealth;
        float healthPerStage = maxHealth / totalStages;

        int currentStageNumber;
        
        if (currentHealth <= 0)
        {
            currentStageNumber = 0;
        }
        else
        {
            currentStageNumber = Mathf.FloorToInt((currentHealth - 0.01f) / healthPerStage) + 1;
            currentStageNumber = Mathf.Clamp(currentStageNumber, 1, totalStages);
        }
        
        float stageMinHealth = (currentStageNumber - 1) * healthPerStage;
        float currentStageHealth = currentHealth - stageMinHealth;
        currentStageHealth = Mathf.Clamp(currentStageHealth, 0, healthPerStage);
        
        // Update all sliders - FIXED: now goes from top to bottom
        for (int i = 0; i < totalStages; i++)
        {
            if (i < stageSliders.Count && stageSliders[i] != null)
            {
                // Stage number now matches slider index: 0=first stage, 1=second, etc.
                int stageNumber = i + 1;
                
                float stageMinHealthCalc = (stageNumber - 1) * healthPerStage;
                float healthInThisStage = Mathf.Clamp(currentHealth - stageMinHealthCalc, 0, healthPerStage);
                float sliderValue = healthInThisStage / healthPerStage;
                
                if (sliderValue <= 0f)
                {
                    stageSliders[i].gameObject.SetActive(false);
                }
                else
                {
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
        }

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