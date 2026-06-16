using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightAttendent_Trigger : MonoBehaviour
{
    [SerializeField] GameObject airPlaneDoors;

    public bool BoardingState;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && GameManagerScript.instance.currentState == FlightState.BoardingState && BoardingState)
        {
            PlayerCamPlay playerCamPlay = other.gameObject.GetComponent<PlayerCamPlay>();
            playerCamPlay.PlayerLookAtNpc(true);

            if (airPlaneDoors != null)
            {
                airPlaneDoors.SetActive(true);
            }

            // Log the exact moment boarding ends and doors close - highly relevant for anxiety tracking
            Debug.Log("[Flight Event] Boarding phase complete. Airplane doors activated.");

            Destroy(this.gameObject);
        }
        else if (other.gameObject.CompareTag("Player") && GameManagerScript.instance.currentState == FlightState.LandedState && !BoardingState)
        {
            PlayerCamPlay.instance.PlayerLookAtNpc(true);
            FlightNpcSpeak.instance.IsSpeaking();

            // Log the post-landing interaction - helps the therapist see when the simulation safely concludes
            Debug.Log("[Flight Event] Post-landing interaction triggered. Flight attendant is speaking.");

            Destroy(this.gameObject);
        }
    }
}