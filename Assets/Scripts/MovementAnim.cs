using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnim : MonoBehaviour
{
    PlayerStats playerStats;
    PlayerMovement playerMovement;
    Animator anim;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerStats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        anim.SetBool("isRunning", playerStats.isRunning);
        anim.SetFloat("runDirection", Vector3.Dot(transform.forward, playerMovement.moveDirection));
    }
}
