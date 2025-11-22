using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance;

    private Image fadeImage;
    public float fadeSpeed = 1.2f; // Ühtne fade kiirus kõikjal

    private void Awake()
    {
        // Loome singletoni per scene
        Instance = this;

        // Leia FadeScreen objekt
        fadeImage = GameObject.Find("FadeScreen").GetComponent<Image>();

        // FadeScreen peab alati alguses olema must
        fadeImage.color = new Color(0, 0, 0, 1);

        // Kohe pärast scene loadi alustame fade-in'iga
        StartCoroutine(FadeInRoutine());
    }

    // Kutsutakse enne scene muutmist
    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutRoutine(sceneName));
    }

    private IEnumerator FadeOutRoutine(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);
        float alpha = 0f;

        // Fade OUT (nähtavaks)
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeInRoutine()
    {
        fadeImage.gameObject.SetActive(true);
        float alpha = 1f;

        // Fade IN (must → nähtav)
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
    }
}
