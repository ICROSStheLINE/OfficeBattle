using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    void OnTriggerStay(Collider collision)
    {
        // If the trigger collided with an object layer that is our opponent
        if (collision.gameObject.layer == LayerMask.NameToLayer("DummyTrigger"))
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
        }
    }
}
