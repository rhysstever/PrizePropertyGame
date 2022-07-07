using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuState
{
    MainMenu,
    Game,
    GameEnd
}

public enum TurnState
{
    Menus,
    Income,
    OpprotunityCard,
    BuyTownMeetingCards,
    BuyProperties
}

public class GameManager : MonoBehaviour
{
    #region Singleton Code
    // A public reference to this script
    public static GameManager instance = null;

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

    // Players
    private Dictionary<Color, string> playerColors;
    private List<Player> players;
    private int currentTurn;

    // States
    private MenuState currentMenuState;
    private TurnState currentTurnState;

    // Misc Values
    private int tempIncome;

    // Start is called before the first frame update
    void Start()
    {
        SetupPlayerColorDictionary();
        CreatePlayers();

        currentTurn = -1;
        ChangeMenuState(MenuState.MainMenu);
        ChangeTurnState(TurnState.Menus);

        tempIncome = 0;
    }

    // Update is called once per frame
    void Update()
    {
        CheckTurnActions();
    }

    // Getters
    public Dictionary<Color, string> PlayerColors { get { return playerColors; } }
    public List<Player> Players { get { return players; } }
    public int CurrentTurn { get { return currentTurn; } }
    public MenuState CurrentMenuState { get { return currentMenuState; } }
    public TurnState CurrentTurnState { get { return currentTurnState; } }
    public int TempIncome { get { return tempIncome; } }

    /// <summary>
    /// Performs initial, one-time logic for when the menu state changes
    /// </summary>
    /// <param name="newMenuState">The new menu state</param>
    public void ChangeMenuState(MenuState newMenuState)
	{
        switch(newMenuState)
        {
            case MenuState.MainMenu:
                currentTurn = -1;
                ChangeTurnState(TurnState.Menus);
                break;
            case MenuState.Game:
                currentTurn = 0;
                ChangeTurnState(TurnState.Income);
                break;
            case MenuState.GameEnd:
                currentTurn = -1;
                ChangeTurnState(TurnState.Menus);
                break;
        }

        currentMenuState = newMenuState;

        // Update UI
        UIManager.instance.ChangeUI(currentMenuState);
	}

    /// <summary>
    /// Performs initial, one-time logic for when the turn state changes
    /// </summary>
    /// <param name="newTurnState">The new turn state</param>
    public void ChangeTurnState(TurnState newTurnState)
	{
        switch(newTurnState)
        {
            case TurnState.Menus:
                break;
            case TurnState.Income:
                tempIncome = 0;
                break;
            case TurnState.OpprotunityCard:
                break;
            case TurnState.BuyTownMeetingCards:
                tempIncome = 0;
                UIManager.instance.UpdateRedDot(CurrentTurn, false);
                break;
            case TurnState.BuyProperties:
                break;
        }

        currentTurnState = newTurnState;
	}

    /// <summary>
    /// Fills the dictionary of player colors
    /// </summary>
    private void SetupPlayerColorDictionary()
	{
        playerColors = new Dictionary<Color, string>();

        playerColors.Add(Color.red, "Red");
        playerColors.Add(Color.blue, "Blue");
        playerColors.Add(Color.green, "Green");
        playerColors.Add(Color.yellow, "Yellow");
    }

    /// <summary>
    /// Creates the players for the game
    /// </summary>
    private void CreatePlayers()
	{
        players = new List<Player>();

        // Add players
        players.Add(new Player(Color.red));
        players.Add(new Player(Color.blue));
        players.Add(new Player(Color.green));
        players.Add(new Player(Color.yellow));
    }

    /// <summary>
    /// Recurring checks for actions that can be taken on the player's turn
    /// </summary>
    private void CheckTurnActions()
	{
        switch(currentTurnState)
        {
            case TurnState.Income:
                GenerateIncome();
                break;
            case TurnState.OpprotunityCard:
                if(Input.GetKeyDown(KeyCode.Return))
				{
                    players[currentTurn].CollectIncome(tempIncome);
                    ChangeTurnState(TurnState.BuyTownMeetingCards);
				}
                break;
            case TurnState.BuyTownMeetingCards:
                if(Input.GetKeyDown(KeyCode.Return))
                    ChangeTurnState(TurnState.BuyProperties);
                break;
            case TurnState.BuyProperties:
                if(Input.GetKeyDown(KeyCode.Return))
				{
                    // Advance the turn to the next player
                    currentTurn++;
                    if(currentTurn == players.Count)
                        currentTurn = 0;

                    ChangeTurnState(TurnState.Income);
				}                    
                break;
        }
    }

    /// <summary>
    /// Generate income as the player rolls the dice
    /// </summary>
    private void GenerateIncome()
	{
        // Press 'R' to roll the dice
		if(Input.GetKeyDown(KeyCode.R))
		{
            int result = Roll();
            if(result == 0)
            {
                // If a '0' is rolled (a red dot) the player gets 0 income for this turn
                tempIncome = 0;
                UIManager.instance.UpdateRedDot(CurrentTurn, true);
                ChangeTurnState(TurnState.OpprotunityCard);
			}
            else 
                tempIncome += result;
		}
        // Press 'T' to "stay"; the player will gain the income accumulated
        else if(Input.GetKeyDown(KeyCode.T))
            ChangeTurnState(TurnState.OpprotunityCard);
	}

    /// <summary>
    /// Roll the dice
    /// </summary>
    /// <returns>The outcome of the dice, 0 meaning the red dot</returns>
    private int Roll()
	{
        // Make a roll from 1-6
        int roll = UnityEngine.Random.Range(1, 7);

        // Roll     :   Outcome
        // 1        :   Red dot - no income awarded this turn 
        // 2        :   +2 income this turn 
        // 3        :   +3 income this turn 
        // 4        :   +1 income this turn
        // 5        :   +1 income this turn
        // 6        :   +1 income this turn
        if(roll == 1)
            return 0;
        else if(roll < 4)
            return roll;
        else
            return 1;
	}

    /// <summary>
    /// Ends the game
    /// </summary>
    /// <param name="winner">The player that has won the game</param>
    public void EndGame(Player winner)
	{
        ChangeMenuState(MenuState.GameEnd);
	}
}
