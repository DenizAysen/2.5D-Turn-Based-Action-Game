using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    #region Privates
    private PartyManager _partyManager;
    private EnemyManager _enemyManager;
    #endregion

    #region Serialized Fields
    [Header("Battlers")]
    [SerializeField] private List<BattleEntities> allBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> enemyBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> playerBattlers = new List<BattleEntities>();

    [Header("SpawnPoints")]
    [SerializeField] private Transform[] partySpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;
    #endregion
    void Start()
    {
        _partyManager = FindObjectOfType<PartyManager>();
        _enemyManager = FindObjectOfType<EnemyManager>();

        CreatePartyEntities();
        CreateEnemyEntities();
    }
    private void CreatePartyEntities()
    {
        List<PartyMember> currentParty = new List<PartyMember> ();
        currentParty = _partyManager.GetCurrentParty();

        for (int i = 0; i < currentParty.Count; i++)
        {
            BattleEntities tempEntity = new BattleEntities();

            tempEntity.SetEntityValues(currentParty[i].MemberName, currentParty[i].CurrentHealth, currentParty[i].MaxHealth,
                currentParty[i].Strength, currentParty[i].Initiative, currentParty[i].Level, true);

            BattleVisuals tempBattleVisuals = Instantiate(currentParty[i].MemberBattleVisualPrefab, 
                partySpawnPoints[i].position, Quaternion.identity).GetComponent<BattleVisuals>();

            tempEntity.BattleVisuals = tempBattleVisuals;

            tempBattleVisuals.SetStartingValues(currentParty[i].CurrentHealth, currentParty[i].MaxHealth, currentParty[i].Level);

            allBattlers.Add(tempEntity);
            playerBattlers.Add (tempEntity);
        }
    }

    private void CreateEnemyEntities()
    {
        List<Enemy> currentEnemies = new List<Enemy>();
        currentEnemies = _enemyManager.GetCurrentEnemies();
        
        for (int i = 0;i < currentEnemies.Count; i++)
        {
            BattleEntities tempEntity = new BattleEntities();

            tempEntity.SetEntityValues(currentEnemies[i].EnemyName, currentEnemies[i].CurrentHealth, currentEnemies[i].MaxHealth,
                currentEnemies[i].Strength, currentEnemies[i].Initiative, currentEnemies[i].Level, false);

            BattleVisuals tempBattleVisuals = Instantiate(currentEnemies[i].EnemyVisualPrefab,
                enemySpawnPoints[i].position, Quaternion.identity).GetComponent<BattleVisuals>();

            tempEntity.BattleVisuals = tempBattleVisuals;

            tempBattleVisuals.SetStartingValues(currentEnemies[i].CurrentHealth, currentEnemies[i].MaxHealth, currentEnemies[i].Level);

            allBattlers.Add(tempEntity);
            enemyBattlers.Add(tempEntity);
        }
    }
}

[Serializable]
public class BattleEntities
{
    public string Name;
    public int CurrentHealth;
    public int MaxHealth;
    public int Strength;
    public int Initiative;
    public int Level;
    public bool IsPlayer;
    public BattleVisuals BattleVisuals;
    public void SetEntityValues(string name, int currentHealth, int maxHealth, int strength, int initiative, int level, bool isPlayer)
    {
        Name = name;
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
        Strength = strength;
        Initiative = initiative;
        Level = level;
        IsPlayer = isPlayer;
    }
}