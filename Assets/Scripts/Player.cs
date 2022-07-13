using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
	#region Fields
	// Player info
	private Color color;

	// Money
	private int currentMoney;
	private int incomeMulitplier;

	// Land & Buildings
	private bool[] isLandClearedArr;
	private List<Building> buildings;

	// Town Meeting Cards
	private Dictionary<TownMeetingCardType, int> townMeetingCards;
	#endregion

	#region Properties
	public Color Color { get { return color; } }
	public int CurrentMoney { get { return currentMoney; } }
	public int IncomeMulitplier { get { return incomeMulitplier; } }
	#endregion

	/// <summary>
	/// Creates a new player
	/// </summary>
	/// <param name="color">The color of the player</param>
	public Player(Color color)
	{
		this.color = color;

		currentMoney = 15;
		incomeMulitplier = 1;

		SetupLand();
		SetupTownMeetingCardsDictionary();
	}

	#region Methods
	/// <summary>
	/// Creates an initial state for the player's board
	/// </summary>
	private void SetupLand()
	{
		// Sets all land to not be cleared
		isLandClearedArr = new bool[3];
		for(int i = 0; i < isLandClearedArr.Length; i++)
			isLandClearedArr[i] = false;

		// Creates all new buildings
		buildings = new List<Building>();

		List<Building> listOfBuildings = BuildingManager.instance.Buildings;
		foreach(Building building in listOfBuildings)
			buildings.Add(building.Clone());
	}

	/// <summary>
	/// Gets whether the player has cleared land
	/// </summary>
	/// <param name="tier">The tier of land</param>
	/// <returns>Whether the tier of land is cleared</returns>
	private bool IsLandCleared(BuildingTier tier)
	{
		return isLandClearedArr[(int)tier];
	}

	/// <summary>
	/// Whether the tier land can be cleared
	/// </summary>
	/// <param name="tier">The tier of land</param>
	/// <returns>Whether the land can be cleared</returns>
	private bool CanClearLand(BuildingTier tier)
	{
		// Make sure the player is at the right point in their turn
		if(GameManager.instance.CurrentTurnState != TurnState.BuyProperties)
		{
			Debug.Log("Wrong turn state");
			return false;
		}

		// Check that the land is not already cleared
		if(IsLandCleared(tier))
		{
			Debug.Log("Land already cleared");
			return false;
		}

		// Check if the player has enough money to clear the land
		if(currentMoney < 5)
		{
			Debug.Log("Not enough money");
			return false;
		}

		return true;
	}

	/// <summary>
	/// Clears land of a certain tier
	/// </summary>
	/// <param name="tier">The tier being cleared</param>
	public void ClearLand(BuildingTier tier)
	{
		if(!CanClearLand(tier))
			return;

		// Deduct the cost 		
		currentMoney -= 5;

		// Update UI
		UIManager.instance.UpdatePlayerStatsText(this);

		// Set the land to cleared
		isLandClearedArr[(int)tier] = true;

		// Hide the button
		UIManager.instance.HideLandButton((int)tier);

		// Advance the turn
		GameManager.instance.AdvanceTurn();
	}

	/// <summary>
	/// Checks if a player can buy/build a building
	/// </summary>
	/// <param name="building">The building that is being built</param>
	/// <returns>Whether the player can buy and build the building</returns>
	private bool CanBuild(Building building)
	{
		// Make sure the player is at the right point in their turn
		if(GameManager.instance.CurrentTurnState != TurnState.BuyProperties)
		{
			Debug.Log("Wrong turn state");
			return false;
		}

		// Check if the land has been cleared
		if(!IsLandCleared(building.BuildingTier))
		{
			Debug.Log("Land not cleared!");
			return false;
		}

		// Check if the player has not already built the building
		if(IsBuilt(building))
		{
			Debug.Log("The building is already built!");
			return false;
		}

		// Check if the player has enough money
		if(CurrentMoney < building.Cost)
		{
			Debug.Log("Not enough money!");
			return false;
		}

		return true;
	}

	/// <summary>
	/// Gets if a player has built a specific building
	/// </summary>
	/// <param name="building">The building being checked</param>
	/// <returns>Whether the player has built that building</returns>
	public bool IsBuilt(Building building)
	{
		foreach(Building playerBuilding in buildings)
			if(building.BuildingName == playerBuilding.BuildingName)
				return building.IsBought;

		return false;
	}

	/// <summary>
	/// Gets if a player has built a set of buildings
	/// </summary>
	/// <param name="buildingType">The type of building of the set that is being checked</param>
	/// <returns>Whether the player has built all 3 buildings of the set type</returns>
	public bool IsBuilt(BuildingTier buildingType)
	{
		// Determine an index offset based on the type of set is being checked 
		int indexOffset = (int)buildingType * 3;

		// Loop through the building set, if any haven't been bought, then the set is not bought
		for(int i = 0; i < 3; i++)
			if(!buildings[indexOffset + i].IsBought)
				return false;

		return true;
	}

	/// <summary>
	/// Builds a building
	/// </summary>
	/// <param name="building">The building being built</param>
	public void Build(Building building)
	{
		Debug.Log("test");

		// Check if the player can buy the building
		if(!CanBuild(building))
			return;

		// Allow other players to challenge the building
		// TODO: ask other players if they want to use a legal action card

		// "Build" the building
		foreach(Building playerBuilding in buildings)
			if(playerBuilding.BuildingName == building.BuildingName)
				playerBuilding.Buy();

		// Remove building cost from player
		currentMoney -= building.Cost;

		// Deselect Building
		BuildingManager.instance.SelectBuilding(null);

		// Update UI
		UIManager.instance.UpdatePlayerStatsText(this);

		// Update the player's income mulitplier changes
		PostBuildCheck();

		// Advance the turn
		GameManager.instance.AdvanceTurn();
	}

	/// <summary>
	/// Checks the player's buildings for their income multiplier and whether they won the game
	/// </summary>
	private void PostBuildCheck()
	{
		// Default multiplier amount
		int multiplier = 1;

		if(IsBuilt(BuildingTier.Tier1))
			multiplier++;

		if(IsBuilt(BuildingTier.Tier2))
			multiplier++;

		if(IsBuilt(BuildingTier.Tier3))
			multiplier++;

		// All sets are built, so all buildings are built,
		// so the player has won the game
		if(multiplier == 4)
			GameManager.instance.EndGame(this);
		else
			incomeMulitplier = multiplier;
	}

	/// <summary>
	/// Adds an amount of money to the player
	/// </summary>
	/// <param name="incomeAmount">The amount of money being added to the player</param>
	public void CollectIncome(int incomeAmount)
	{
		currentMoney += incomeAmount * incomeMulitplier;
	}

	/// <summary>
	/// Causes the player to lose a building
	/// </summary>
	/// <param name="building">The building the player will lose</param>
	public void LoseBuilding(Building building)
	{
		foreach(Building playerBuilding in buildings)
			if(playerBuilding.BuildingName == building.BuildingName)
				playerBuilding.Lose();
	}

	/// <summary>
	/// Setup the dictionary to hold the player's town meeting cards
	/// </summary>
	private void SetupTownMeetingCardsDictionary()
	{
		townMeetingCards = new Dictionary<TownMeetingCardType, int>();

		townMeetingCards.Add(TownMeetingCardType.LegalAction, 0);
		townMeetingCards.Add(TownMeetingCardType.Defense, 0);
	}

	public void BuyTownMeetingCard()
	{
		if(currentMoney >= 3)
		{
			currentMoney -= 3;
			TownMeetingCardType newlyBoughtCardType = TownMeetingManager.instance.BuyTownMeetingCard();
			townMeetingCards[newlyBoughtCardType]++;
			UIManager.instance.UpdatePlayerStatsText(this);
			GameManager.instance.ChangeTurnState(TurnState.BuyProperties);
		}
	}

	/// <summary>
	/// Plays a player's town meeting card
	/// </summary>
	/// <param name="cardType">Whether the card is a defense or legal action card</param>
	public void PlayTownMeetingCard(TownMeetingCardType cardType)
	{
		if(townMeetingCards[cardType] > 0)
		{
			townMeetingCards[cardType]--;
			TownMeetingManager.instance.AddPlayedCard(cardType);
		} else
			Debug.Log("Not enough " + cardType.ToString() + " cards");
	}
	#endregion

}
