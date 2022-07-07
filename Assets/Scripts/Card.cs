using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Card
{
    private string description;
    private UnityAction action;

    public string Description { get { return description; } }
    public UnityAction Effect { get { return action; } }

    public Card(string text, UnityAction action)
	{
        this.description = text;
        this.action = action;
    }

    public Card(UnityAction action)
	{
        description = "";
        this.action = action;
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
