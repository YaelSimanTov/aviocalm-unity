using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class Attendent_Call_Script : MonoBehaviour
{
    [SerializeField] Transform AttendentPos; // For The Attendent To Go To
    [SerializeField] GameObject WaterGlassPrefab;

    public static Attendent_Call_Script instance;

    private void Awake()
    {
        instance = this;
    }
    
    private void OnMouseDown()
    {
        bool ShouldCallAttendent = Player_SeatManager.instance.Is_Seat_Belt_On && this.enabled && GameManagerScript.instance.currentState == FlightState.InFlightState;

        if (ShouldCallAttendent)
        {
            // Clinical log: Seeking reassurance is a key safety behavior in anxiety management
            Debug.Log("[User Action] Patient called the flight attendant (Non-VR - Seeking Reassurance).");
            
            NpcMovement.instance.SetButton(AttendentPos.transform);
            NpcMovement.instance.MovingTowardsPlayer = true;
            NpcMovement.instance.ShouldMove = true;

            SoundManagerScript.instance.PlayAttendentButtonSound(); // Play The Sound
            StartCoroutine(DisableForAMin());
        }
    }

    public void OnSelectEntered()
    {
        bool ShouldCallAttendent = Player_SeatManager.instance.Is_Seat_Belt_On && this.enabled && GameManagerScript.instance.currentState == FlightState.InFlightState;

        if (ShouldCallAttendent)
        {
            // Clinical log: Seeking reassurance in VR
            Debug.Log("[User Action] Patient called the flight attendant (VR - Seeking Reassurance).");
            
            NpcMovement.instance.SetButton(AttendentPos.transform);
            NpcMovement.instance.MovingTowardsPlayer = true;
            NpcMovement.instance.ShouldMove = true;

            SoundManagerScript.instance.PlayAttendentButtonSound();

            StartCoroutine(DisableForAMin());
        }
    }

    private IEnumerator DisableForAMin() // Disabling The Abillty To Call The Flight Attendent Over and Over
    {
        this.enabled = false;
        yield return new WaitForSeconds(420f);
        
        // Tagged as [Dev] because the clinical timeline doesn't need to know when the button cooldown ends
        Debug.Log("[Dev] Flight attendant call button re-enabled.");
        
        this.enabled = true;
    }

    public void GlassOfWaterAnim()
    {
        PlayerCamPlay playerObject = PlayerCamPlay.instance.gameObject.GetComponent<PlayerCamPlay>();

        if(playerObject != null)
        {
            Animator playerAnim = playerObject.GetComponent<Animator>();

            if(playerAnim != null)
            {
                // Clinical log: Drinking water is a physical grounding technique
                // Debug.Log("[User Action] Patient drank a glass of water (Coping Mechanism / Grounding).");
                playerAnim.Play("DrinkWater");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(AttendentPos.position, 0.2F);
        Gizmos.DrawLine(AttendentPos.position, transform.position);
    }
}