using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton Code
    // A public reference to this script
    public static UIManager instance = null;

    // Awake is called even before start 
    // (I think its at the very beginning of runtime)
    private void Awake()
    {
        // If the reference for this script is null, assign it this script
        if(instance == null)
            instance = this;
        // If the reference is to something else (it already exists)
        // than this is not needed, thus destroy it
        else if(instance != this)
            Destroy(gameObject);
    }
    #endregion

    [SerializeField]
    private Canvas canvas;
    [SerializeField]    // Empty object parents
    private GameObject mainMenuParent, gameParent, gameEndParent;
    [SerializeField]    // Buttons
    private GameObject playButton, quitButton, gameEndToMainMenuButton;
    [SerializeField]    // Empty object parents (in gameParent)
    private GameObject playerStatsParent, mapParent, landParent;
    [SerializeField]    // Empty object parents (for each turn state related in gameParent)
    private GameObject turnSideParent, incomeParent, opprotunityCardParent, buyTMCardParent, selectedParent;

    // Non-UI Elements
    private Dictionary<Player, GameObject> playerStatsUI;

    // Start is called before the first frame update
    void Start()
    {
        SetupPlayerStatsParentsDictionary();
        SetupUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Set values and events for ui elements
    /// </summary>
    private void SetupUI()
	{
        // Menu Buttons
        playButton.GetComponent<Button>().onClick.AddListener(() => GameManager.instance.ChangeMenuState(MenuState.Game));
        quitButton.GetComponent<Button>().onClick.AddListener(() => Application.Quit());
        gameEndToMainMenuButton.GetComponent<Button>().onClick.AddListener(() => GameManager.instance.ChangeMenuState(MenuState.MainMenu));

        // Player Names Text
        for(int i = 0; i < GameManager.instance.Players.Count; i++)
		{
            Player player = GameManager.instance.Players[i];
            playerStatsUI[player].transform.GetChild(0).GetComponent<TMP_Text>().text = GameManager.instance.PlayerColors[player.Color] + " Player";
        }

		// Map Building Buttons
		foreach(Transform playerBuildings in mapParent.transform.GetChild(1))
            if(playerBuildings.tag == "playerBuildingParent")
			    foreach(Transform buildingChild in playerBuildings)
				    if(buildingChild.GetComponent<Button>() != null)
					    buildingChild.GetComponent<Button>().onClick.AddListener(() => SelectBuilding(buildingChild.gameObject.name));

        // Map Land Buttons
        foreach(Transform playerLandButtons in landParent.transform)
            for(int j = 0; j < playerLandButtons.childCount; j++)
                if(playerLandButtons.GetChild(j).GetComponent<Button>() != null)
				{
                    BuildingTier tier = (BuildingTier)j;
                    playerLandButtons.GetChild(j).GetComponent<Button>().onClick.AddListener(() => SelectLand(tier));
				}

        // Turn State Buttons
        incomeParent.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(
            () => GameManager.instance.RollIncome());
        incomeParent.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(
            () => GameManager.instance.ChangeTurnState(TurnState.OpprotunityCard));

        opprotunityCardParent.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(
            () => GameManager.instance.DrawOpprotunityCard());

        buyTMCardParent.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(
            () => GameManager.instance.CurrentPlayer.BuyTownMeetingCard());   // TODO: add random TM card to player
        buyTMCardParent.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(
            () => GameManager.instance.ChangeTurnState(TurnState.BuyProperties));

        selectedParent.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(
            () => BuildingManager.instance.CurrentPlayerBuildCurrentSelection());
        selectedParent.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(
            () => GameManager.instance.AdvanceTurn());
    }

    /// <summary>
    /// Runs recurring logic, updating UI elements
    /// </summary>
    private void UpdateUI()
    {
        
    }

    /// <summary>
    /// Update the UI based on the current menu state
    /// </summary>
    /// <param name="menuState">The current menu state</param>
    public void ChangeUI(MenuState menuState)
	{
        // Hide all UI groups 
        foreach(Transform child in canvas.transform)
            child.gameObject.SetActive(false);

        // Set the specific parent to be active
        switch(menuState)
        {
            case MenuState.MainMenu:
                mainMenuParent.gameObject.SetActive(true);
                break;
            case MenuState.Game:
                gameParent.gameObject.SetActive(true);

                // Hide all red dots
                for(int i = 0; i < playerStatsParent.transform.childCount; i++)
                    if(playerStatsParent.transform.GetChild(i).tag == "playerPanel")
                        UpdateRedDot(i, false);
                break;
            case MenuState.GameEnd:
                gameEndParent.gameObject.SetActive(true);
                break;
        }
	}

    /// <summary>
    /// Update the panels depending on the current turn
    /// </summary>
    public void UpdatePlayerTurnUI()
	{
        // Change the color of the panel of the player who's turn it is
        for(int i = 0; i < playerStatsParent.transform.childCount; i++)
        {
            if(playerStatsParent.transform.GetChild(i).tag != "playerPanel")
                continue;

            if(i == GameManager.instance.CurrentTurn)
                playerStatsParent.transform.GetChild(i).GetComponent<Image>().color = UnityEngine.Color.yellow;
            else
                playerStatsParent.transform.GetChild(i).GetComponent<Image>().color = UnityEngine.Color.white;
        }

        // Update current turn state UI
        foreach(Transform turnStateChildUI in turnSideParent.transform.GetChild(0))
            turnStateChildUI.gameObject.SetActive(false);
        switch(GameManager.instance.CurrentTurnState)
        {
            case TurnState.Income:
                incomeParent.SetActive(true);
                break;
            case TurnState.OpprotunityCard:
                opprotunityCardParent.SetActive(true);
                break;
            case TurnState.BuyTownMeetingCards:
                buyTMCardParent.SetActive(true);
                break;
            case TurnState.BuyProperties:
                selectedParent.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// Links a player object to the UI parent element that displays its stats
    /// </summary>
    private void SetupPlayerStatsParentsDictionary()
	{
        playerStatsUI = new Dictionary<Player, GameObject>();

        for(int i = 0; i < GameManager.instance.Players.Count; i++)
            playerStatsUI.Add(GameManager.instance.Players[i], playerStatsParent.transform.GetChild(i).GetChild(0).gameObject);
    }

    /// <summary>
    /// Update the text to display correct stats of a player
    /// </summary>
    /// <param name="player">The player whose stats are being updated</param>
    public void UpdatePlayerStatsText(Player player)
	{
        string updatedText = "Money: " + player.CurrentMoney;

        // Only display the "+ income" text if it is that player's turn and they are still rolling for income
        if(GameManager.instance.CurrentTurnState == TurnState.Income)
		{
            updatedText += " + " + GameManager.instance.TempIncome * player.IncomeMulitplier;
        } 
        else if(GameManager.instance.CurrentTurnState == TurnState.OpprotunityCard)
		{
            int totalIncome = GameManager.instance.TempIncome * player.IncomeMulitplier;
            if(totalIncome > 0)
                updatedText += " + " + totalIncome;
        }

        playerStatsUI[player].transform.GetChild(1).GetComponent<TMP_Text>().text = updatedText;
    }

    /// <summary>
    /// Updates a player's red dot sprite
    /// </summary>
    /// <param name="currentTurn">The player that is currently up</param>
    /// <param name="isToBeHiding">Whether the red dot will be hidden or revealed</param>
    public void UpdateRedDot(int currentTurn, bool isToBeHiding)
	{
        playerStatsUI[GameManager.instance.Players[currentTurn]].transform.GetChild(2).gameObject.SetActive(isToBeHiding);
    }

    /// <summary>
    /// Interprets a building button into selecting that building
    /// </summary>
    /// <param name="buildingName">The name of the building button</param>
    private void SelectBuilding(string buildingName)
	{
        Building selectedBuilding = BuildingManager.instance.GetBuildingByIndex(
            int.Parse(buildingName.Substring(8, 1)),
            int.Parse(buildingName.Substring(10, 1)));

        BuildingManager.instance.SelectBuilding(selectedBuilding);
	}

    /// <summary>
    /// Selects clearable land
    /// </summary>
    /// <param name="tier">The tier of land selected</param>
    private void SelectLand(BuildingTier tier)
	{
        BuildingManager.instance.SelectLand((int)tier);
	}

    /// <summary>
    /// Hide the land button that was clicked
    /// </summary>
    /// <param name="tier">The tier of land that was cleared</param>
    public void HideLandButton(int tier)
	{
        landParent.transform.GetChild(GameManager.instance.CurrentTurn).GetChild(tier).gameObject.SetActive(false);
    }

    /// <summary>
    /// Update the selected building panel and text with the building info
    /// </summary>
    /// <param name="selectedBuilding">The currently selected building</param>
    public void UpdateSelectedUI(Building selectedBuilding)
	{
        // Update the selected building
        if(selectedBuilding == null)
		{
            selectedParent.transform.GetChild(1).gameObject.SetActive(false);
            selectedParent.transform.GetChild(2).gameObject.SetActive(false);
		} else
        {
            selectedParent.transform.GetChild(1).gameObject.SetActive(true);
            selectedParent.transform.GetChild(2).gameObject.SetActive(true);

            selectedParent.transform.GetChild(1).GetComponent<TMP_Text>().text
                = selectedBuilding.FullName;
            selectedParent.transform.GetChild(2).GetComponent<TMP_Text>().text
                = "Cost: $" + selectedBuilding.Cost;
        }
    }

    /// <summary>
    /// Update the selected building panel and text with the land info
    /// </summary>
    /// <param name="landTier">The currently selected land's tier</param>
    public void UpdateSelectedUI(int landTier)
	{
        // Update the selected building
        if(landTier >= 0 && landTier < 3)
        {
            selectedParent.transform.GetChild(1).gameObject.SetActive(true);
            selectedParent.transform.GetChild(2).gameObject.SetActive(true);

            selectedParent.transform.GetChild(1).GetComponent<TMP_Text>().text
                = "Uncleared Land (Tier " + (landTier + 1) + ")";
            selectedParent.transform.GetChild(2).GetComponent<TMP_Text>().text
                = "Cost: $5";
        } else 
        { 
            selectedParent.transform.GetChild(1).gameObject.SetActive(false);
            selectedParent.transform.GetChild(2).gameObject.SetActive(false);
        }
    }
}
