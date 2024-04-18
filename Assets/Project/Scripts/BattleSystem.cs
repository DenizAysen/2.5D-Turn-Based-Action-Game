using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleSystem : MonoBehaviour
{
    #region Privates
    private PartyManager _partyManager;
    private EnemyManager _enemyManager;

    private int _currentPlayer;

    #region Consts
    private const string ACTION_MESSAGE = "'s Action";
    private const string WIN_MESSAGE = "You win";
    private const string LOSE_MESSAGE = "Your party has been defeated";
    private const string SUCCESSFULLY_RAN_MESSAGE = "Your party ran away";
    private const string UNSUCCESSFULLY_RAN_MESSAGE = "Your party failed to run";
    private const string MAIN_SCENE = "MainScene";
    private const int RUN_CHANCE = 50;
    private const int TURN_DURATION = 2; 
    #endregion
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
        DetermineBattleOrder();
    }
    #region Coroutines
    private IEnumerator BattleRoutine()
    {
        enemySelectionMenu.SetActive(false);
        battleState = BattleState.Battle;
        bottomTextPopUp.SetActive(true);

        for (int i = 0; i < allBattlers.Count; i++)
        {
            if(battleState == BattleState.Battle && allBattlers[i].CurrentHealth > 0)
            {
                switch (allBattlers[i].battleAction)
                {
                    case Action.Attack:
                        yield return StartCoroutine(AttackRoutine(i));
                        break;
                    case Action.Run:
                        yield return StartCoroutine(RunRoutine());
                        break;

                    default:
                        Debug.Log("Incorrect battle action");
                        break;
                }
            }          
        }
        RemoveDeadBattlers();

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
            if (allBattlers[curAttacker.Target].CurrentHealth <= 0)
            {
                curAttacker.SetTarget(GetRandomEnemy());
            }
            BattleEntities curTarget = allBattlers[curAttacker.Target];
            AttackAction(curAttacker, curTarget);
            yield return new WaitForSeconds(TURN_DURATION);

            if(curTarget.CurrentHealth <= 0)
            {
                bottomText.text = string.Format("{0} defeated {1}", curAttacker.Name, curTarget.Name);
                yield return new WaitForSeconds(TURN_DURATION);

                enemyBattlers.Remove(curTarget);
                allBattlers.Remove(curTarget);

                if (enemyBattlers.Count == 0)
                {
                    battleState = BattleState.Won;

                    bottomText.text = WIN_MESSAGE;

                    yield return new WaitForSeconds(TURN_DURATION);
                    SceneManager.LoadScene(MAIN_SCENE);
                }
            }
          
        }

        else if (index < allBattlers.Count && allBattlers[index].IsPlayer == false)
        {
            BattleEntities curAttacker = allBattlers[index];
            curAttacker.SetTarget(GetRandomPartyMember());
            BattleEntities curTarget = allBattlers[curAttacker.Target];

            AttackAction(curAttacker, curTarget);
            yield return new WaitForSeconds(TURN_DURATION);

            if(curTarget.CurrentHealth <= 0)
            {
                bottomText.text = string.Format("{0} defeated {1}", curAttacker.Name, curTarget.Name);
                yield return new WaitForSeconds(TURN_DURATION);
                playerBattlers.Remove(curTarget);
                allBattlers.Remove(curTarget);

                if(playerBattlers.Count <= 0)
                {
                    battleState = BattleState.Lost;
                    bottomText.text = LOSE_MESSAGE;
                    yield return new WaitForSeconds(TURN_DURATION);
                    Debug.Log("Gaem over");
                }
            }
        }
        //yield return null;
    }
    private IEnumerator RunRoutine()
    {
        if(battleState == BattleState.Battle)
        {
            if(UnityEngine.Random.Range(1,101) >= RUN_CHANCE)
            {
                bottomText.text = SUCCESSFULLY_RAN_MESSAGE;
                battleState = BattleState.Run;
                allBattlers.Clear();
                yield return new WaitForSeconds(TURN_DURATION);
                SceneManager.LoadScene(MAIN_SCENE);
                yield break;
            }
            else
            {
                bottomText.text = UNSUCCESSFULLY_RAN_MESSAGE;
                yield return new WaitForSeconds(TURN_DURATION);
            }
        }
    }
    #endregion
    private void RemoveDeadBattlers()
    {
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if (allBattlers[i].CurrentHealth <= 0)
            {
                allBattlers.RemoveAt(i);
            }
        }
    }
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
    #region Combat
    public void SelectEnemy(int currentEnemy)
    {
        BattleEntities currentPlayerEntity = playerBattlers[_currentPlayer];
        currentPlayerEntity.SetTarget(allBattlers.IndexOf(enemyBattlers[currentEnemy]));

        currentPlayerEntity.battleAction = Action.Attack;
        _currentPlayer++;

        if (_currentPlayer >= playerBattlers.Count)
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
        bottomText.text = string.Format("{0} attacks {1} for {2} damage", currentAttacker.Name, currentTarget.Name, damage);
        SaveHealth();
    } 
    private int GetRandomPartyMember()
    {
        List<int> partyMembers = new List<int>();
        for (int i = 0; i< allBattlers.Count; i++)
        {
            if (allBattlers[i].IsPlayer)
                partyMembers.Add(i);
        }
        return partyMembers[UnityEngine.Random.Range(0,partyMembers.Count)];
    }
    private int GetRandomEnemy()
    {
        List<int> enemyMembers = new List<int>();
        for (int i = 0; i < allBattlers.Count; i++)
        {
            if (!allBattlers[i].IsPlayer)
                enemyMembers.Add(i);
        }
        return enemyMembers[UnityEngine.Random.Range(0, enemyMembers.Count)];
    }
    #endregion
    private void SaveHealth()
    {
        for (int i = 0; i < playerBattlers.Count; i++)
        {
            _partyManager.SaveHealth(i, playerBattlers[i].CurrentHealth);
        }
    }
    private void DetermineBattleOrder()
    {
        allBattlers.Sort((bi1, bi2) => -bi1.Initiative.CompareTo(bi2.Initiative)); 
    }
    public void SelectRunAction()
    {
        battleState = BattleState.Selection;
        BattleEntities currentPlayerEntity = playerBattlers[_currentPlayer];

        currentPlayerEntity.battleAction = Action.Run;

        battleMenu.SetActive(false);
        _currentPlayer++;

        if (_currentPlayer >= playerBattlers.Count)
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