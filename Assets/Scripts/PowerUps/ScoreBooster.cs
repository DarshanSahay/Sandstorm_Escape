using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBooster : InteractableObject
{
    public ObjectType type;
    public PowerUpData data;

    public override void Activate(GameObject player)
    {
        EventManager.Instance.TriggerPowerupPicked(data);
        EventManager.Instance.TriggerOnObjectReturnToPool(this.gameObject);
    }
}

[System.Serializable]
public class PowerUpData
{
    public float powerTimer;
    public PowerUpType type;
    public Sprite icon;
}

public enum PowerUpType 
{
    ScoreBooster,
    Shield
}
