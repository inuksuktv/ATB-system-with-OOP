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
                // Remove the hero's actions from the queue.
                for (int i = 0; i < BSM.actionQueue.Count; i++)
                {
                    //If there were any actions targeting this unit, choose a new target.
                    if (BSM.actionQueue[i].attackTarget == gameObject)
                    {
                        BSM.actionQueue[i].attackTarget = BSM.heroesInBattle[Random.Range(0, BSM.heroesInBattle.Count)];
                    }

                    //Remove this object's turn
                    if (BSM.actionQueue[i].attackerGameObject == gameObject)
                    {
                        BSM.actionQueue.Remove(BSM.actionQueue[i]);
                    }
                }
            }

            // Remove this unit from the GUI handler list.
            if (BSM.heroesToManage.Count > 0)
            {
                BSM.heroesToManage.Remove(gameObject);
            }

            // Set alive false
            alive = false;

            // Initialize the GUI.
            BSM.ClearActivePanel();

            //Check if battle is won or lost
            BSM.battleState = BattleStateMachine.BattleState.VictoryCheck;
        }
    }
}
