using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class PlayerCamPlay : MonoBehaviour
{
    [Header("Player Componenets")]
    private PlayerController playerController;
    private PlayerMovement_Vr playerController_Vr;
    public float playerlookSpeed;

    [Header("Models To Look At")]
    [HideInInspector] public Transform flight_Attendent_pos;

    [Header("OffSets")]
    public Vector3 camerOffset;
    public bool IsLookingAtNpc;

    public static PlayerCamPlay instance;

    #region Unity Methods
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            // Tagged as [Dev] because this is an internal component warning
            Debug.LogWarning("[Dev] PlayerController not found on this GameObject.");
        }

        playerController_Vr = GetComponent<PlayerMovement_Vr>();
        if (playerController_Vr == null)
        {
            // Tagged as [Dev] because this is an internal component warning
            Debug.LogWarning("[Dev] PlayerMovement_Vr not found on this GameObject.");
        }
    }

    private void Awake()
    {
        instance = this;
    }
    
    private void Update()
    {
    }

    public void setNpc_Pos(Transform pos)
    {
        flight_Attendent_pos = pos;
    }

    #endregion

    #region Act Methods
    public void PlayerLookAtNpc(bool ShouldLook)
    {
        if (ShouldLook)
        {
            ShouldLook = false;

            if(playerController != null)
              playerController.LooksAtAttendent = true;

            else if (playerController_Vr != null)
            playerController_Vr.LooksAtAttendent = true;

            IsLookingAtNpc = true;

            // Tagged as [Dev] to avoid timeline clutter; the clinical action (calling the attendant) is logged elsewhere
            Debug.Log("[Dev] Camera is locking onto NPC.");
        }
    }

    public void PlayerLooking(Transform target)
    {
        Vector3 direction = target.position - transform.position;

        direction.y = 0;

        direction = direction.normalized;

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * playerlookSpeed);
    }

    public IEnumerator AirPlane_Turbalance()
    {
        float magnitude = 0f; // How intense the shake should be
        float roughness = 0f;     // How chaotic the shake feels
        float fadeInTime = 0f;    // Time for the shake to fade in
        float fadeOutTime = 0F;   // Time for the shake to fade out

        CameraShaker cameraShaker = GetComponentInChildren<CameraShaker>();

        if (!cameraShaker.enabled)
        {
            cameraShaker.enabled = true;
        }

        switch (GameManagerScript.instance.LevelDiff)
        {
            case LevelDiff.Easy:
                magnitude = 3f; 
                roughness = 2f; 
                fadeInTime = 0.1f;   
                fadeOutTime = 10.0f;  
                break;

            case LevelDiff.Medium:
                magnitude = 5f; 
                roughness = 5f;     
                fadeInTime = 0.1f;    
                fadeOutTime = 14.0f;   
                break;

            case LevelDiff.Hard:
                magnitude = 3f;
                roughness = 15f;     
                fadeInTime = 0.1f;    
                fadeOutTime = 18.0f; 
                break;
        }

        // Tagged as [Dev] because the clinical event (Turbulence initiated) is already logged in GameManagerScript
        Debug.Log($"[Dev] Turbulence magnitude set to: {magnitude}");
        
        CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
        SoundManagerScript.instance.PlayAirPlaneTurblance();

        yield return new WaitForSeconds(fadeOutTime);
        cameraShaker.enabled = false;
    }

    #endregion
    
    #region GetComponenets

    #endregion
}