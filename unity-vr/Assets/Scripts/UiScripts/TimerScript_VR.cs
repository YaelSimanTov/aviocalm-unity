using UnityEngine;

public class TimerScript_VR : MonoBehaviour
{
    public float time_spent;
    public static TimerScript_VR instance;

    private void Awake()
    {
        instance = this;
        //DontDestroyOnLoad(gameObject); // Optional: Keep it across scenes
    }

    private void Update()
    {
        time_spent += Time.deltaTime;
        //Debug.Log("VR Timer running: " + time_spent.ToString("F1") + "s");
    }
}
