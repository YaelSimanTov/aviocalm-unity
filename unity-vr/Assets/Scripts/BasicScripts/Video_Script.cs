using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Video_Script : MonoBehaviour
{
    [Header("Button Essentails")]
    private bool is_Video_Open = false;
    private Animator buttonAnim;

    [SerializeField] private GameObject Video_gm;
  
    private GameObject Video;

    private void Start()
    {
        buttonAnim = GetComponent<Animator>();
        Video_gm.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!is_Video_Open)
        {
            // Log opening video via mouse - clinically shows an attempt to use distraction/coping
            Debug.Log("[User Action] Patient turned ON the in-flight video (Non-VR).");
            
            Video_gm.SetActive(true);
            is_Video_Open = true;    
        }
        else
        {
            // Log closing video via mouse
            Debug.Log("[User Action] Patient turned OFF the in-flight video (Non-VR).");
            
            Video_gm.SetActive(false);
            is_Video_Open = false;
        }
    }

    public void OnSelectEntered()
    {
        if (!is_Video_Open)
        {
            // Log opening video via VR controller - clinically shows an attempt to use distraction/coping
            Debug.Log("[User Action] Patient turned ON the in-flight video (VR).");
            
            Video_gm.SetActive(true);
            is_Video_Open = true;
        }
        else
        {
            // Log closing video via VR controller
            Debug.Log("[User Action] Patient turned OFF the in-flight video (VR).");
            
            Video_gm.SetActive(false);
            is_Video_Open = false;
        }
    }
}