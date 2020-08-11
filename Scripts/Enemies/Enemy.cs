using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
	public abstract void GetHit(bool direction, float power);
	public abstract void Die();
	public abstract void SetAggro(bool state);
	public abstract void Respawn();
}
