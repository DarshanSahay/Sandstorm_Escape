using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : InteractableObject
{
    public ObjectType type;
    public PowerUpData data;

    public override void Activate(GameObject player)
    {
        EventManager.Instance.TriggerPowerupPicked(data);
        EventManager.Instance.TriggerOnObjectReturnToPool(this.gameObject);
    }
}
