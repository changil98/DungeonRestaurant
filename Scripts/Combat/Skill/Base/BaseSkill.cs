using UnityEngine;

public abstract class BaseSkill : ScriptableObject
{
    public string skillName;
    public string rcode;
    public Sprite skillImg;
    public int manaCost;
    public GameObject skillEffectObject;
    [TextArea]
    public string skillDiscription;
    public bool useOnCombatStart = false;

    public abstract void ExecuteSkill(BaseCombatAI character);
}
