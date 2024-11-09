using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EventOutcome
{
    public string description;                   // Опис нагороди або покарання
    [Range(0, 100)]
    public int chance;                           // Шанс на випадіння (у відсотках)

    public enum OutcomeType
    {
        Coins,                                   // Нагорода у вигляді монет
        StatusEffect,                            // Накладання або зняття статусу
        ItemReward,                              // Нагорода у вигляді предмета
    }

    public OutcomeType outcomeType;              // Тип ефекту
    public int value;                            // Значення ефекту (кількість монет, зміна здоров'я)
    public Item itemReward;                      // Предмет у разі нагороди (опціонально)

    public EventOutcome(string description, int chance, OutcomeType outcomeType, int value, Item item = null)
    {
        this.description = description;
        this.chance = chance;
        this.outcomeType = outcomeType;
        this.value = value;
        this.itemReward = item;
    }
}

[CreateAssetMenu(fileName = "NewLocationEvent", menuName = "Location Event")]
public class LocationEvent : ScriptableObject
{
    public string eventName;                           // Назва події
    [TextArea]
    public string description;                         // Опис події
    public Sprite sprite;                              // Спрайт для візуального відображення події
    [Range(0, 100)]
    public int successChance;                          // Базовий шанс на успіх у відсотках (0-100)

    public List<EventOutcome> rewards;                 // Список можливих нагород за успіх
    public List<EventOutcome> penalties;               // Список можливих покарань за провал

    // Метод для отримання випадкової нагороди
    public EventOutcome GetRandomReward()
    {
        return GetRandomOutcome(rewards);
    }

    // Метод для отримання випадкового покарання
    public EventOutcome GetRandomPenalty()
    {
        return GetRandomOutcome(penalties);
    }

    // Загальний метод для вибору випадкового ефекту
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
        return null; // Повертає null, якщо список outcomes порожній або шанс не спрацьовує
    }
}