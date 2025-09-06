using UnityEngine;

public class CrosshairTarget : MonoBehaviour
{
	Animator anim;
	LayerMask itemPickupLayerMask;
	LayerMask humanTouchLayerMask;
    [SerializeField] RectTransform crosshairRectTransform;
    float pickUpDistance = 5f;
	float humanInteractDistance = 12f;
    float smoothSpeed = 30f;
	float dogshitAimRadius = 0.5f;

    private Vector3 screenCenter;

    void Start() 
	{
		anim = GetComponent<Animator>();
		itemPickupLayerMask = LayerMask.GetMask("Object");
		humanTouchLayerMask = LayerMask.GetMask("HumanTrigger");
        screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
    }

    void Update() 
	{
		screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
		anim.SetBool("HoveringItem", false);
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        Vector3 targetPos = screenCenter;

		// Raycast for picking up objects
        if (Physics.SphereCast(ray, dogshitAimRadius, out hit, pickUpDistance, itemPickupLayerMask)) {
            targetPos = Camera.main.WorldToScreenPoint(hit.collider.transform.position);
			anim.SetBool("HoveringItem", true);
        }
		
		// Raycast for interacting with humans
		if (Physics.SphereCast(ray, dogshitAimRadius, out hit, humanInteractDistance, humanTouchLayerMask, QueryTriggerInteraction.Collide))
		{
			targetPos = Camera.main.WorldToScreenPoint(hit.collider.transform.position);
			anim.SetBool("HoveringItem", true);
		}

        crosshairRectTransform.position = Vector3.Lerp(
            crosshairRectTransform.position,
            targetPos,
            Time.deltaTime * smoothSpeed
        );
    }
}
