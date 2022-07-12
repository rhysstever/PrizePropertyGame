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
	private bool isTier1LandCleared;
	private bool isTier2LandCleared;
	private bool isTier3LandCleared;
	private List<Building> buildings;

	// Town Meeting Cards
	private int legalActionCardCount;
	private int defenseCardCount;
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

		legalActionCardCount = 0;
		defenseCardCount = 0;
	}

	#region Methods
	/// <summary>
	/// Creates an initial state for the player's board
	/// </summary>
	private void SetupLand()
	{
		// Sets all land to not be cleared
		isTier1LandCleared = false;
		isTier2LandCleared = false;
		isTier3LandCleared = false;

		// Creates all new buildings
		buildings = new List<Building>();

		List<Building> listOfBuildings = BuildingManager.instance.Buildings;
		foreach(Building building in listOfBuildings)
			buildings.Add(building.Clone());
	}

	/// <summary>
	/// Builds a building
	/// </summary>
	/// <param name="building">The building being built</param>
	public void Build(Building building)
	{
		// Check if the player can buy the building
		if(!BuildingManager.instance.CanBuild(this, building))
			return;

		// Allow other players to challenge the building
		// TODO: ask other players if they want to use a legal action card
		
		// "Build" the building
		foreach(Building playerBuilding in buildings)
			if(playerBuilding.BuildingName == building.BuildingName)
				playerBuilding.Buy();

		// Remove building cost from player
		currentMoney -= building.Cost;

		Debug.Log(building.FullName + " built");

		// Update the player's income mulitplier changes
		PostBuildCheck();
	}

	/// <summary>
	/// Checks the player's buildings for their income multiplier and whether they won the game
	/// </summary>
	public void PostBuildCheck()
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
	/// Gets whether the player has cleared land
	/// </summary>
	/// <param name="tier">The tier of land</param>
	/// <returns>Whether the tier of land is cleared</returns>
	public bool IsLandCleared(BuildingTier tier)
	{
		if(tier == BuildingTier.Tier1)
			return isTier1LandCleared;
		else if(tier == BuildingTier.Tier2)
			return isTier2LandCleared;
		else if(tier == BuildingTier.Tier3)
			return isTier3LandCleared;

		return false;
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
	/// Plays a player's town meeting card
	/// </summary>
	/// <param name="cardName">Whether the card is a defense or legal action card</param>
	public void PlayTownHallCard(string cardName)
	{
		if(cardName.ToLower() == "defense")
		{
			if(defenseCardCount > 0)
			{
				legalActionCardCount--;
				TownMeetingManager.instance.AddDefense();
			}
			else
			{
				Debug.Log("Not enough defense cards");
			}
		}
		else if(cardName.ToLower() == "legal action")
		{
			if(legalActionCardCount > 0)
			{
				legalActionCardCount--;
				TownMeetingManager.instance.AddLegalAction();
			}
			else
			{
				Debug.Log("Not enough legal action cards");
			}
		}		
	}
	#endregion

}
