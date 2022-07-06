using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardManager : MonoBehaviour
{
    #region Singleton Code
    // A public reference to this script
    public static CardManager instance = null;

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

    private List<Card> cards;
    private List<Card> currentCards;

    // Start is called before the first frame update
    void Start()
    {
        cards = new List<Card>();
        currentCards = new List<Card>();

        ShuffleDeck();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
            PickACard();
    }

    private void CreateCards()
	{

	}

    private void ShuffleDeck()
	{
        currentCards.Clear();

        foreach(Card card in cards)
            currentCards.Add(card.Clone());
	}

    private Card PickACard()
	{
        // Get a random card from the deck
        int cardIndex = Random.Range(0, currentCards.Count);
        Card pickedCard = currentCards[cardIndex];
        
        // Remove the card from the deck
        currentCards.RemoveAt(cardIndex);
        
        // Invoke effect event of picked card 
        pickedCard.Effect.Invoke();
        Debug.Log(pickedCard.Description);

        return pickedCard;
	}

    // ===== Card Event Functions =====

    private void AddStaticAmount(int amount)
	{
        GameManager.instance.Players[GameManager.instance.CurrentTurn].CollectIncome(amount);
	}

    private void SubtractStaticAmount(int amount)
	{
        AddStaticAmount(-amount);
	}

    private void AddContingentAmount(int amount, Building contingencyBuilding)
	{
        // Check if the player has built the prerequisite building
        if(GameManager.instance.Players[GameManager.instance.CurrentTurn].IsBuilt(contingencyBuilding))
            AddStaticAmount(amount);
    }

    private void DestroyBuilding(int unusedAmount, Building buildingToDestroy)
	{
        // Check if the player has built the prerequisite building
        if(GameManager.instance.Players[GameManager.instance.CurrentTurn].IsBuilt(buildingToDestroy))
            GameManager.instance.Players[GameManager.instance.CurrentTurn].LoseBuilding(buildingToDestroy);
    }

    // Lose/Double Income

    // Share static amount
}
