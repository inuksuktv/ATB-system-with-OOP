using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroStateMachine : UnitStateMachine
{
    public override void PrepareCooldown()
    {
        base.PrepareCooldown();

        // Set the progressBar transform to scale based on the cooldown to produce the ATB GUI.
        // We don't have a progressBar with the current design.
        //float calcCooldown = elapsedCooldown / turnCooldown;
        //progressBar.transform.localScale = new Vector3(Mathf.Clamp(calcCooldown, 0, 1), progressBar.transform.localScale.y, progressBar.transform.localScale.z);
    }

    // Add the object for the HeroGUI state machine to manage.
    public override void ChooseAction()
    {
        BSM.heroesToManage.Add(gameObject);
        turnState = TurnState.Idle;
    }

    // Return if the unit's already died and cleaned up.
    // Change the tag, remove from BSM lists, deactivate selector, change colour, repopulate target buttons, set CheckVictory.
    public override void DieAndCleanup()
    {
        if (!alive) { return; }
        else {
            // Change the unit's tag
            gameObject.tag = "DeadHero";

            // Remove this object from the heroesInBattle list. Used for VictoryCheck and to pick attackTarget among others.
            BSM.heroesInBattle.Remove(gameObject);

            // Disable selector.
            selector.SetActive(false);

            // Change the color to grey (or play death animations?).
            gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);

            // Clean up the actionQueue.
            if (BSM.heroesInBattle.Count > 0)
            {
                // Skipping i=0 because TimeForAction() is removing the first object in the actionQueue.
                // Shouldn't this throw an exception if actionQueue.Count is small?
                for (int i = 1; i < BSM.actionQueue.Count; i++)
                {
                    // Remove any action where this object is the attacker from the queue. 
                    // I decided to keep enemies targeting dead heroes, not removing an action if this object was the target.
                    if (BSM.actionQueue[i].attackerGameObject == gameObject)
                    {
                        BSM.actionQueue.Remove(BSM.actionQueue[i]);
                    }
                }
            }

            // Set alive false
            alive = false;

            // Reset enemyButtons for the target panel.
            // We may take a different approach with buttons here so this line will likely change.
            //BSM.EnemyButtons();

            //Check if battle is won or lost
            BSM.battleState = BattleStateMachine.BattleState.VictoryCheck;
        }
    }
}
