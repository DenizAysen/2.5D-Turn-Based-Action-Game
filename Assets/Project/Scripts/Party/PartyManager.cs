using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private PartyMemberInfo[] allMembers;
    [SerializeField] private List<PartyMember> currentParty;

    [SerializeField] private PartyMemberInfo defaultPartyMember;
    private void Awake()
    {
        AddMemberToPartyByName(defaultPartyMember.MemberName);
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
