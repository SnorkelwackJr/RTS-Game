using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Scriptable Objects/Unit", order = 1)]
public class UnitData : ScriptableObject
{
    public string code;
    public string unitName;
    public string description;
    public int healthpoints;
    public float fieldOfView;
    public GameObject prefab;
    public List<ResourceValue> cost;
    public List<SkillData> skills = new List<SkillData>();
    public InGameResource[] canProduce;
    public float attackRange;
    public int attackDamage;
    public float attackRate;
    public bool canBePromoted;
    public int XPGivenOnDeath;
    public int XPPromotionThreshold;
    public int maxXP;

    [Header("General Sounds")]
    public AudioClip onSelectSound;

    public bool CanBuy(int owner)
    {
        return Globals.CanBuy(owner, cost);
    }

    public List<ResourceValue> Cost { get { return cost; } }
    public string Code { get { return code; } }
    public string UnitName { get { return unitName; } }
    public string Description { get { return description; } }
    public int Healthpoints { get { return healthpoints; } }
}