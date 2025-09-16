using Unity.Netcode;
using UnityEngine;

public class CameraFollow : NetworkBehaviour
{
    private string pathToHeadBone = "Armature.001/Pelvis/Spine/Chest/Neck/Head";
    void Start()
    {
        if (IsOwner)
        {
            GameObject HeadBone = transform.Find(pathToHeadBone).gameObject;
            Camera.main.transform.position = HeadBone.transform.position + HeadBone.transform.forward/2;
            Camera.main.transform.rotation = transform.rotation;
            Camera.main.transform.parent = gameObject.transform;
        }
    }
}
