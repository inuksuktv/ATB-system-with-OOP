using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackHandler
{
    public string attackerName;
    public string attackDescription;

    // Enemy or hero attacker. Skipped this data by using tags instead, I think.
    // public string type

    // The attacker.
    public GameObject attackerGameObject;

    // The target. How do we handle multi-targeting?
    public GameObject attackTarget;

    // The attack being performed.
    public BaseAttack chosenAttack;
    
}
