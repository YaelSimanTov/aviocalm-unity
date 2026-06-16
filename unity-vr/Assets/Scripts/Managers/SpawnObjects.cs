using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SpawnObjects : MonoBehaviour
{
    [Header("Npcs")]
    [SerializeField] GameObject flight_attendent_npc;

    [Header("Player")]
    [SerializeField] GameObject Player_Parent;

    [Header("Player Vr")]
    [SerializeField] GameObject Player_Vr;
    [SerializeField] Transform spawnPoint;

    [Header("Game Objects")]
    [SerializeField] GameObject plane_Object;

    [Header("In Script Variables")]
    public static SpawnObjects instance;

    public void Awake()
    {
        instance = this;
    }

    #region States
    public void BoardingPlaneSpawning(Transform flight_attendent_Spawn_Pos,Transform player_Spawn_pos)
    {
        Spawn_Player(player_Spawn_pos,flight_attendent_Spawn_Pos);
        Spawn_Player_Vr();
        Spawn_Plane();
        Spawn_Flight_attendent(flight_attendent_Spawn_Pos);
    }
    #endregion

    #region Npc Spawning
    public void Spawn_Flight_attendent(Transform pos)
    {
        if (flight_attendent_npc == null)
            return;

       GameObject newNpc = Instantiate(flight_attendent_npc, pos.position ,Quaternion.identity);

       GameObject AirPlane = GameObject.Find("AirPlaneParent(Clone)");
        if (AirPlane == null)
        {
            // Tagged as [Dev] because this is a developer warning
            Debug.LogWarning("[Dev] Couldn't Find AirPlane Folder");
        }
        else
        {
            newNpc.transform.SetParent(AirPlane.transform);
        }
    }
    #endregion

    #region SpawnPlane
    public void Spawn_Plane()
    {
        if(plane_Object != null)
        {
            GameObject newObject = Instantiate(plane_Object, Vector3.zero, Quaternion.identity);

            GameObject ObjectsFolder = GameObject.Find("BackGround");
            if (ObjectsFolder == null)
            {
                // Tagged as [Dev] because this is a developer warning
                Debug.LogWarning("[Dev] Couldn't Find Objects Folder");
            }
            else
            {
                // BUG FIX: Correctly parenting the new plane object to the background folder
                newObject.transform.SetParent(ObjectsFolder.transform);
            }

            // Tagged as [Dev] because the clinical timeline doesn't need to know when prefabs spawn
            Debug.Log("[Dev] Spawned Plane");
        }
    }
    #endregion

    #region SpawnPlayer
    public void Spawn_Player(Transform pos,Transform npcPos)
    {
       if(Player_Parent != null)
        {
            GameObject new_Player = Instantiate(Player_Parent, pos.position, Quaternion.identity);

            PlayerCamPlay playerCameraPlay = new_Player.GetComponent<PlayerCamPlay>();

            if (playerCameraPlay != null)
            {
                playerCameraPlay.setNpc_Pos(npcPos);
            }
        }
    }

    public void Spawn_Player_Vr()
    {
        if (Player_Vr != null && spawnPoint != null)
        {
            GameObject spawnedPlayer = Instantiate(Player_Vr, spawnPoint.position, spawnPoint.rotation);
            XROrigin xrOrigin = spawnedPlayer.GetComponent<XROrigin>();
            if (xrOrigin != null)
            {
                Vector3 cameraOffset = xrOrigin.CameraInOriginSpacePos;
                xrOrigin.transform.position = spawnPoint.position - cameraOffset;
            }
        }
    }
    #endregion
}