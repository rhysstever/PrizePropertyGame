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
    [SerializeField]    // Players' stats text objects
    private GameObject player1StatsText, player2StatsText, player3StatsText, player4StatsText;
    [SerializeField]
    private GameObject player1TurnMarker, player2TurnMarker, player3TurnMarker, player4TurnMarker;

    // Non-UI Elements
    private Dictionary<Player, GameObject> playerStatsText;
    private List<GameObject> playerTurnMarkers;

    // Start is called before the first frame update
    void Start()
    {
        SetupPlayerStatsTextDictionary();
        SetupPlayerTurnMarkersList();
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

        // Set number values for each player stats text
        for(int i = 0; i < GameManager.instance.Players.Count; i++)
            UpdatePlayerStatsText(i);
    }

    private void UpdateUI()
    {
        // End early if it is not the game menu state
        if(GameManager.instance.CurrentMenuState != MenuState.Game)
            return;

        // Based on which player is taking their turn, update their stats text
        UpdatePlayerStatsText(GameManager.instance.CurrentTurn);
        UpdateTurnMarkers(GameManager.instance.CurrentTurn);
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
                break;
            case MenuState.GameEnd:
                gameEndParent.gameObject.SetActive(true);
                break;
        }
	}

    /// <summary>
    /// Links a player to the UI element that displays its stats
    /// </summary>
    private void SetupPlayerStatsTextDictionary()
	{
        playerStatsText = new Dictionary<Player, GameObject>();

        playerStatsText.Add(GameManager.instance.Players[0], player1StatsText);
        playerStatsText.Add(GameManager.instance.Players[1], player2StatsText);
        playerStatsText.Add(GameManager.instance.Players[2], player3StatsText);
        playerStatsText.Add(GameManager.instance.Players[3], player4StatsText);
    }

    /// <summary>
    /// Update the text to display correct stats of a player
    /// </summary>
    /// <param name="currentTurn">The index of the player that currently is taking their turn</param>
    private void UpdatePlayerStatsText(int currentTurn)
	{
        string updatedText = "Income: " + GameManager.instance.TempIncome + "\nMoney: " + GameManager.instance.Players[currentTurn].CurrentMoney;
        playerStatsText[GameManager.instance.Players[currentTurn]].GetComponent<TMP_Text>().text = updatedText;
    }

    private void SetupPlayerTurnMarkersList()
	{
        playerTurnMarkers = new List<GameObject>();

        playerTurnMarkers.Add(player1TurnMarker);
        playerTurnMarkers.Add(player2TurnMarker);
        playerTurnMarkers.Add(player3TurnMarker);
        playerTurnMarkers.Add(player4TurnMarker);
    }

    public void UpdateTurnMarkers(int currentTurn)
	{
        for(int i = 0; i < playerTurnMarkers.Count; i++)
		{
            if(i == currentTurn)
                playerTurnMarkers[i].SetActive(true);
            else 
                playerTurnMarkers[i].SetActive(false);
		}
	}
}
