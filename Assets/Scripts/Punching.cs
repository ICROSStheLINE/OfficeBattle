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
    Vector3 playerMovementTarget = default(Vector3);
    float gapBetweenPlayerAndTargetWhilePunching = 3;

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
        if (Input.GetKey("e") && !isMidRunningPunch && playerStats.isRunning)
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
		    RaycastHit hit;
            // Raycast for interacting with humans
            if (Physics.SphereCast(ray, dogshitAimRadius, out hit, humanInteractDistance, humanTouchLayerMask, QueryTriggerInteraction.Collide))
            {
                StartCoroutine(RunningPunch(hit.transform.gameObject));
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
        playerMovementTarget = punchTarget.transform.position + (playerMovement.NormalizedDirectionTowardsTarget(punchTarget.transform.position,transform.position) * gapBetweenPlayerAndTargetWhilePunching);
        constantPlayerMovement = default(Vector3);
        constantPlayerSpeed = default(float);

        yield return new WaitForSeconds(secondsUntilDamageActivation);
        handDamageActive = true; // -- Turn on the damage hitboxes on the punch
        yield return new WaitForSeconds(secondsBetweenDamageActivationAndDeactivation);

        // -- Player will no longer run towards that point where the opponent was.
        // -- Instead, player will blindly move forward, but at a snails pace
        // -- This is because the animation has a little post-punch animation depicting the player having a bit of momentum
        playerMovementTarget = default(Vector3); 
        constantPlayerMovement = transform.forward;
        constantPlayerSpeed = playerMovement.forwardMoveSpeed / 4f;

        handDamageActive = false; // -- Turn off the damage hitboxes on the punch
        yield return new WaitForSeconds(secondsBetweenDamageDeactivationAndEnd);

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
        if (dataOwner == rightArm && handDamageActive)
        {
            // Deal damage to collidedObject's PlayerStats.currentHealth variable
        }
    }
}
