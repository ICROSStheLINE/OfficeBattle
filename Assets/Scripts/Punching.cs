using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Punching : MonoBehaviour
{
    Animator anim;
    [SerializeField] GameObject leftArm;
    [SerializeField] GameObject rightArm;

    bool handDamageActive = false;
    bool isMidPunch = false;

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
    }
    void Update()
    {
        if (Input.GetKey("e") && !isMidPunch)
        {
            StartCoroutine("RunningPunch");
        }
    }

    IEnumerator RunningPunch()
    {
        isMidPunch = true;
        anim.SetBool("isPunching", true);

        yield return new WaitForSeconds(secondsUntilDamageActivation);
        handDamageActive = true;
        yield return new WaitForSeconds(secondsBetweenDamageActivationAndDeactivation);
        handDamageActive = false;
        yield return new WaitForSeconds(secondsBetweenDamageDeactivationAndEnd);
        anim.SetBool("isPunching", false);
        isMidPunch = false;
    }

    public void DetectedCollision(GameObject dataOwner, GameObject collidedObject)
    {
        if (dataOwner == rightArm && handDamageActive)
        {
            // Deal damage to collidedObject's PlayerStats.currentHealth variable
        }
    }
}
