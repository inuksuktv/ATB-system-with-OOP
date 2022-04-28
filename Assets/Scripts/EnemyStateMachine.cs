using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : BaseUnit
{
    private BattleStateMachine BSM;

    // Enemy turn states
    public enum TurnState
    {
        Preparing,
        Choosing,
        Idle,
        Acting,
        Dead
    }
    public TurnState turnState;

    // For the preparing phase
    private float elapsedCooldown = 0f;
    private float turnCooldown = 5f;

    // For the acting phase
    private bool actionStarted = false;
    private float animationSpeed = 15f;
    private Vector3 startPosition;
    public GameObject objectToAttack;

    // Alive check
    private bool alive = true;


    // Start is called before the first frame update
    void Start()
    {
        turnState = TurnState.Preparing;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch (turnState)
        {
            case (TurnState.Preparing):

                // Count up to the turnCooldown then Choosing
                PrepareCooldown();

                break;

            case (TurnState.Choosing):

                // If there's a hero, choose an action
                if (BSM.heroesInBattle.Count > 0)
                {
                    ChooseAction();
                }

                // Wait for your action to come to the front of the queue.
                turnState = TurnState.Idle;
                break;

            case (TurnState.Idle):

                // Wait for your turn in the action queue

                break;

            case (TurnState.Acting):

                // It's your time in the spotlight! Move the enemy and deal damage to the target. Tell the BSM when you're done moving and reset the enemy.
                StartCoroutine(TimeForAction());
                break;

            case (TurnState.Dead):



                break;

        }
    }

    private void PrepareCooldown()
    {
        // Record the time since this unit starting preparing.
        elapsedCooldown += Time.deltaTime;

        // If this unit's been preparing long enough, choose an action.
        if (elapsedCooldown >= turnCooldown)
        {
            turnState = TurnState.Choosing;
        }
    }

    private void ChooseAction()
    {
        // Populate AttackHandler's fields with this object's name, gameObject, and target
        AttackHandler myAttack = new AttackHandler();
        myAttack.attackerName = unitName;
        myAttack.attackerGameObject = gameObject;
        myAttack.attackTarget = BSM.heroesInBattle[Random.Range(0, BSM.heroesInBattle.Count)];

        // Pick a BaseAttack from this enemy's attackList
        int num = Random.Range(0, attackList.Count);
        myAttack.chosenAttack = attackList[num];

        // Tell the BSM to add myAttack to the actionQueue.
        BSM.CollectAction(myAttack);
    }

    private IEnumerator TimeForAction()
    {
        // Break if this Coroutine's already running.
        if (actionStarted)
        {
            yield break;
        }

        // Move the attacker near the target to attack.

        // Do damage

        // Wait a bit.
        
        // Move the attacker back to its starting position.

        // Remove this performer from the list in BSM.

        // Set the BSM's state to Available.

        // Set actionStarted false so the Coroutine breaks on the next frame

        // Reset this enemy's state
    }
}
