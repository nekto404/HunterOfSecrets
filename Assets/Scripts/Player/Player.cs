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

    public int coins = 0;
    public int level = 1;                   
    public Backpack backpack;
    public Storage storage;
    public List<PlayerSkill> skills = new List<PlayerSkill>();
    public int[] currentStatuses = new int[10];

    private Player() 
    {
        backpack = new Backpack();
        storage = new Storage();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.Log("Coins added: " + amount + ". Total coins: " + coins);
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            Debug.Log("Coins spent: " + amount + ". Remaining coins: " + coins);
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
        level++;
        Debug.Log("Player level increased! New level: " + level);
    }

    public void AddSkill(PlayerSkill skill)
    {
        if (!skills.Contains(skill))
        {
            skills.Add(skill);
            Debug.Log("Skill added: " + skill.Name);
        }
    }

    public void ApplyStatus(int statusIndex)
    {
        if (statusIndex >= 0 && statusIndex < currentStatuses.Length)
        {
            currentStatuses[statusIndex]++;
            Debug.Log("Status " + statusIndex + " applied. Current count: " + currentStatuses[statusIndex]);
        }
        else
        {
            Debug.LogWarning("Invalid status index.");
        }
    }

    public void RemoveStatus(int statusIndex)
    {
        if (statusIndex >= 0 && statusIndex < currentStatuses.Length && currentStatuses[statusIndex] > 0)
        {
            currentStatuses[statusIndex]--;
            Debug.Log("Status " + statusIndex + " removed. Current count: " + currentStatuses[statusIndex]);
        }
        else
        {
            Debug.LogWarning("Invalid status index or no active statuses to remove.");
        }
    }
}