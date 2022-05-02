using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackHandler
{
    // ENCAPSULATION
    private string m_attackerName;
    public string attackerName {
        get { return m_attackerName; }
        set {
            if (value.Length > 12) {
                Debug.Log("attackerName limit is 15 characters.");
            } else {
                m_attackerName = value;
            }
        }
    }

    private string m_attackDescription;
    public string attackDescription
    {
        get { return m_attackDescription; }
        set {
            if (value.Length > 24) {
                Debug.Log("attackDescription limit is 30 characters.");
            } else {
                m_attackDescription = value;
            }
        }
    }

    // The attacker.
    public GameObject attackerGameObject;

    // The target. How do we handle multi-targeting?
    public GameObject attackTarget;

    // The attack being performed.
    public BaseAttack chosenAttack;
    
}
