using UnityEngine;
using System.Collections.Generic; // Add this line to use List<>

[CreateAssetMenu(fileName = "NewEncounter", menuName = "Combat/Encounter Definition")]
public class CombatEncounter : ScriptableObject
{
    [Header("Scene Settings")]
    [Tooltip("The name of the scene to load (e.g. 'Combat - Forest')")]
    public string combatSceneName;

    [Header("Enemy Settings")]
    [Tooltip("List of possible enemy prefabs. If this list is not empty, enemies will be randomly chosen from here.")]
    public List<GameObject> enemyPrefabs;

    [Header("Spawn Count")]
    [Min(1)] public int minEnemies = 1;
    [Min(1)] public int maxEnemies = 3;
}
