using System;
using System.Collections;
using System.Collections.Generic;

public enum PlayerColor
{
	Red, 
	Blue, 
	Orange, 
	Yellow
}

public class Player
{
	#region Fields
	// Player info
	private PlayerColor color;
	private string playerName;

	// Money
	private int currentMoney;
	private int incomeMulitplier;

	// Land & Buildings
	private bool isTier1LandCleared;
	private bool isTier2LandCleared;
	private bool isTier3LandCleared;
	private List<Building> buildings;

	// Town Meeting Cards
	private int legalActionCards;
	private int defenseCards;
	#endregion

	#region Properties
	public string PlayerName { get { return playerName; } }
	public int CurrentMoney { get { return currentMoney; } }
	public int IncomeMulitplier { get { return incomeMulitplier; } }
	#endregion

	#region Constructors
	/// <summary>
	/// Creates a new player
	/// </summary>
	/// <param name="color">The color of the player</param>
	public Player(PlayerColor color)
	{
		this.color = color;
		playerName = color.ToString() + " Player";

		currentMoney = 15;
		incomeMulitplier = 1;

		SetupLand();

		legalActionCards = 0;
		defenseCards = 0;
	}

	/// <summary>
	/// Creates a new player
	/// </summary>
	/// <param name="color">The color of the player</param>
	/// <param name="name">The name of the player</param>
	public Player(PlayerColor color, string name)
	{
		this.color = color;
		playerName = name;

		currentMoney = 15;
		incomeMulitplier = 1;

		SetupLand();

		legalActionCards = 0;
		defenseCards = 0;

	}
	#endregion

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
		{
			GameManager.instance.EndGame(color);
		}
		else
			incomeMulitplier = multiplier;
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
	#endregion

}
