using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public string unitName;
    public float baseHP, currentHP, baseATK, currentATK, magicATK, baseDEF, currentDEF;
    public List<BaseAttack> attackList = new List<BaseAttack>();
}
