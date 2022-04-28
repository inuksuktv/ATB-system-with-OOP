using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : BaseAttack
{
    public Punch()
    {
        attackName = "Punch";
        attackDescription = "A solid thunk.";
        attackDamage = 15f;
    }
}
