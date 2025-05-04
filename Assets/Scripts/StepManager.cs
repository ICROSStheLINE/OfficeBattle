using UnityEngine;

public class StepManager : MonoBehaviour
{
    GameObject steppingTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Before running, have the steppingposition/rayposition gameObject target
        // a position AHEAD of standing position, but not far enough ahead to exceed
        // the stepDistanceThreshhold defined in IKWalkingAnim.

        // On runtime, make the player stand on the StandingPosition gameobject,
        // then have the player feet teleport to their future positions as already
        // coded. 

        // COMPLETED <--

        // ==================================

        // Once that has been developed properly move on to stage two: Making the feet
        // return to their standing position when not moving.

        // This will be difficult to do at this time because testing the stepping code
        // has only been done through manually moving the player on runtime through
        // the scene view.

        // To go around this hurdle for testing, you can make a serialized boolean
        // called "isStanding" that you can set to true whenever you want to simulate
        // what would happen if the player was not trying to move.

        // COMPLETED <--
        
        // ==================================
        
        // Once that is complete, begin stage 3: Making the stepping motion
        // multi-directional.

        // This would probably require a rewrite of how the raycasting is called.
        // Instead of casting rays at "rayPosition" that go straight down, maybe cast
        // some rays that start at the player's waist that don't point straight down,
        // but instead point DIAGONALLY down. Ideally, have it point in around the
        // same positions as where the rayPosition gameobject is currently set to.

        // Maybe have some kind of signifier showing where the raycasts are pointing to
        // in the form of a test gameobject that constantly moves to the position of
        // where the raycasts are hitting, and make the angle of the raycasts a 
        // serialized variable that you can move around on runtime.

        // When that's done, you can freely rotate the raycast direction to match the
        // direction of the player's movement.

        // NOTE: Maybe make the position that the raycast hit call another raycast
        // straight down, just in case the raycast hits a WALL instead of a FLOOR.
        // Further testing required.

        // Alternatively if you want to be cringe, you DON'T need to rewrite the
        // raycasting system. You can just choose to have an empty gameobject
        // that acts as the rayPosition's parent object and you can set its
        // position to be the player's centre position (so that would be directly
        // between the player's feet I guess) and just rotate that around to match
        // the direction of the player's movement.

        // Pros to rewriting raycasting system:
        // - If an object (like a wall) is super close to the player their feet
        // they can position their feet to not go through the wall.
        // - Technical details are subconsciously noticeable in-game by the player.

        // Cons to rewriting raycasting system:
        // - Takes a lot of time and headache
        // - Might make you slightly suicidal
        
        // ==================================

        // Have the stepping animation triggered by in-game player-inputted movement
        // as opposed to a serialized variable that dictates when the player is walking.

        // COMPLETED <--
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
