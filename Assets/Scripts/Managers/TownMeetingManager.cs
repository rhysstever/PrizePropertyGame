using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TownMeetingCardType
{
    LegalAction,
    Defense
}

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

    private List<TownMeetingCardType> townMeetingDeckList;

    private int legalActionCount;
    private int defenseCount;

    // Start is called before the first frame update
    void Start()
    {
        SetupTownMeetingDeck();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetupTownMeetingDeck()
	{
        townMeetingDeckList = new List<TownMeetingCardType>();

        // Add legal action cards to deck
        for(int i = 0; i < 18; i++)
            townMeetingDeckList.Add(TownMeetingCardType.LegalAction);

        // Add defense cards to deck
        for(int i = 0; i < 16; i++)
            townMeetingDeckList.Add(TownMeetingCardType.Defense);
    }

    public TownMeetingCardType BuyTownMeetingCard()
	{
        int randIndex = Random.Range(0, townMeetingDeckList.Count);
        TownMeetingCardType tmCard = townMeetingDeckList[randIndex];
        townMeetingDeckList.RemoveAt(randIndex);
        return tmCard;
	}

    public void ClearTownMeetingCards()
	{
        legalActionCount = 0;
        defenseCount = 0;
	}

    public void AddPlayedCard(TownMeetingCardType cardType)
	{
        if(cardType == TownMeetingCardType.LegalAction)
            legalActionCount++;
        else
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
