using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New item", menuName = "Item")]
public class Item : ScriptableObject
{
    public string Name;
    public int Size;
    public int Price;
    public Sprite Sprite;
    public List<Skill> Skills;
}

[Serializable]
public enum Trigger
{
    GetPlayerEffectStacks,
    RoundStart,
    TileEnter
}

[Serializable]
public enum Effect
{
    RemoveStackWithChance,
    AbbreviatedPassageOfTile,
    AddStackWithChance
}

[Serializable]
public class Skill
{
    public Trigger Trigger;
    public int TriggerValue;
    public Effect Effect;
    public int EffectValue;
    public int EffectValueAdditional;
    public string EffectDescription;
    public bool OneTimeUse;
}