using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : InteractableObject
{
    public ObjectType type;

    public override void Activate(GameObject player)
    {
        EventManager.Instance.TriggerPlayerHitObstacle();
        EventManager.Instance.TriggerOnObjectReturnToPool(this.gameObject);
    }
}
