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

    public Building GetBuilding(string name)
	{
        foreach(Building building in buildings)
            if(building.BuildingName.ToLower() == name.ToLower() 
                || building.FullName.ToLower() == name.ToLower())
                return building;

        return null;
	}

    public bool CanBuy(Player player, Building building)
	{
        // Check if the land has been cleared

        // Check if the player has not already built the building

        // Check if the player has enough money
        
        return true;
	}
}
