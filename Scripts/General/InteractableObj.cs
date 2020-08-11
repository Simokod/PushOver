using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObj : MonoBehaviour {
	public abstract void Interact();
	public abstract void finishInteract();
	public abstract string getType();
}
