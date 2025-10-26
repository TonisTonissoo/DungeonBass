using UnityEngine;

public enum TileType
{
    Enemy,
    Shop,
    Rest,
    Bandit,
    HorseCarriage,
    RandomEvent,
    Boss,
    Start
}

public class TileEvent : MonoBehaviour
{
    public TileType tileType;

    public void TriggerEvent()
    {
        switch (tileType)
        {
            case TileType.Enemy:
                Debug.Log("Enemy encounter!");
                PlayerPrefs.SetInt("LastTileIndex", transform.GetSiblingIndex());
                PlayerPrefs.Save();
                SceneLoader.Load("CombatScene");
                break;

            case TileType.Boss:
                Debug.Log("Boss fight!");
                PlayerPrefs.SetInt("LastTileIndex", transform.GetSiblingIndex());
                PlayerPrefs.Save();
                SceneLoader.Load("CombatScene");
                break;

            case TileType.Shop:
                Debug.Log("Shop entered!");
                if (ShopUI.Instance != null)
                    ShopUI.Instance.OpenShop();
                else
                    Debug.LogWarning("ShopUI.Instance is missing in the scene!");
                break;

            case TileType.Rest:
                Debug.Log("Rest event triggered!");

                int coinsGained = Random.Range(10, 26); // 10–25 münti
                PlayerStats.Instance.AddCoins(coinsGained);

                HUDController.Instance?.UpdateHUD();

                // Kui sul on EventPopupManager (popup tekstiks)
                if (EventPopupManager.Instance != null)
                {
                    EventPopupManager.Instance.ShowEvent($"You found {coinsGained} coins while resting!");
                }
                else
                {
                    Debug.Log($"You found {coinsGained} coins while resting!");
                }
                break;

            case TileType.Bandit:
                Debug.Log("Bandit event!");
                // TODO: lisa raha riskimine / kaotus
                break;

            case TileType.HorseCarriage:
                Debug.Log("Horse Carriage event!");
                // TODO: lisa teleportatsioon / juhuslik liikumine
                break;

            case TileType.RandomEvent:
                Debug.Log("Random card drawn!");
                // TODO: lisa random event süsteem
                break;

            case TileType.Start:
                Debug.Log("Start tile.");
                break;
        }
    }
}
