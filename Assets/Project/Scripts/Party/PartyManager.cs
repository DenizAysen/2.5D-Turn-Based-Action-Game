using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private PartyMemberInfo[] allMembers;
    [SerializeField] private List<PartyMember> currentParty;

    [SerializeField] private PartyMemberInfo defaultPartyMember;

    private Vector3 _playerPos;
    private static PartyManager instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        AddMemberToPartyByName(defaultPartyMember.MemberName);
        DontDestroyOnLoad(gameObject);
    }
    public void AddMemberToPartyByName(string memberName)
    {
        for (int i = 0; i < allMembers.Length; i++)
        {
            if (allMembers[i].MemberName == memberName)
            {
                PartyMember partyMember = new PartyMember();
                partyMember.MemberName = allMembers[i].MemberName;
                partyMember.Level = allMembers[i].StartingLevel;
                partyMember.CurrentHealth = allMembers[i].BaseHealth;
                partyMember.MaxHealth = partyMember.CurrentHealth;
                partyMember.Strength = allMembers[i].BaseStr;
                partyMember.Initiative = allMembers[i].BaseInitiative;
                partyMember.MemberBattleVisualPrefab = allMembers[i].MemberBattleVisualPrefab;
                partyMember.MemberOverworldVisualPrefab = allMembers[i].MemberOverworldVisualPrefab;

                currentParty.Add(partyMember);
            }
        }
    }
    public List<PartyMember> GetCurrentParty()
    {
        return currentParty;
    }
    public void SaveHealth(int partyMember , int health)
    {
        currentParty[partyMember].CurrentHealth = health;
    }
    public void SetPosition(Vector3 position)
    {
        _playerPos = position;
    }
    public Vector3 GetPosition() => _playerPos;
}
[Serializable]
public class PartyMember
{
    public string MemberName;
    public int Level;
    public int CurrentHealth;
    public int MaxHealth;
    public int Strength;
    public int Initiative;
    public int CurrentExp;
    public int MaxExp;
    public GameObject MemberBattleVisualPrefab;
    public GameObject MemberOverworldVisualPrefab;
}
