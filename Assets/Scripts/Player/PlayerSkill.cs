using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New skill", menuName = "PlayerSkill")]
public class PlayerSkill : ScriptableObject
{
    public List<Skill> Skills;
    public Sprite Sprite;
    public string Name;
}
