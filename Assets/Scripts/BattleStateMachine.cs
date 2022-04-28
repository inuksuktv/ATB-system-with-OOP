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

                // Test if there is action queued up.
                if (actionQueue.Count > 0)
                {
                    battleState = BattleState.OrderAction;
                }

                break;

            case (BattleState.OrderAction):

                // Check if the performer is a hero or enemy so we can get its script and set it to act.
                GameObject performer = actionQueue[0].attackerGameObject;

                if (performer.CompareTag("Enemy"))
                {
                    // Set the performer's state to Acting.
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                    ESM.turnState = EnemyStateMachine.TurnState.Acting;
                }
                else if (performer.CompareTag("Hero"))
                {
                    // We could add a check here to redirect a hero's attack if its target is now dead

                    // Set the performer's state to Acting.
                    HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
                    HSM.turnState = HeroStateMachine.TurnState.Acting;
                }
                else
                {
                    Debug.Log("Error: Couldn't find the attacker.");
                }

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
}
