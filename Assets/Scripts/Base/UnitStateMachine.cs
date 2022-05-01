using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UnitStateMachine : BaseUnit, IPointerClickHandler
{
    protected BattleStateMachine BSM;

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
    protected float elapsedCooldown = 0f;
    protected float turnCooldown = 5f;

    // For the acting phase
    protected bool actionStarted = false;
    private float animationSpeed = 20f;
    protected Vector3 startPosition;
    public GameObject attackTarget;

    // GUI
    public GameObject selector;
    private GameObject infoBox;
    private string oldInfoText;

    // Alive check
    protected bool alive = true;

    void OnMouseEnter()
    {
        if (BSM.isSelecting && gameObject.CompareTag("Unit"))
        {
            // Store the info box's current text.
            TextMeshProUGUI infoText = infoBox.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
            oldInfoText = infoText.text;

            selector.SetActive(true);
            BSM.UpdateInfoBox(gameObject.name);
        }
    }

    void OnMouseExit()
    {
        if (BSM.isSelecting && gameObject.CompareTag("Unit")) { 
            BSM.UpdateInfoBox(oldInfoText);
            selector.SetActive(false);
        }
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (BSM.isSelecting && gameObject.CompareTag("Unit")) 
        { 
            BSM.Input2(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        startPosition = transform.position;
        selector.SetActive(false);
        turnState = TurnState.Preparing;
        infoBox = GameObject.Find("BattleCanvas").transform.GetChild(0).gameObject;
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
                ChooseAction();

            break;

            case (TurnState.Idle):

                // Wait for your turn in the actionQueue.

            break;

            case (TurnState.Acting):

                // It's your time in the spotlight! Move and deal damage to the target. Tell the BSM when you're done moving and reset the enemy.
                StartCoroutine(TimeForAction());

            break;

            case (TurnState.Dead):

                // Change the tag, remove from BSM lists, deactivate selector, change colour, repopulate target buttons, set CheckVictory.
                DieAndCleanup();

            break;
        }
    }

    public virtual void ChooseAction()
    {
        if (BSM.heroesInBattle.Count > 0)
        {
            // Populate myAttack's fields with this object's name, gameObject, and target.
            AttackHandler myAttack = new AttackHandler();
            myAttack.attackerName = unitName;
            myAttack.attackerGameObject = gameObject;
            myAttack.attackTarget = BSM.heroesInBattle[Random.Range(0, BSM.heroesInBattle.Count)];

            // Pick a BaseAttack from this object's attackList.
            int num = Random.Range(0, attackList.Count);
            myAttack.chosenAttack = attackList[num];

            // Tell the BSM to add myAttack to the actionQueue.
            BSM.CollectAction(myAttack);

            // Set Idle and wait for your action to come to the front of the queue.
            turnState = TurnState.Idle;
        }
    }

    public virtual void DieAndCleanup()
    {
        if (!alive) { return; }
        else {
            // Change the unit's tag
            gameObject.tag = "DeadUnit";

            // Remove this object from the enemiesInBattle list. Used for VictoryCheck and to pick attackTarget among others.
            BSM.enemiesInBattle.Remove(gameObject);

            // Disable selector.
            selector.SetActive(false);

            // Change the color to grey (or play death animations?).
            gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);

            // Clean up the actionQueue.
            if (BSM.enemiesInBattle.Count > 0)
            {
                for (int i = 0; i < BSM.actionQueue.Count; i++)
                {
                    //If there were any actions targeting this unit, choose a new target
                    if (BSM.actionQueue[i].attackTarget == gameObject)
                    {
                        BSM.actionQueue[i].attackTarget = BSM.enemiesInBattle[Random.Range(0, BSM.enemiesInBattle.Count)];
                    }

                    //Remove this object's turn
                    if (BSM.actionQueue[i].attackerGameObject == gameObject)
                    {
                        BSM.actionQueue.Remove(BSM.actionQueue[i]);
                    }
                }
            }

            // Set alive false
            alive = false;

            // Initialize the GUI.
            BSM.ClearActivePanel();

            //Check if battle is won or lost
            BSM.battleState = BattleStateMachine.BattleState.VictoryCheck;
        }
    }

    private void DoDamage(AttackHandler myAttack)
    {
        float calcDamage = currentATK + myAttack.chosenAttack.attackDamage;
        attackTarget.GetComponent<UnitStateMachine>().TakeDamage(calcDamage);
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

    private void TakeDamage(float damageAmount)
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

        // Move the attacker until it's close to the target.
        Vector3 heroPosition = new Vector3(attackTarget.transform.position.x, transform.position.y, attackTarget.transform.position.z);
        while (MoveToTarget(heroPosition)) { yield return null; }

        // Load myAttack from the actionQueue and DoDamage.
        AttackHandler myAttack = BSM.actionQueue[0];
        DoDamage(myAttack);

        // Wait a bit.
        yield return new WaitForSeconds(0.5f);

        // Move the attacker back to its starting position. Hold the coroutine until movement finishes.
        Vector3 firstPosition = startPosition;
        while (MoveBack(firstPosition)) { yield return null; }

        // Remove this performer's attack from the list in BSM.
        BSM.actionQueue.RemoveAt(0);

        // Set the BSM's state to Available.
        BSM.battleState = BattleStateMachine.BattleState.Available;

        // Set actionStarted false to initialize the coroutine.
        actionStarted = false;

        // Reset this object's state
        elapsedCooldown = 0f;
        turnState = TurnState.Preparing;
    }

    private bool MoveToTarget(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, animationSpeed * Time.deltaTime);
        return (Vector3.Distance(transform.position, target) > 3f);
    }

    private bool MoveBack(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, animationSpeed * Time.deltaTime);
        return transform.position != target;
    }
}
