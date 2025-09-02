using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnim : MonoBehaviour
{
    PlayerStats playerStats;
    PlayerMovement playerMovement;
    Animator anim;

    static readonly float animationDurationSpeedMultiplier = 1f;
	static readonly float animationDuration = 0.833f / animationDurationSpeedMultiplier;
	static readonly float animationFrames = 20f;
    static readonly float framesForStep = 10f;
    static readonly float durationForStep = animationDuration * (framesForStep / animationFrames);
    public float forwardStepDistance;
    public float backwardStepDistance;
	
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerStats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();

        forwardStepDistance = playerMovement.forwardMoveSpeed * durationForStep;
        backwardStepDistance = playerMovement.backwardMoveSpeed * durationForStep;
    }

	void LateUpdate()
	{
		Vector3 localMove = playerMovement.GetLocalMovementDirectionNormalized();
		
		anim.SetBool("isRunning", playerStats.isRunning);
		
		anim.SetFloat("runDirectionX", localMove.x, 0.1f, Time.deltaTime); // for strafing left and right
		anim.SetFloat("runDirectionY", localMove.z, 0.1f, Time.deltaTime); // for running forward and backwards
    }
}
