using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        Available,
        Idle,
        Done
    }
    public HeroGUI heroInput;

    // List of heroes ready for input
    public List<GameObject> heroesToManage = new List<GameObject>();
    private AttackHandler heroChoice;

    // GUI objects.
    private GameObject activeHero;
    private GameObject activePanel;
    [SerializeField] private GameObject heroPanelPrefab;
    [SerializeField] private RectTransform battleCanvas;
    private GameObject infoBox;
    private RectTransform heroPanelTra;
    private Vector2 screenPoint;
    public List<GameObject> heroPanels = new List<GameObject>();

    // For mouseover selection
    public bool isSelecting = false;

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
            // For each new panel, set its parent as battleCanvas and get the RectTransform of the panel. Deactivate and add to heroPanels list.
            GameObject newPanel = Instantiate(heroPanelPrefab, Vector3.zero, Quaternion.identity);
            newPanel.name = hero.name + "Panel";
            newPanel.transform.SetParent(battleCanvas, false);
            heroPanelTra = newPanel.GetComponent<RectTransform>();
            heroPanelTra.localScale = new Vector3(1f, 1f, 1f);
            newPanel.SetActive(false);
            heroPanels.Add(newPanel);

            // Calculate *screen* position of hero (not a canvas / rectTransform).
            screenPoint = Camera.main.WorldToScreenPoint(hero.transform.position);

            Vector2 canvasPos;

            // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
            RectTransformUtility.ScreenPointToLocalPointInRectangle(battleCanvas, screenPoint, null, out canvasPos);

            // Position the panel.
            heroPanelTra.localPosition = canvasPos;
        }

        infoBox = battleCanvas.GetChild(0).gameObject;
        infoBox.SetActive(false);
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
                } else if (enemiesInBattle.Count < 1) {
                    battleState = BattleState.Win;
                } else {
                    // Clearing the inputs and panel every time a unit dies isn't ideal, but oh well.
                    ClearActivePanel();
                    isSelecting = false;
                    heroInput = HeroGUI.Available;
                }

            break;

            case (BattleState.Win):

                // Set heroes to idle.
                foreach (GameObject hero in heroesInBattle) {
                    hero.GetComponent<HeroStateMachine>().turnState = HeroStateMachine.TurnState.Idle;
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
            case (HeroGUI.Available):

                if (heroesToManage.Count > 0)
                {
                    activeHero = heroesToManage[0];

                    // Activate selector.
                    activeHero.transform.Find("Selector").gameObject.SetActive(true);

                    // Prepare the hero's input panel. We've already created it, so find the right children buttons and give them listeners to fill in some of heroChoice.
                    activePanel = battleCanvas.transform.Find(activeHero.name + "Panel").gameObject; // Panels were named in Start().
                    activePanel.SetActive(true);

                    // Find the buttons and add a listener.
                    foreach (RectTransform child in activePanel.transform) {
                        if (child.GetComponent<Image>() != null) {
                            child.GetComponent<Button>().onClick.AddListener(() => Input1(activeHero, child));
                        }
                    }

                    // Wait for the player's input.
                    heroInput = HeroGUI.Idle;
                }

                break;

            case (HeroGUI.Idle):



                break;

            case (HeroGUI.Done):

                // Send the action to the battle logic.
                CollectAction(heroChoice);

                ClearActivePanel();

                // Disable the selector, remove the hero from the GUI list, and initialize the GUI.
                activeHero.transform.Find("Selector").gameObject.SetActive(false);
                heroesToManage.RemoveAt(0);
                heroInput = HeroGUI.Available;

                break;

        }
    }

    public void ClearActivePanel()
    {
        // Enable all the arrow buttons again in case the player made a selection.
        foreach (RectTransform child in activePanel.transform) {
            if (child.gameObject.GetComponent<Image>() != null) {
                Image image = child.gameObject.GetComponent<Image>();
                image.enabled = true;
            }
        }

        // Hide the panels and infobox.
        foreach (GameObject panel in heroPanels) {
            panel.SetActive(false);
        }
        infoBox.SetActive(false);
    }

    public void CollectAction(AttackHandler input)
    {
        actionQueue.Add(input);
    }

    public void UpdateInfoBox(string str)
    {
        TextMeshProUGUI infoText = infoBox.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        infoText.text = str;
    }

    private void ExecuteAction()
    {
        // Get the performer's gameObject and script.
        GameObject performer = actionQueue[0].attackerGameObject;
        UnitStateMachine unitSM = performer.GetComponent<UnitStateMachine>();

        // Pass the target for the performer's Acting phase.
        unitSM.attackTarget = actionQueue[0].attackTarget;

        // Set the performer's state to Acting and self to Idle.
        unitSM.turnState = UnitStateMachine.TurnState.Acting;
        battleState = BattleState.Idle;
    }

    private void Input1(GameObject unit, Transform arrow)
    {
        heroChoice = new AttackHandler();
        
        // Get the chosen attack, which is a child of the button.
        BaseAttack attack = arrow.GetChild(0).gameObject.GetComponent<BaseAttack>();

        // Fill what fields we can for heroChoice here. We've attached the attack prefabs to the buttons. Get the attack's target on the next input.
        heroChoice.attackerName = unit.name;
        heroChoice.attackDescription = attack.attackDescription;
        heroChoice.chosenAttack = attack;
        heroChoice.attackerGameObject = unit;

        // Send the attackDescription to the infobox.
        infoBox.SetActive(true);
        UpdateInfoBox(attack.attackDescription);
        
        // Enables mouse selection on the enemies.
        isSelecting = true;

        // Hide all the buttons that weren't clicked.
        foreach (RectTransform child in arrow.parent.gameObject.transform) {
            if (child.gameObject.GetComponent<Image>() != null) {
                Image image = child.gameObject.GetComponent<Image>();
                image.enabled = false;
            }
        }

        // Show the button that was clicked again.
        Image arrowImage = arrow.GetComponent<Image>();
        arrowImage.enabled = true;
    }

    public void Input2(GameObject unit)
    {
        // Fill the last field for heroChoice.
        heroChoice.attackTarget = unit;

        // Disable mouse selection on the enemies and disable GUI elements.
        isSelecting = false;
        infoBox.SetActive(false);
        unit.transform.GetChild(0).gameObject.SetActive(false); // Selector.

        heroInput = HeroGUI.Done;
    }
}
