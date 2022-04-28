using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateMachine : MonoBehaviour
{
    // Battle handler states
    public enum BattleState
    {
        Available,
        OrderAction,
        Idle,
        VictoryCheck,
        Win,
        Lose
    }
    public BattleState battleState;

    // Lists of actions, heroes, and enemies for the battle logic
    public List<AttackHandler> actionQueue = new List<AttackHandler>();
    public List<GameObject> heroesInBattle = new List<GameObject>();
    public List<GameObject> enemiesInBattle = new List<GameObject>();

    private void Awake()
    {
        
    }

    private void Start()
    {
        // Initialize the battle state.
        battleState = BattleState.Available;

        // Find heroes and enemies in the scene with tags. Later we can replace these by reading the information from the GameManager during Awake().
        enemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        heroesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
    }

    private void Update()
    {
        switch (battleState)
        {
            case (BattleState.Available):

                // Change to OrderAction if there is an action to perform.
                if (actionQueue.Count > 0)
                {
                    battleState = BattleState.OrderAction;
                }

                break;

            case (BattleState.OrderAction):

                // Get the performer's script from the front of the queue and tell it to act. Set performer to Acting and self to Idle.
                ExecuteAction();

                break;

            case (BattleState.Idle):

                break;

            case (BattleState.VictoryCheck):



                break;

            case (BattleState.Win):



                break;

            case (BattleState.Lose):



                break;

        }
    }

    public void CollectAction(AttackHandler input)
    {
        actionQueue.Add(input);
    }

    private void ExecuteAction()
    {
        GameObject performer = actionQueue[0].attackerGameObject;

        if (performer.CompareTag("Enemy"))
        {
            EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();

            // Pass the target for the performer's Acting phase.
            ESM.attackTarget = actionQueue[0].attackTarget;

            // Set the performer's state to Acting and self to Idle.
            ESM.turnState = EnemyStateMachine.TurnState.Acting;
            battleState = BattleState.Idle;
        }
        else if (performer.CompareTag("Hero"))
        {
            HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();

            // We could add a check here to redirect a hero's attack on the actionQueue if its target is now dead.

            // Pass the target for the performer's Acting phase.
            HSM.attackTarget = actionQueue[0].attackTarget;

            // Set the performer's state to Acting and self to Idle.
            HSM.turnState = HeroStateMachine.TurnState.Acting;
            battleState = BattleState.Idle;
        }
        else
        {
            Debug.Log("Error: Couldn't find the attacker. Removing the attack from queue.");
            actionQueue.RemoveAt(0);
            battleState = BattleState.Available;
        }
    }
}
