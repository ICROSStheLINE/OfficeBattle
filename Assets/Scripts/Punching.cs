using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Punching : MonoBehaviour
{
    // -- Important References
    PlayerStats playerStats;
    Animator anim;
	PlayerMovement playerMovement;
    [SerializeField] GameObject leftArm;
    [SerializeField] GameObject rightArm;

    // -- General Use Variables
    LayerMask humanTouchLayerMask;
    bool handDamageActive = false;
    float dogshitAimRadius = 0.5f;
	float humanInteractDistance = 12f;
	Vector3 constantPlayerMovement = default(Vector3);
	float constantPlayerSpeed = default(float);
    Vector3 playerMovementTarget = default(Vector3);
    
	// -- 2 Step Running Punch
    bool isMidRunningPunch = false;
	float punchGap = 2;
	Vector2 twoStepRunningPunchTriggerRange = new Vector2(8f, 12f);

    // -- 2 Step Running Punch Animation Variables
    static readonly float runPunchAnimationDurationSpeedMultiplier = 1f;
    static readonly float runPunchAnimationDuration = 1.083f / runPunchAnimationDurationSpeedMultiplier;
    static readonly float runPunchAnimationFrames = 26f;
    static readonly float runPunchDamageActivationFrame = 18f;
    static readonly float runPunchDamageDeactivationFrame = 20f;
    static readonly float secondsUntilRunPunchDamageActivation = (runPunchDamageActivationFrame / runPunchAnimationFrames) * runPunchAnimationDuration;
    static readonly float secondsUntilRunPunchDamageDeactivation = (runPunchDamageDeactivationFrame / runPunchAnimationFrames) * runPunchAnimationDuration;
    static readonly float secondsBetweenRunPunchDamageActivationAndDeactivation = secondsUntilRunPunchDamageDeactivation - secondsUntilRunPunchDamageActivation;
    static readonly float secondsBetweenRunPunchDamageDeactivationAndEnd = runPunchAnimationDuration - secondsUntilRunPunchDamageDeactivation;

    void Start()
    {
        anim = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStats>();
		playerMovement = GetComponent<PlayerMovement>();
        humanTouchLayerMask = LayerMask.GetMask("DummyTrigger");
    }

    void Update()
    {
        CheckForPunchInput();
        // If the player needs to move for the attack, trigger the BasicMovement() method in PlayerMovement
        if (constantPlayerMovement != default(Vector3)) { playerMovement.BasicMovement(constantPlayerMovement, constantPlayerSpeed); }
        // If the player needs to move somewhere specific for the attack, trigger BasicMovementTowards()
        if (playerMovementTarget != default(Vector3)) { playerMovement.BasicMovementTowards(playerMovementTarget, constantPlayerSpeed); }
    }

	void CheckForPunchInput()
	{
        if (Input.GetKey(KeyCode.Mouse0) && !isMidRunningPunch)
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
		    RaycastHit hit;
            // Raycast for interacting with humans
            if (Physics.SphereCast(ray, dogshitAimRadius, out hit, humanInteractDistance, humanTouchLayerMask, QueryTriggerInteraction.Collide))
            {
				if (playerStats.isRunning && hit.distance > twoStepRunningPunchTriggerRange.x && hit.distance < twoStepRunningPunchTriggerRange.y)
				{
					StartCoroutine(RunningPunch(hit.transform.gameObject));
				}
            }
        }
	}

    IEnumerator RunningPunch(GameObject punchTarget)
    {
        // -- Player cannot move or turn the camera while punching
        isMidRunningPunch = true;
        anim.SetBool("isPunching", true);
        playerStats.canMove = false;
		playerStats.canTurn = false;

        // -- Player will not blindly move forward for this attack.
        // -- Instead, player will linearly run towards where the opponent was when they were clicked on
        // -- And the player will run towards that point until they reach 3 units' worth of distance from that position
        // -- (At default speed)
        playerMovementTarget = punchTarget.transform.position + (playerMovement.NormalizedDirectionTowardsTarget(punchTarget.transform.position,transform.position) * punchGap);
        constantPlayerMovement = default(Vector3);
        constantPlayerSpeed = default(float);

        yield return new WaitForSeconds(secondsUntilRunPunchDamageActivation);
        handDamageActive = true; // -- Turn on the damage hitboxes on the punch
        yield return new WaitForSeconds(secondsBetweenRunPunchDamageActivationAndDeactivation);

        // -- Player will no longer run towards that point where the opponent was.
        // -- Instead, player will blindly move forward, but at a snails pace
        // -- This is because the animation has a little post-punch animation depicting the player having a bit of momentum
        playerMovementTarget = default(Vector3); 
        constantPlayerMovement = transform.forward;
        constantPlayerSpeed = playerMovement.forwardMoveSpeed / 4f;

        handDamageActive = false; // -- Turn off the damage hitboxes on the punch
        yield return new WaitForSeconds(secondsBetweenRunPunchDamageDeactivationAndEnd);

        // -- No more custom movement
        playerMovementTarget = default(Vector3);
        constantPlayerMovement = default(Vector3);
		constantPlayerSpeed = default(float);

        // -- Player is re-granted ability to move and turn the camera
        anim.SetBool("isPunching", false);
        isMidRunningPunch = false;
        playerStats.canMove = true;
		playerStats.canTurn = true;
    }

    public void DetectedCollision(GameObject dataOwner, GameObject collidedObject)
    {
        if (dataOwner == rightArm && handDamageActive && !IsArm(collidedObject))
        {
            // Deal damage to collidedObject's PlayerStats.currentHealth variable
            collidedObject.transform.root.GetComponent<PlayerStats>().TakeDamage();
			handDamageActive = false;
        }
		if (dataOwner == rightArm && handDamageActive && IsArm(collidedObject))
		{
			Blocking collidedObjBlockingComponent = collidedObject.transform.root.GetComponent<Blocking>();
			if (collidedObjBlockingComponent.isBlocking)
			{
				handDamageActive = false;
				collidedObjBlockingComponent.StartCoroutine("BlockPushback");
			}
		}
    }
	
	bool IsArm(GameObject limbInQuestion)
	{
		return (limbInQuestion.name == "LowerArm_L" || limbInQuestion.name == "LowerArm_R");
	}
}
