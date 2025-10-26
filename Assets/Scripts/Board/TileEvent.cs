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
                // Käivita võitlus
                break;

            case TileType.Shop:
                Debug.Log("Shop entered!");
                // Ava shop UI
                break;

            case TileType.Rest:
                Debug.Log("Resting...");
                // Lisa HP
                break;

            case TileType.Bandit:
                Debug.Log("Bandit event!");
                // Raha riskimine
                break;

            case TileType.HorseCarriage:
                Debug.Log("Horse Carriage event!");
                // Teleportatsioon
                break;

            case TileType.RandomEvent:
                Debug.Log("Random card drawn!");
                // Random event süsteem
                break;

            case TileType.Boss:
                Debug.Log("Boss fight!");
                // Ava boss fight stseen
                break;

            case TileType.Start:
                Debug.Log("Start tile.");
                break;
        }
    }
}
