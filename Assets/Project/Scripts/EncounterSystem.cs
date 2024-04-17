using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterSystem : MonoBehaviour
{
    [SerializeField] private Encounter[] enemiesInScene;
    [SerializeField] private int maxNumEnemies;

    private EnemyManager _enemyManager;
    void Start()
    {
        _enemyManager = FindObjectOfType<EnemyManager>();
        _enemyManager.GenerateEnemiesByEncounter(enemiesInScene, maxNumEnemies);
    }
}
[Serializable]
public class Encounter
{
    public EnemyInfo Enemy;
    public int MinLevel;
    public int MaxLevel;
}
