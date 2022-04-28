using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : BaseUnit
{
    private BattleStateMachine BSM;

    // Turn states to control the state machine.
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
    private float animationSpeed = 20f;
    private Vector3 startPosition;
    public GameObject attackTarget;

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

                // Count up to the turnCooldown then set Choosing.
                PrepareCooldown();

                break;

            case (TurnState.Choosing):

                // If there's a hero, choose an action and put it in the actionQueue.
                if (BSM.heroesInBattle.Count > 0)
                {
                    ChooseAction();
                }

                // WSet Idle and wait for your action to come to the front of the queue.
                turnState = TurnState.Idle;
                break;

            case (TurnState.Idle):

                // Wait for your turn in the actionQueue.

                break;

            case (TurnState.Acting):

                // It's your time in the spotlight! Move and deal damage to the target. Tell the BSM when you're done moving and reset the enemy.
                StartCoroutine(TimeForAction());
                break;

            case (TurnState.Dead):



                break;

        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHP -= damageAmount;
        if (currentHP <= 0)
        {
            currentHP = 0;
            turnState = TurnState.Dead;
        }
    }

    private IEnumerator TimeForAction()
    {
        // Break if this Coroutine's already running.
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        // Move the attacker near the target to attack. Hold the coroutine until movement finishes.
        Vector3 position = new Vector3(attackTarget.transform.position.x, transform.position.y, attackTarget.transform.position.z + 4f);
        while (MovingTo(position)) { yield return null; }

        // Do damage
        AttackHandler myAttack = BSM.actionQueue[0];
        DoDamage(myAttack);

        // Wait a bit.
        yield return new WaitForSeconds(0.5f);

        // Move the attacker back to its starting position. Hold the coroutine until movement finishes.
        Vector3 readyPosition = startPosition;
        while (MovingTo(readyPosition)) { yield return null; }

        // Remove this performer's attack from the list in BSM.
        BSM.actionQueue.RemoveAt(0);

        // Set the BSM's state to Available.
        BSM.battleState = BattleStateMachine.BattleState.Available;

        // Set actionStarted false so the Coroutine doesn't break next time it's called.
        actionStarted = false;

        // Reset this enemy's state
        elapsedCooldown = 0f;
        turnState = TurnState.Preparing;
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

    private void DoDamage(AttackHandler myAttack)
    {
        float calcDamage = currentATK + myAttack.chosenAttack.attackDamage;
        attackTarget.GetComponent<HeroStateMachine>().TakeDamage(calcDamage);
    }

    private bool MovingTo(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animationSpeed * Time.deltaTime));
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
}
