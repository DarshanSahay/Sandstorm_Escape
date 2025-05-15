using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public float worldScrollSpeed = 5f;

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Activate(other.gameObject); // <- this is where the magic happens
        }
    }

    public virtual void Start()
    {
        EventManager.Instance.OnWorldSpeedUpdate += UpdateWorldScrollSpeed;
    }

    public virtual void Update()
    {
        transform.Translate(Vector3.left * worldScrollSpeed);
    }

    public abstract void Activate(GameObject player);

    public virtual void UpdateWorldScrollSpeed(float speed)
    {
        worldScrollSpeed = speed;
    }
}
