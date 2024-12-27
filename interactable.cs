using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    //the name of the object to pass to the text and for general usage.
    //Interact function to be overwritten by the actual script for that individual object.
    public abstract void Interact(PlayerInput playerStats);
}