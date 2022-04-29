using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // HeroGUI handler states
    public enum HeroGUI
    {
        Activated,
        Idle,
        Done
    }
    public HeroGUI heroInput;

    // List of heroes ready for input
    public List<GameObject> heroesToManage = new List<GameObject>();
    private AttackHandler heroChoice;

    // GUI objects.
    [SerializeField] private GameObject heroPanelPrefab;
    [SerializeField] private RectTransform battleCanvas;
    private RectTransform heroPanelTra;
    private Vector2 screenPoint;

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

        // Create HeroGUI panels
        foreach (GameObject hero in heroesInBattle)
        {
            // For each new panel, set its parent as battleCanvas and get the RectTransform of the panel.
            GameObject newPanel = Instantiate(heroPanelPrefab, Vector3.zero, Quaternion.identity);
            newPanel.name = hero.name + "Panel";
            newPanel.transform.SetParent(battleCanvas, false);
            heroPanelTra = newPanel.GetComponent<RectTransform>();
            heroPanelTra.localScale = new Vector3(1f, 1f, 1f);
            newPanel.SetActive(false);

            // Calculate *screen* position of hero (not a canvas / rectTransform).
            screenPoint = Camera.main.WorldToScreenPoint(hero.transform.position);

            Vector2 canvasPos;

            // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
            RectTransformUtility.ScreenPointToLocalPointInRectangle(battleCanvas, screenPoint, null, out canvasPos);

            // Position the panel.
            heroPanelTra.localPosition = canvasPos;
        }
    }

    private void Update()
    {
        // Update the battle handler.
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

        // Update the GUI handler
        switch (heroInput)
        {
            case (HeroGUI.Activated):

                if (heroesToManage.Count > 0)
                {
                    heroesToManage[0].transform.Find("Selector").gameObject.SetActive(true);
                    heroChoice = new AttackHandler();

                    // Prepare the hero's input panel. We've already created it, so find the right children buttons and give them listeners to fill in some of heroChoice.
                    GameObject panel = battleCanvas.transform.Find(heroesToManage[0].name + "Panel").gameObject;
                    panel.SetActive(true);

                    // Find the buttons.
                    GameObject upArrow = panel.transform.Find("ArrowUp").gameObject;
                    GameObject leftArrow = panel.transform.Find("ArrowLeft").gameObject;
                    GameObject rightArrow = panel.transform.Find("ArrowRight").gameObject;
                    GameObject downArrow = panel.transform.Find("ArrowDown").gameObject;

                    // Add a listener to each button component.
                    upArrow.GetComponent<Button>().onClick.AddListener(() => Input1(upArrow.name));
                    leftArrow.GetComponent<Button>().onClick.AddListener(() => Input1(leftArrow.name));
                    rightArrow.GetComponent<Button>().onClick.AddListener(() => Input1(rightArrow.name));
                    downArrow.GetComponent<Button>().onClick.AddListener(() => Input1(downArrow.name));

                    heroInput = HeroGUI.Idle;
                }

                break;

            case (HeroGUI.Idle):



                break;

            case (HeroGUI.Done):



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

    private void Input1(string str)
    {
        Debug.Log(heroesToManage[0] + " clicked " + str);
    }
}
