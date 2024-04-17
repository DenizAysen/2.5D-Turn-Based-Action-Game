using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyInfo[] allEnemies;
    [SerializeField] private List<Enemy> currentEnemies;

    private const float LEVEL_MODIFIER = 0.5f;
    private static EnemyManager instance;
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void GenerateEnemiesByEncounter(Encounter[] encounters, int maxNumEnemies)
    {
        currentEnemies.Clear();
        int numEnemies = UnityEngine.Random.Range(1,maxNumEnemies + 1);

        for (int i = 0; i < numEnemies; i++)
        {
            Encounter tempEncounter = encounters[UnityEngine.Random.Range(0, encounters.Length)];
            GenerateEnemyByName(tempEncounter.Enemy.EnemyName, 
                UnityEngine.Random.Range(tempEncounter.MinLevel, tempEncounter.MaxLevel +1));
        }
    }
    private void GenerateEnemyByName(string enemyName , int level) 
    {
        for (int i = 0; i < allEnemies.Length; i++) 
        {
            if(enemyName == allEnemies[i].EnemyName)
            {
                Enemy enemy = new Enemy();

                enemy.EnemyName = allEnemies[i].EnemyName;
                enemy.Level = level;
                float levelModifier = (LEVEL_MODIFIER * enemy.Level);

                enemy.MaxHealth = Mathf.RoundToInt(allEnemies[i].BaseHealth + (allEnemies[i].BaseHealth * levelModifier));
                enemy.CurrentHealth = enemy.MaxHealth;
                enemy.Strength = Mathf.RoundToInt(allEnemies[i].BaseStr + (allEnemies[i].BaseStr * levelModifier));
                enemy.Initiative = Mathf.RoundToInt(allEnemies[i].BaseInitiative + (allEnemies[i].BaseInitiative * levelModifier));
                enemy.EnemyVisualPrefab = allEnemies[i].EnemyVisualPrefab;

                currentEnemies.Add(enemy);
            }

        } 
    }
    public List<Enemy> GetCurrentEnemies() => currentEnemies;
}
[Serializable]
public class Enemy
{
    public string EnemyName;
    public int Level;
    public int CurrentHealth;
    public int MaxHealth;
    public int Strength;
    public int Initiative;
    public GameObject EnemyVisualPrefab;
}
