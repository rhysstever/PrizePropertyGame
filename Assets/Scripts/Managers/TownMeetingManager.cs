using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownMeetingManager : MonoBehaviour
{
    #region Singleton Code
    // A public reference to this script
    public static TownMeetingManager instance = null;

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

    private int legalActionCount;
    private int defenseCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ClearTownMeetingCards()
	{
        legalActionCount = 0;
        defenseCount = 0;
	}

    public void AddLegalAction()
    {
        legalActionCount++;
    }

    public void AddDefense()
	{
        defenseCount++;
	}

    public string GetTownMeetingDecision()
	{
        string[] decisions = new string[legalActionCount + defenseCount + 1];

        for(int i = 0; i < legalActionCount; i++)
            decisions[i] = "Legal Action";

        for(int i = legalActionCount; i < decisions.Length; i++)
            decisions[i] = "Defense";

        return decisions[Random.Range(0, decisions.Length)];
    }
}
