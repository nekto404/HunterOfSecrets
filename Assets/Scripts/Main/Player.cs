using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private static Player _instance;
    public static Player Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Player();
            }
            return _instance;
        }
    }

    public int Coins { get; private set; } = 0;
    public int Level { get; private set; } = 1;
    public Backpack Backpack { get; private set; }
    public Backpack Storage { get; private set; }
    public List<PlayerSkill> Skills { get; private set; } = new List<PlayerSkill>();
    public int[] CurrentStatuses { get; private set; } = new int[10];
    public delegate void CoinsChanged(int newAmount);
    public event CoinsChanged OnCoinsChanged;

    private List<Skill> tileEnterSkills = new List<Skill>();
    private List<Skill> getPlayerEffectStacksSkills = new List<Skill>();

    private Player()
    {
        Backpack = new Backpack();
        Storage = new Backpack();
    }

    public static void InitializeInstance()
    {
        if (_instance == null)
        {
            _instance = new Player();
        }
    }

    public void ActivateRoundStartSkills()
    {
        foreach (var item in Backpack.GetItems())
        {
            foreach (var skill in item.Skills)
            {
                if (skill.Trigger == Trigger.RoundStart)
                {
                    ActivateSkill(skill, item);
                }
            }
        }
    }

    public void ApplyStatus(int statusIndex)
    {
        if (statusIndex >= 0 && statusIndex < CurrentStatuses.Length)
        {
            CurrentStatuses[statusIndex]++;
            Debug.Log("Status " + statusIndex + " applied. Current count: " + CurrentStatuses[statusIndex]);

            // Активація навичок при отриманні стека ефекту
            foreach (var skill in GetGetPlayerEffectStacksSkills())
            {
                if (skill.TriggerValue == 0 || CurrentStatuses[statusIndex] >= skill.TriggerValue)
                {
                    if (skill.TriggerValue == statusIndex)
                    {
                        ActivateSkill(skill, null);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("Invalid status index.");
        }
    }

    private void ActivateSkill(Skill skill, Item item)
    {
        switch (skill.Effect)
        {
            case Effect.RemoveStackWithChance:
                if (UnityEngine.Random.Range(0, 100) < skill.EffectValue)
                {
                    RemoveStatus(skill.EffectValueAdditional);
                    Debug.Log($"Ефект {skill.EffectDescription} активовано: видалено стек ефекту {skill.EffectValueAdditional}");
                }
                break;

            case Effect.AbbreviatedPassageOfTile:
                // Скорочення часу проходження тайла
                // Логіка для скорочення часу проходження тайла
                break;

            case Effect.AddStackWithChance:
                if (UnityEngine.Random.Range(0, 100) < skill.EffectValue)
                {
                    ApplyStatus(skill.EffectValueAdditional);
                    Debug.Log($"Ефект {skill.EffectDescription} активовано: додано стек ефекту {skill.EffectValueAdditional}");
                }
                break;
        }

        if (skill.OneTimeUse && item != null)
        {
            Backpack.RemoveItem(item);
            RemoveItemSkills(item);
            Debug.Log($"Предмет {item.Name} видалено з рюкзака після одноразового використання.");
        }
    }

    private void RemoveItemSkills(Item item)
    {
        foreach (var skill in item.Skills)
        {
            getPlayerEffectStacksSkills.Remove(skill);
            tileEnterSkills.Remove(skill);
        }
    }

    public void CalculateGetPlayerEffectStacksSkills()
    {
        getPlayerEffectStacksSkills.Clear();
        foreach (var item in Backpack.GetItems())
        {
            foreach (var skill in item.Skills)
            {
                if (skill.Trigger == Trigger.GetPlayerEffectStacks)
                {
                    getPlayerEffectStacksSkills.Add(skill);
                }
            }
        }
        Debug.Log($"Загальна кількість навичок, що активуються при отриманні стека ефекту: {getPlayerEffectStacksSkills.Count}");
    }

    public List<Skill> GetGetPlayerEffectStacksSkills()
    {
        return getPlayerEffectStacksSkills;
    }

    public void CalculateTileEnterSkills()
    {
        tileEnterSkills.Clear();
        foreach (var item in Backpack.GetItems())
        {
            foreach (var skill in item.Skills)
            {
                if (skill.Trigger == Trigger.TileEnter)
                {
                    tileEnterSkills.Add(skill);
                }
            }
        }
        Debug.Log($"Загальна кількість навичок, що активуються при вході на тайл: {tileEnterSkills.Count}");
    }

    public List<Skill> GetTileEnterSkills()
    {
        return tileEnterSkills;
    }

    public void Initialize()
    {
        Coins = 40;                        // Початкова кількість монет
        Backpack = new Backpack();           // Створення нового рюкзака
        Storage = new Backpack();             // Створення нового складу
        Skills = new List<PlayerSkill>();    // Порожній список навичок
        CurrentStatuses = new int[10];       // Масив статусів
        Level = 1;                           // Початковий рівень
        OnCoinsChanged?.Invoke(Coins);
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        OnCoinsChanged?.Invoke(Coins);
        Debug.Log("Coins added: " + amount + ". Total coins: " + Coins);
    }

    public bool SpendCoins(int amount)
    {
        if (Coins >= amount)
        {
            Coins -= amount;
            Debug.Log("Coins spent: " + amount + ". Remaining coins: " + Coins);
            OnCoinsChanged?.Invoke(Coins);
            return true;
        }
        else
        {
            Debug.LogWarning("Not enough coins to complete the transaction.");
            return false;
        }
    }

    public void LevelUp()
    {
        Level++;
        Debug.Log("Player level increased! New level: " + Level);
    }

    public void AddSkill(PlayerSkill skill)
    {
        if (!Skills.Contains(skill))
        {
            Skills.Add(skill);
            Debug.Log("Skill added: " + skill.Name);
        }
    }

    public float GetSpeedModifier()
    {
        float speedModifier = 1.0f;
        if (CurrentStatuses[1] > 0)
        {
            speedModifier -= 0.01f * CurrentStatuses[1]; // Зменшення швидкості на 1% за кожну одиницю ефекту 1
        }
        return Mathf.Max(speedModifier, 0.1f); // Мінімальна швидкість 10%
    }

    public bool ShouldStop()
    {
        if (CurrentStatuses[2] > 20)
        {
            CurrentStatuses[2] -= 20; // Зменшення значення ефекту 2 на 20
            return true;
        }
        return false;
    }

    public void RemoveStatus(int statusIndex)
    {
        if (statusIndex >= 0 && statusIndex < CurrentStatuses.Length && CurrentStatuses[statusIndex] > 0)
        {
            CurrentStatuses[statusIndex]--;
            Debug.Log("Status " + statusIndex + " removed. Current count: " + CurrentStatuses[statusIndex]);
        }
        else
        {
            Debug.LogWarning("Invalid status index or no active statuses to remove.");
        }
    }

    public bool CanAfford(int amount)
    {
        return Coins >= amount;
    }

    public void ClearInstance()
    {
        _instance = null;
    }

    public string GetActiveStatuses()
    {
        List<string> activeStatuses = new List<string>();
        for (int i = 0; i < CurrentStatuses.Length; i++)
        {
            if (CurrentStatuses[i] > 0)
            {
                activeStatuses.Add($"Effect {i}: {CurrentStatuses[i]}");
            }
        }
        return string.Join(", ", activeStatuses);
    }
}




