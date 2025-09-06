using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Punching : MonoBehaviour
{
    PlayerStats playerStats;
    Animator anim;
	PlayerMovement playerMovement;
    [SerializeField] GameObject leftArm;
    [SerializeField] GameObject rightArm;

    LayerMask humanTouchLayerMask;
    bool handDamageActive = false;
    bool isMidRunningPunch = false;
    float dogshitAimRadius = 0.5f;
	float humanInteractDistance = 12f;
	Vector3 constantPlayerMovement = default(Vector3);
	float constantPlayerSpeed = default(float);

    static readonly float runPunchAnimationDurationSpeedMultiplier = 1f;
    static readonly float runPunchAnimationDuration = 1.083f / runPunchAnimationDurationSpeedMultiplier;
    static readonly float runPunchAnimationFrames = 26f;
    static readonly float punchDamageActivationFrame = 18f;
    static readonly float punchDamageDeactivationFrame = 20f;
    static readonly float secondsUntilDamageActivation = (punchDamageActivationFrame / runPunchAnimationFrames) * runPunchAnimationDuration;
    static readonly float secondsUntilDamageDeactivation = (punchDamageDeactivationFrame / runPunchAnimationFrames) * runPunchAnimationDuration;
    static readonly float secondsBetweenDamageActivationAndDeactivation = secondsUntilDamageDeactivation - secondsUntilDamageActivation;
    static readonly float secondsBetweenDamageDeactivationAndEnd = runPunchAnimationDuration - secondsUntilDamageDeactivation;

    void Start()
    {
        anim = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStats>();
		playerMovement = GetComponent<PlayerMovement>();
        humanTouchLayerMask = LayerMask.GetMask("HumanTrigger");
    }

    void Update()
    {
		CheckForPunchInput();
		if (constantPlayerMovement != default(Vector3))
		{
			playerMovement.BasicMovement(constantPlayerMovement, constantPlayerSpeed);
		}
    }

	void CheckForPunchInput()
	{
        if (Input.GetKey("e") && !isMidRunningPunch && playerStats.isRunning)
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
		    RaycastHit hit;
            // Raycast for interacting with humans
            if (Physics.SphereCast(ray, dogshitAimRadius, out hit, humanInteractDistance, humanTouchLayerMask, QueryTriggerInteraction.Collide))
            {
                StartCoroutine("RunningPunch");
            }
        }
	}

    IEnumerator RunningPunch()
    {
        isMidRunningPunch = true;
        anim.SetBool("isPunching", true);
        playerStats.canMove = false;
		
		constantPlayerMovement = transform.forward;
		constantPlayerSpeed = default(float);

        yield return new WaitForSeconds(secondsUntilDamageActivation);
        handDamageActive = true;
        yield return new WaitForSeconds(secondsBetweenDamageActivationAndDeactivation);
		constantPlayerSpeed = playerMovement.forwardMoveSpeed / 4f;
        handDamageActive = false;
        yield return new WaitForSeconds(secondsBetweenDamageDeactivationAndEnd);

		constantPlayerMovement = default(Vector3);
		constantPlayerSpeed = default(float);
		
        anim.SetBool("isPunching", false);
        isMidRunningPunch = false;
        playerStats.canMove = true;
    }

    public void DetectedCollision(GameObject dataOwner, GameObject collidedObject)
    {
        if (dataOwner == rightArm && handDamageActive)
        {
            // Deal damage to collidedObject's PlayerStats.currentHealth variable
        }
    }
}
