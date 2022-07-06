using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Card
{
    private string description;
    private UnityEvent effect;

    public string Description { get { return description; } }
    public UnityEvent Effect { get { return effect; } }

    public Card(string text, UnityEvent effect)
	{
        this.description = text;
        this.effect = effect;
    }

    public Card(UnityEvent effect)
	{
        description = "";
        this.effect = effect;
	}

    /// <summary>
    /// Clones this card
    /// </summary>
    /// <returns>A clone of this Card object</returns>
    public Card Clone()
	{
        return (Card)MemberwiseClone();
	}
}
