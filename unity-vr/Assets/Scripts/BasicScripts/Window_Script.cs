using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR;

public class Window_Script : MonoBehaviour
{
    private Animator window_Animator;
    private bool is_Open = false;
    private bool CanOpen = true;

    private float timer;
    private float time = 1.3f;
    
    private void Start()
    {
        window_Animator = GetComponentInParent<Animator>();
    }
    
    public void OnSelectEntered()
    {
        if (!is_Open && CanOpen)
        {
            // Clinical log: Opening window is an act of visual exposure
            Debug.Log("[User Action] Patient OPENED the window blind (VR - Visual Exposure).");
            
            window_Animator.Play("CurtainOpen");
            is_Open = true;

            CanOpen = false; // Delay For Opening And Closing
            timer = time;

            SoundManagerScript.instance.PlayCurtainSound();
        }
        else if(is_Open && CanOpen)
        {
            // Clinical log: Closing window is an act of avoidance/coping to reduce anxiety
            Debug.Log("[User Action] Patient CLOSED the window blind (VR - Avoidance).");
            
            window_Animator.Play("CurtainClosed");
            is_Open = false;

            CanOpen = false;
            timer = time;
            SoundManagerScript.instance.PlayCurtainSound();
        }
    }

    private void OnMouseDown()
    {
        if (!is_Open && CanOpen)
        {
            // Clinical log: Opening window is an act of visual exposure
            Debug.Log("[User Action] Patient OPENED the window blind (Non-VR - Visual Exposure).");
            
            window_Animator.Play("CurtainOpen");
            is_Open = true;

            CanOpen = false; // Delay For Opening And Closing
            timer = time;

            SoundManagerScript.instance.PlayCurtainSound();
        }
        else if(is_Open && CanOpen)
        {
            // Clinical log: Closing window is an act of avoidance/coping to reduce anxiety
            Debug.Log("[User Action] Patient CLOSED the window blind (Non-VR - Avoidance).");
            
            window_Animator.Play("CurtainClosed");
            is_Open = false;

            CanOpen = false;
            timer = time;
            SoundManagerScript.instance.PlayCurtainSound();
        }
    }

    public void Update()
    {
        if(timer > 0f)
        {
            timer -= Time.deltaTime;
            if (timer < 0f)
            {
                CanOpen = true;
            }
        }
    }
}