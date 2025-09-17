using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    PlayerStats playerStats;
    int[] targetedLayers;
    void Start()
    {
        playerStats = transform.root.transform.GetComponent<PlayerStats>();

        targetedLayers = new int[] { LayerMask.NameToLayer("DummyTrigger") };
        if (playerStats.playerNumber == 1)
            targetedLayers = new int[] { LayerMask.NameToLayer("Player2Trigger"), LayerMask.NameToLayer("DummyTrigger") };
        else if (playerStats.playerNumber == 2)
            targetedLayers = new int[] { LayerMask.NameToLayer("Player1Trigger"), LayerMask.NameToLayer("DummyTrigger") };
    }
    void OnTriggerStay(Collider collision)
    {
        foreach (int layer in targetedLayers)
        {
            // If the trigger collided with an object layer that is our opponent
            if (collision.gameObject.layer == layer)
            {
                // If it collided with the following body parts
                if (collision.gameObject.name == "Pelvis" ||
                 collision.gameObject.name == "Spine" ||
                 collision.gameObject.name == "Chest" ||
                 collision.gameObject.name == "Neck" ||
                 collision.gameObject.name == "Head")
                {
                    // Tell Punching.cs that we have made a collision with an opponent
                    gameObject.transform.root.GetComponent<Punching>().DetectedCollision(transform.gameObject, collision.transform.gameObject);
                }

                // If it collided with the arms
                if (collision.gameObject.name == "LowerArm_L" ||
                 collision.gameObject.name == "LowerArm_R")
                {
                    gameObject.transform.root.GetComponent<Punching>().DetectedCollision(transform.gameObject, collision.transform.gameObject);
                }
            }
        }
    }
}
