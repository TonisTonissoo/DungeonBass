using UnityEngine;

public static class PauseManager
{
    public static bool IsPaused { get; private set; }

    public static void PauseGame()
    {
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public static void ResumeGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;
    }
}
