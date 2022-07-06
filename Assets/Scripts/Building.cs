using System.Collections;
using System.Collections.Generic;

public enum BuildingTier
{
	Tier1,
	Tier2,
	Tier3
}

public class Building
{
	private string buildingName;
	private BuildingTier buildingTier;
	private string fullName;
	private int cost;
	private bool isBought;

	#region Properties
	public string BuildingName { get { return buildingName; } }
	public BuildingTier BuildingType { get { return buildingTier; } }
	public string FullName { get { return fullName; } }
	public int Cost { get { return cost; } }
	public bool IsBought { get { return isBought; } }
	#endregion

	public Building(string name, BuildingTier tier, string fullName)
	{
		buildingName = name;
		buildingTier = tier;
		this.fullName = fullName;
		isBought = false;

		// Determine cost based on the type
		switch(buildingTier)
		{
			case BuildingTier.Tier1:
				cost = 5;
				break;
			case BuildingTier.Tier2:
				cost = 10;
				break;
			case BuildingTier.Tier3:
				cost = 15;
				break;
			default:
				cost = 0;
				break;
		}
	}

	#region Methods
	/// <summary>
	/// Clones this building
	/// </summary>
	/// <returns>A clone of this Building object</returns>
	public Building Clone()
	{
		return (Building)MemberwiseClone();
	}

	public void Buy() { isBought = true; }

	public void Lose()
	{
		isBought = false;
	}
	#endregion
}
