using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EventOutcome
{
    public string description;                   // ���� �������� ��� ���������
    [Range(0, 100)]
    public int chance;                           // ���� �� �������� (� ��������)

    public enum OutcomeType
    {
        Coins,                                   
        StatusEffect,                            
        ItemReward,  
        SecretFounded,
        TimeLose
    }

    public OutcomeType outcomeType;              // ��� ������
    public int value;
    public int effectType;
    public Item itemReward;                      // ������� � ��� �������� (�����������)

    public EventOutcome(string description, int chance, OutcomeType outcomeType, int value, Item item = null, int effectType = 0)
    {
        this.description = description;
        this.chance = chance;
        this.outcomeType = outcomeType;
        this.value = value;
        this.itemReward = item;
        this.effectType = effectType;
    }
}

[CreateAssetMenu(fileName = "NewLocationEvent", menuName = "Location Event")]
public class LocationEvent : ScriptableObject
{
    public string eventName;                           // ����� ��䳿
    [TextArea]
    public string description;                         // ���� ��䳿
    public Sprite spriteOriginal;                              // ������ ��� ���������� ����������� ��䳿
    public Sprite spriteAlter;                              // ������ ��� ���������� ����������� ��䳿
    [Range(0, 100)]
    public int successChance;                          // ������� ���� �� ���� � �������� (0-100)

    public List<EventOutcome> rewards;                 // ������ �������� ������� �� ����
    public List<EventOutcome> penalties;               // ������ �������� �������� �� ������

    // ����� ��� ��������� ��������� ��������
    public EventOutcome GetRandomReward()
    {
        return GetRandomOutcome(rewards);
    }

    // ����� ��� ��������� ����������� ���������
    public EventOutcome GetRandomPenalty()
    {
        return GetRandomOutcome(penalties);
    }

    // ��������� ����� ��� ������ ����������� ������
    private EventOutcome GetRandomOutcome(List<EventOutcome> outcomes)
    {
        int totalChance = 0;
        foreach (var outcome in outcomes)
            totalChance += outcome.chance;

        int randomValue = UnityEngine.Random.Range(0, totalChance);
        int accumulatedChance = 0;

        foreach (var outcome in outcomes)
        {
            accumulatedChance += outcome.chance;
            if (randomValue < accumulatedChance)
                return outcome;
        }
        return null; // ������� null, ���� ������ outcomes ������� ��� ���� �� ���������
    }
}