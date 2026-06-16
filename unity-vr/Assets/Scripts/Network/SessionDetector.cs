using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SessionDetector : MonoBehaviour
{
    [Header("Testing & Simulation")]
    public bool useKeyboardSimulation = true;

    private bool isUserPresent = false;
    
    // Explicitly target the XR InputDevice to resolve ambiguity
    private UnityEngine.XR.InputDevice headsetDevice;

    void Start()
    {
        if (!useKeyboardSimulation)
        {
            InitializeHeadset();
        }
        else
        {
            // Tagged as [Dev] to hide this instruction from the main system log
            Debug.Log("[Dev] [SESSION SIMULATION] Press 'S' to Simulate Wearing Headset. Press 'E' to Simulate Removing Headset.");
        }
    }

    void Update()
    {
        // 1. Keyboard Simulation Mode (For easy testing in Editor)
        if (useKeyboardSimulation)
        {
            // Ensure a keyboard is connected before checking for key presses
            if (Keyboard.current != null)
            {
                if (Keyboard.current.sKey.wasPressedThisFrame && !isUserPresent)
                {
                    isUserPresent = true;
                    
                    // Tagged as [System Event] because starting the session is a major timeline anchor
                    Debug.Log("[System Event] User put on the VR headset (Simulation - Session Started).");
                    SocketManager.instance.SendEvent("SESSION_START", null);
                }
                else if (Keyboard.current.eKey.wasPressedThisFrame && isUserPresent)
                {
                    isUserPresent = false;
                    
                    // Tagged as [System Event] because ending the session is a major timeline anchor
                    Debug.Log("[System Event] User removed the VR headset (Simulation - Session Ended).");
                    SocketManager.instance.SendEvent("SESSION_END", null);
                }
            }
            return; // Skip hardware checks if simulating
        }

        // 2. Real Hardware Mode
        if (!headsetDevice.isValid)
        {
            InitializeHeadset();
            return;
        }

        bool presenceValue;
        // Explicitly target the XR CommonUsages to resolve ambiguity
        if (headsetDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.userPresence, out presenceValue))
        {
            if (presenceValue && !isUserPresent)
            {
                isUserPresent = true;
                
                // Added critical event log for real hardware
                Debug.Log("[System Event] User put on the VR headset (Session Started).");
                SocketManager.instance.SendEvent("SESSION_START", null);
            }
            else if (!presenceValue && isUserPresent)
            {
                isUserPresent = false;
                
                // Added critical event log for real hardware - could indicate panic/overload mid-experience
                Debug.Log("[System Event] User removed the VR headset (Session Ended).");
                SocketManager.instance.SendEvent("SESSION_END", null);
            }
        }
    }

    private void InitializeHeadset()
    {
        // Explicitly target the XR InputDevice here as well
        List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, devices);

        if (devices.Count > 0)
        {
            headsetDevice = devices[0];
        }
    }
}