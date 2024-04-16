using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    #region Privates
    private PartyManager _partyManager;
    private EnemyManager _enemyManager;

    private int _currentPlayer;

    private const string ACTION_MESSAGE = "'s Action";
    private const string WIN_MESSAGE = "You win";
    private const int TURN_DURATION = 2;
    #endregion

    #region Serialized Fields

    [SerializeField] private BattleState battleState;

    [Header("Battlers")]
    [SerializeField] private List<BattleEntities> allBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> enemyBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> playerBattlers = new List<BattleEntities>();

    [Header("SpawnPoints")]
    [SerializeField] private Transform[] partySpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;

    [Header("UI")]
    [SerializeField] private GameObject[] enemySelectionButtons;
    [SerializeField] private GameObject battleMenu;
    [SerializeField] private GameObject enemySelectionMenu;
    [SerializeField] private GameObject bottomTextPopUp;

    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private TextMeshProUGUI bottomText;
    #endregion
    void Start()
    {
        _partyManager = FindObjectOfType<PartyManager>();
        _enemyManager = FindObjectOfType<EnemyManager>();

        CreatePartyEntities();
        CreateEnemyEntities();

        ShowBattleMenu();

    }
    #region Coroutines
    private IEnumerator BattleRoutine()
    {
        enemySelectionMenu.SetActive(false);
        battleState = BattleState.Battle;
        bottomTextPopUp.SetActive(true);

        for (int i = 0; i < allBattlers.Count; i++)
        {
            switch (allBattlers[i].battleAction)
            {
                case Action.Attack:
                    yield return StartCoroutine(AttackRoutine(i));
                    break;
                case Action.Run:

                    break;

                default:
                    Debug.Log("Incorrect battle action");
                    break;
            }
        }
        if (battleState == BattleState.Battle)
        {
            bottomTextPopUp.SetActive(false);
            _currentPlayer = 0;
            ShowBattleMenu();
        }

        yield return null;
    }
    private IEnumerator AttackRoutine(int index)
    {
        if (allBattlers[index].IsPlayer)
        {
            BattleEntities curAttacker = allBattlers[index];
            BattleEntities curTarget = allBattlers[curAttacker.Target];
            AttackAction(curAttacker, curTarget);
            yield return new WaitForSeconds(TURN_DURATION);

            if(curTarget.CurrentHealth <= 0)
            {
                bottomText.text = string.Format("{0} defeated {1}", curAttacker, curTarget);
                yield return new WaitForSeconds(TURN_DURATION);

                enemyBattlers.Remove(curTarget);
                allBattlers.Remove(curTarget);

                if (enemyBattlers.Count == 0)
                {
                    battleState = BattleState.Won;

                    bottomText.text = WIN_MESSAGE;

                    yield return new WaitForSeconds(TURN_DURATION);
                    Debug.Log("Go Back to the Overworld");
                }
            }
          
        }
        //yield return null;
    }
    #endregion
    #region Entities
    private void CreatePartyEntities()
    {
        List<PartyMember> currentParty = new List<PartyMember>();
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
            playerBattlers.Add(tempEntity);
        }
    }

    private void CreateEnemyEntities()
    {
        List<Enemy> currentEnemies = new List<Enemy>();
        currentEnemies = _enemyManager.GetCurrentEnemies();

        for (int i = 0; i < currentEnemies.Count; i++)
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
    #endregion
    #region UI
    public void ShowBattleMenu()
    {
        actionText.text = playerBattlers[_currentPlayer].Name + ACTION_MESSAGE;
        battleMenu.SetActive(true);
    }
    public void ShowEnemySelectionMenu()
    {
        battleMenu.SetActive(false);
        SetEnemySelectionButtons();
        enemySelectionMenu.SetActive(true);
    }
    private void SetEnemySelectionButtons()
    {
        for (int i = 0; i < enemySelectionButtons.Length; i++)
        {
            enemySelectionButtons[i].SetActive(false);
        }

        for (int i = 0; i < enemyBattlers.Count; i++)
        {
            enemySelectionButtons[i].SetActive(true);
            enemySelectionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = enemyBattlers[i].Name;
        }
    } 
    #endregion
    public void SelectEnemy(int currentEnemy)
    {
        BattleEntities currentPlayerEntity = playerBattlers[_currentPlayer];
        currentPlayerEntity.SetTarget(allBattlers.IndexOf(enemyBattlers[currentEnemy]));

        currentPlayerEntity.battleAction = Action.Attack;
        _currentPlayer++;

        if(_currentPlayer >= playerBattlers.Count)
        {
            Debug.Log("Start Battle");
            StartCoroutine(BattleRoutine());

        }
        else
        {
            enemySelectionMenu.SetActive(false);
            ShowBattleMenu();
        }
    }
    private void AttackAction(BattleEntities currentAttacker, BattleEntities currentTarget)
    {
        int damage = currentAttacker.Strength;
        currentAttacker.BattleVisuals.PlayAttackAnimation();
        currentTarget.CurrentHealth -= damage;
        currentTarget.BattleVisuals.PlayHitAnimation();
        currentTarget.UpdateUI();
        bottomText.text = string.Format("{0} attacks {1} for {2} damage", currentAttacker.Name , currentTarget.Name , damage);
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
    public int Target;

    public BattleVisuals BattleVisuals;
    public Action battleAction;
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
    public void SetTarget(int target)
    {
        Target = target;
    }
    public void UpdateUI()
    {
        BattleVisuals.ChangeHealth(CurrentHealth);
    }
}