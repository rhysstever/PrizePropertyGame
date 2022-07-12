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
    [SerializeField]
    private GameObject mapParent;

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
        UpdateUI();
    }

    /// <summary>
    /// Set values and events for ui elements
    /// </summary>
    private void SetupUI()
	{
        // Events
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
		for(int i = 0; i < mapParent.transform.childCount; i++)
            if(mapParent.transform.GetChild(i).tag == "playerBuildingParent")
			    foreach(Transform buildingChild in mapParent.transform.GetChild(i))
				    if(buildingChild.GetComponent<Button>() != null)
					    buildingChild.GetComponent<Button>().onClick.AddListener(() => SelectBuilding(buildingChild.gameObject.name));
	}

    /// <summary>
    /// Runs recurring logic, updating UI elements
    /// </summary>
    private void UpdateUI()
    {
        if(GameManager.instance.CurrentMenuState == MenuState.Game)
		{
            // Based on which player is taking their turn, update their stats text
            UpdatePlayerStatsText(GameManager.instance.CurrentTurn);

            // Change the color of the panel of the player who's turn it is
            for(int i = 0; i < gameParent.transform.childCount; i++)
			{
                if(gameParent.transform.GetChild(i).tag != "playerPanel")
                    continue;

                if(i == GameManager.instance.CurrentTurn
                    && GameManager.instance.CurrentTurnState == TurnState.Income)
					gameParent.transform.GetChild(i).GetComponent<Image>().color = UnityEngine.Color.yellow;
                else if(i == GameManager.instance.CurrentTurn)
					gameParent.transform.GetChild(i).GetComponent<Image>().color = UnityEngine.Color.green;
                else
					gameParent.transform.GetChild(i).GetComponent<Image>().color = UnityEngine.Color.white;
			}
        }
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
                for(int i = 0; i < gameParent.transform.childCount; i++)
                    if(gameParent.transform.GetChild(i).tag == "playerPanel")
                        UpdateRedDot(i, false);
                break;
            case MenuState.GameEnd:
                gameEndParent.gameObject.SetActive(true);
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
            playerStatsUI.Add(GameManager.instance.Players[i], gameParent.transform.GetChild(i).GetChild(0).gameObject);
    }

    /// <summary>
    /// Update the text to display correct stats of a player
    /// </summary>
    /// <param name="currentTurn">The index of the player that currently is taking their turn</param>
    private void UpdatePlayerStatsText(int currentTurn)
	{
        string updatedText = "Money: " + GameManager.instance.Players[currentTurn].CurrentMoney;

        // Only display the "+ income" text if it is that player's turn and they are still rolling for income
        if(GameManager.instance.CurrentTurnState == TurnState.Income)
		{
            updatedText += " + " + GameManager.instance.TempIncome * GameManager.instance.Players[currentTurn].IncomeMulitplier;
        } 
        else if(GameManager.instance.CurrentTurnState == TurnState.OpprotunityCard)
		{
            int totalIncome = GameManager.instance.TempIncome * GameManager.instance.Players[currentTurn].IncomeMulitplier;
            if(totalIncome > 0)
                updatedText += " + " + totalIncome;
        }

        playerStatsUI[GameManager.instance.Players[currentTurn]].transform.GetChild(1).GetComponent<TMP_Text>().text = updatedText;
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
    public void SelectBuilding(string buildingName)
	{
        Building selectedBuilding = BuildingManager.instance.GetBuildingByIndex(
            int.Parse(buildingName.Substring(8, 1)),
            int.Parse(buildingName.Substring(10, 1)));

        BuildingManager.instance.SelectBuilding(selectedBuilding);
	}
}
