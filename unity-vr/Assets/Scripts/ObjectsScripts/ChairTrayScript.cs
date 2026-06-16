using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairTrayScript : MonoBehaviour
{
    private Animator animator;
    private bool IsOpen;
    
    private void Start()
    {
        IsOpen = true;
        animator = GetComponent<Animator>();
    }

    private void OnMouseDown()
    {
        IsOpen = !IsOpen;

        if (IsOpen)
        {
            // Clinical log: Patient interacting with the environment. Can indicate settling in or fidgeting.
            Debug.Log("[User Action] Patient OPENED the seat tray (Non-VR).");
        }
        else
        {
            // Clinical log: Closing the tray
            Debug.Log("[User Action] Patient CLOSED the seat tray (Non-VR).");
        }

        animator.SetBool("IsOpen", IsOpen);
    }

    public void OnSelectEntered()
    {
        IsOpen = !IsOpen;

        if (IsOpen)
        {
            // Clinical log: Patient interacting with the environment in VR. 
            Debug.Log("[User Action] Patient OPENED the seat tray (VR).");
        }
        else
        {
            // Clinical log: Closing the tray in VR
            Debug.Log("[User Action] Patient CLOSED the seat tray (VR).");
        }

        animator.SetBool("IsOpen", IsOpen);
    }
}