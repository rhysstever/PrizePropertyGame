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
    private Building currentSelectedBuilding;

    public List<Building> Buildings { get { return buildings; } }
    public Building CurrentSelectedBuilding { get { return currentSelectedBuilding; } }

    // Start is called before the first frame update
    void Start()
    {
        CreateBuildingDictionary();
        SelectBuilding(null);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Creates a dictionary with all the building types
    /// </summary>
    private void CreateBuildingDictionary()
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

    /// <summary>
    /// Select a new building
    /// </summary>
    /// <param name="building">The building being selected</param>
    public void SelectBuilding(Building building)
	{
        currentSelectedBuilding = building;

        // Update UI
        UIManager.instance.UpdateSelectedBuildingUI(currentSelectedBuilding);
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
    /// Gets a building by tier and place indecies
    /// </summary>
    /// <param name="tier">The tier of the building (1-3)</param>
    /// <param name="index">The place index of the building within the tier (1-3)</param>
    /// <returns>The building at that index</returns>
    public Building GetBuildingByIndex(int tier, int index)
	{
        int totalIndex = (tier - 1) * 3 + (index - 1);
        return buildings[totalIndex];
	}

    /// <summary>
    /// Builds the currently selected building for the current player
    /// </summary>
    public void CurrentPlayerBuildCurrentBuilding()
	{
        GameManager.instance.CurrentPlayer.Build(currentSelectedBuilding);
	}
}
