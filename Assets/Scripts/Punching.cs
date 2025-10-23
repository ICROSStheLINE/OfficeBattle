using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class Punching : NetworkBehaviour
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
    bool isMidRunningPunch = false;
    float punchGap = 2;

    // -- 1 Step Running Punch
    Vector2 oneStepRunningPunchTriggerRange = new Vector2(4f, 7.99f);
	
	// -- 1 Step Running Punch
	static readonly float oneStepRunPunchAnimationDurationSpeedMultiplier = 1f;
	static readonly float oneStepRunPunchAnimationDuration = 0.583f / oneStepRunPunchAnimationDurationSpeedMultiplier;
	static readonly float oneStepRunPunchAnimationFrames = 14f;
	static readonly float oneStepRunPunchDamageActivationFrame = 8f;
	static readonly float oneStepRunPunchDamageDeactivationFrame = 12f;
	static readonly float secondsUntilOneStepRunPunchDamageActivation = (oneStepRunPunchDamageActivationFrame / oneStepRunPunchAnimationFrames) * oneStepRunPunchAnimationDuration;
	static readonly float secondsUntilOneStepRunPunchDamageDeactivation = (oneStepRunPunchDamageDeactivationFrame / oneStepRunPunchAnimationFrames) * oneStepRunPunchAnimationDuration;
	static readonly float secondsBetweenOneStepRunPunchDamageActivationAndDeactivation = secondsUntilOneStepRunPunchDamageDeactivation - secondsUntilOneStepRunPunchDamageActivation;
    static readonly float secondsBetweenOneStepRunPunchDamageDeactivationAndEnd = oneStepRunPunchAnimationDuration - secondsUntilOneStepRunPunchDamageDeactivation;

    // -- 2 Step Running Punch
    Vector2 twoStepRunningPunchTriggerRange = new Vector2(8f, 12f);

    // -- 2 Step Running Punch Animation Variables
    static readonly float twoStepRunPunchAnimationDurationSpeedMultiplier = 1f;
    static readonly float twoStepRunPunchAnimationDuration = 1.083f / twoStepRunPunchAnimationDurationSpeedMultiplier;
    static readonly float twoStepRunPunchAnimationFrames = 26f;
    static readonly float twoStepRunPunchDamageActivationFrame = 18f;
    static readonly float twoStepRunPunchDamageDeactivationFrame = 20f;
    static readonly float secondsUntiltwoStepRunPunchDamageActivation = (twoStepRunPunchDamageActivationFrame / twoStepRunPunchAnimationFrames) * twoStepRunPunchAnimationDuration;
    static readonly float secondsUntiltwoStepRunPunchDamageDeactivation = (twoStepRunPunchDamageDeactivationFrame / twoStepRunPunchAnimationFrames) * twoStepRunPunchAnimationDuration;
    static readonly float secondsBetweentwoStepRunPunchDamageActivationAndDeactivation = secondsUntiltwoStepRunPunchDamageDeactivation - secondsUntiltwoStepRunPunchDamageActivation;
    static readonly float secondsBetweentwoStepRunPunchDamageDeactivationAndEnd = twoStepRunPunchAnimationDuration - secondsUntiltwoStepRunPunchDamageDeactivation;

	// -- Parried Punch Animation Variables
	static readonly float parriedPunchAnimationDurationSpeedMultiplier = 1f;
	static readonly float parriedPunchAnimationDuration = 0.167f / parriedPunchAnimationDurationSpeedMultiplier;

    void Start()
    {
        anim = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStats>();
        playerMovement = GetComponent<PlayerMovement>();
		if (humanTouchLayerMask == default(LayerMask))
		{
			humanTouchLayerMask = LayerMask.GetMask("DummyTrigger");
		}
    }

    void Update()
    {
		if (!IsOwner)
        {
            return;
        }
		
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
                    StartCoroutine(Punch("twoStepPunching", hit.transform.gameObject, secondsUntiltwoStepRunPunchDamageActivation, secondsBetweentwoStepRunPunchDamageActivationAndDeactivation, secondsBetweentwoStepRunPunchDamageDeactivationAndEnd));
                }
                if (playerStats.isRunning && hit.distance > oneStepRunningPunchTriggerRange.x && hit.distance < oneStepRunningPunchTriggerRange.y)
                {
                    StartCoroutine(Punch("oneStepPunching", hit.transform.gameObject, secondsUntilOneStepRunPunchDamageActivation, secondsBetweenOneStepRunPunchDamageActivationAndDeactivation, secondsBetweenOneStepRunPunchDamageDeactivationAndEnd));
                }
            }
        }
    }

    IEnumerator Punch(string punchType, GameObject punchTarget, float secondsUntilPunchDamageActivation, float secondsBetweenPunchDamageActivationAndDeactivation, float secondsBetweenPunchDamageDeactivationAndEnd)
    {
        // -- Player cannot move or turn the camera while punching
        isMidRunningPunch = true;
        anim.SetBool(punchType, true);
        playerStats.canMove = false;
        playerStats.canTurn = false;
		punchTarget.transform.root.GetComponent<PlayerStats>().SetAsTargetServerRpc(true);

        // -- Player will not blindly move forward for this attack.
        // -- Instead, player will linearly run towards where the opponent was when they were clicked on
        // -- And the player will run towards that point until they reach 'punchGap' worth of distance from that position
        // -- (At default speed)
        playerMovementTarget = punchTarget.transform.position + (playerMovement.NormalizedDirectionTowardsTarget(punchTarget.transform.position, transform.position) * punchGap);
        constantPlayerMovement = default(Vector3);
        constantPlayerSpeed = default(float);
        if (punchType == "oneStepPunching")
        {
            constantPlayerSpeed = 13f;
        }

        yield return new WaitForSeconds(secondsUntilPunchDamageActivation);
        handDamageActive = true; // -- Turn on the damage hitboxes on the punch
        yield return new WaitForSeconds(secondsBetweenPunchDamageActivationAndDeactivation);

        // -- Player will no longer run towards that point where the opponent was.
        // -- Instead, player will blindly move forward, but at a snails pace
        // -- This is because the animation has a little post-punch animation depicting the player having a bit of momentum
        playerMovementTarget = default(Vector3);
        constantPlayerMovement = transform.forward;
        constantPlayerSpeed = playerMovement.forwardMoveSpeed / 4f;

        handDamageActive = false; // -- Turn off the damage hitboxes on the punch
        yield return new WaitForSeconds(secondsBetweenPunchDamageDeactivationAndEnd);

        // -- No more custom movement
        playerMovementTarget = default(Vector3);
        constantPlayerMovement = default(Vector3);
        constantPlayerSpeed = default(float);

        // -- Player is re-granted ability to move and turn the camera
        anim.SetBool(punchType, false);
        isMidRunningPunch = false;
        playerStats.canMove = true;
        playerStats.canTurn = true;
		punchTarget.transform.root.GetComponent<PlayerStats>().SetAsTargetServerRpc(false);
		anim.SetBool("parriedPunch", false);
    }

    public void DetectedCollision(GameObject dataOwner, GameObject collidedObject)
    {
        if (dataOwner == rightArm && handDamageActive)
        {
			Blocking collidedObjBlockingComponent = collidedObject.transform.root.GetComponent<Blocking>();
			// Get directions
            Vector3 defenderForward = collidedObject.transform.root.forward;
            Vector3 toAttacker = (gameObject.transform.position - collidedObject.transform.root.position).normalized;

            // Calculate angle between defender forward and attacker direction
            float angleOffsetFromPlayer = Vector3.Angle(defenderForward, toAttacker); // 0 means target is facing the player dead-on
			
			if (collidedObjBlockingComponent.isParrying.Value && angleOffsetFromPlayer <= 30f)
			{
				handDamageActive = false;
				collidedObjBlockingComponent.TriggerParryServerRpc(new NetworkObjectReference(transform.gameObject.GetComponent<NetworkObject>()));
			}
			else if (collidedObjBlockingComponent.isBlocking.Value && angleOffsetFromPlayer <= 30f)// If opponent was blocking 
			{
				handDamageActive = false;
                collidedObjBlockingComponent.TriggerBlockPushbackServerRpc();
			}
			else // If opponent wasn't blocking
            {
				// Deal damage to collidedObject's PlayerStats.currentHealth variable
				collidedObject.transform.root.GetComponent<PlayerStats>().TakeDamageServerRpc();
				handDamageActive = false;
            }
			collidedObject.transform.root.GetComponent<PlayerStats>().SetAsTargetServerRpc(false);
        }
    }

    bool IsArm(GameObject limbInQuestion)
    {
        return (limbInQuestion.name == "LowerArm_L" || limbInQuestion.name == "LowerArm_R");
    }
    
    public void SetPlayer(int player)
    {
        if (player == 1)
        {
            humanTouchLayerMask = LayerMask.GetMask("DummyTrigger", "Player2Trigger");
        }
        if (player == 2)
        {
            humanTouchLayerMask = LayerMask.GetMask("DummyTrigger", "Player1Trigger");
        }
    }
}
