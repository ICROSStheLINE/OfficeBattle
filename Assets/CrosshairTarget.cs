using UnityEngine;

public class CrosshairTarget : MonoBehaviour
{
	Animator anim;
	LayerMask layerMask;
    [SerializeField] RectTransform crosshairRectTransform;
    float maxDistance = 100f;
    float smoothSpeed = 30f;
	float dogshitAimRadius = 0.5f;

    private Vector3 screenCenter;

    void Start() 
	{
		anim = GetComponent<Animator>();
		layerMask = LayerMask.GetMask("Object");
        screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
    }

    void Update() 
	{
		anim.SetBool("HoveringItem", false);
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        Vector3 targetPos = screenCenter;

        if (Physics.SphereCast(ray, dogshitAimRadius, out hit, maxDistance, layerMask)) {
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
