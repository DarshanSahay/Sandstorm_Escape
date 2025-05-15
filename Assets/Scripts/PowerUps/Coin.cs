using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : InteractableObject
{
    public ObjectType type;


    public override void Activate(GameObject player)
    {
        EventManager.Instance.TriggerCoinCollected(1);
        EventManager.Instance.TriggerOnObjectReturnToPool(this.gameObject);
    }
}
