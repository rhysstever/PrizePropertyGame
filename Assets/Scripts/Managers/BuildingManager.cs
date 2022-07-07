using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    #region Singleton Code
    // A public reference to this script
    public static BuildingManager instance = null;

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

    private List<Building> buildings;

    public List<Building> Buildings { get { return buildings; } }

    // Start is called before the first frame update
    void Start()
    {
        buildings = new List<Building>();

        buildings.Add(new Building("CampOffice", BuildingTier.Tier1, "Camp Office"));
        buildings.Add(new Building("Marina", BuildingTier.Tier1, "Marina"));
        buildings.Add(new Building("TennisSwimClub", BuildingTier.Tier1, "Tennis and Swim Club"));
        buildings.Add(new Building("DudeRanch", BuildingTier.Tier2, "Dude Ranch"));
        buildings.Add(new Building("GolfClub", BuildingTier.Tier2, "Golf Club"));
        buildings.Add(new Building("HealthSpa", BuildingTier.Tier2, "Health Spa"));
        buildings.Add(new Building("Casino", BuildingTier.Tier3, "Casino and Night Club"));
        buildings.Add(new Building("Hotel", BuildingTier.Tier3, "Resort Hotel"));
        buildings.Add(new Building("SkiLodge", BuildingTier.Tier3, "Ski Condominium and Lodge"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Gets a building by name
    /// </summary>
    /// <param name="name">The name of the building</param>
    /// <returns>A building (not owned by a player)</returns>
    public Building GetBuildingByName(string name)
	{
        foreach(Building building in buildings)
            if(building.BuildingName.ToLower() == name.ToLower() 
                || building.FullName.ToLower() == name.ToLower())
                return building;

        return null;
	}

    /// <summary>
    /// Checks if a player can buy/build a building
    /// </summary>
    /// <param name="player">The player trying to build</param>
    /// <param name="building">The building that is being built</param>
    /// <returns>Whether the player can buy and build the building</returns>
    public bool CanBuild(Player player, Building building)
	{
        // Check if the land has been cleared
        if(!player.IsLandCleared(building.BuildingTier))
            return false;

        // Check if the player has not already built the building
        if(player.IsBuilt(building))
            return false;

        // Check if the player has enough money
        return player.CurrentMoney >= building.Cost;
	}
}
