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

    // Non-UI Elements
    private Dictionary<Player, GameObject> playerStatsText;

    // Start is called before the first frame update
    void Start()
    {
        SetupPlayerStatsTextDictionary();
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
                if(i == GameManager.instance.CurrentTurn)
                    gameParent.transform.GetChild(i).GetComponent<Image>().color = Color.yellow;
				else
                    gameParent.transform.GetChild(i).GetComponent<Image>().color = Color.white;
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
                    UpdateRedDot(i, false);
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
        string updatedText = "Money: " + GameManager.instance.Players[currentTurn].CurrentMoney;

        // Only display the "+ income" text if it is that player's turn and they are still rolling for income
        if(GameManager.instance.CurrentTurnState == TurnState.Income)
            updatedText += " + " + GameManager.instance.TempIncome * GameManager.instance.Players[currentTurn].IncomeMulitplier;

        gameParent.transform.GetChild(currentTurn).GetChild(1).GetComponent<TMP_Text>().text = updatedText;
    }

    public void UpdateRedDot(int currentTurn, bool isToBeHiding)
	{
        gameParent.transform.GetChild(currentTurn).GetChild(2).gameObject.SetActive(isToBeHiding);
    }
}
