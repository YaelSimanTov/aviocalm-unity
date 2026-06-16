using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Player_SeatManager : MonoBehaviour
{
    [Header("Seat Booleans")]
    public bool Is_Sitted = false;
    public bool Is_Seat_Belt_On = false;

    [Header("Instance")]
    public static Player_SeatManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void OnSeatBeltOn()
    {
        if (Is_Seat_Belt_On)
        {
            FlightState newFlightState = FlightState.TakeOffState;

            GameManagerScript.instance.ChangeCurrentState(newFlightState);

            // Tagged as [Dev] because GameManagerScript already logs the phase change clinically using [Flight Phase]
            Debug.Log("[Dev] Changed Current State to TakeOffState");
        }
    }
}