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

    // List of heroes ready for input
    public List<GameObject> heroesToManage = new List<GameObject>();

    private void Awake()
    {
        
    }

    private void Start()
    {
        // Initialize the battle state.
        battleState = BattleState.Available;

        // Find heroes and enemies in the scene with tags. Later we can replace these by reading the information from the GameManager during Awake().
        enemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Unit"));
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

                if (heroesInBattle.Count < 1) {
                    battleState = BattleState.Lose;
                }
                else if (enemiesInBattle.Count < 1) {
                    battleState = BattleState.Win;
                }
                else {
                    // Refresh the HeroGUI.
                }

                break;

            case (BattleState.Win):

                // Set heroes to idle.
                for (int i = 0; i < heroesInBattle.Count; i++)
                {
                    heroesInBattle[i].GetComponent<HeroStateMachine>().turnState = HeroStateMachine.TurnState.Idle;
                }

                Debug.Log("You won the battle.");

                // Reset scene to world state.

                break;

            case (BattleState.Lose):

                Debug.Log("You lost the battle.");

                break;

        }
    }

    public void CollectAction(AttackHandler input)
    {
        actionQueue.Add(input);
    }

    private void ExecuteAction()
    {
        // Get the performer's gameObject and script.
        GameObject performer = actionQueue[0].attackerGameObject;
        BaseStateMachine unitSM = performer.GetComponent<BaseStateMachine>();

        // Pass the target for the performer's Acting phase.
        unitSM.attackTarget = actionQueue[0].attackTarget;

        // Set the performer's state to Acting and self to Idle.
        unitSM.turnState = BaseStateMachine.TurnState.Acting;
        battleState = BattleState.Idle;
      
        // We could add a check here to redirect a hero's attack on the actionQueue if its target is now dead.
    }
}
