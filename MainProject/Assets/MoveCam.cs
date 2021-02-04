using UnityEngine;
using UnityEngine.SpatialTracking;

public class MoveCam : MonoBehaviour
{
    private float y;
    
    private void Start()
    {
        y = transform.localPosition.y;
    }

    private void Update()
    {
        y += Mathf.Sin(Time.time) * .1f * Time.deltaTime;
        var pos = transform.localPosition;
        pos.y = y;
        transform.localPosition = pos;

        Debug.Log(Camera.main.projectionMatrix);

        if (Time.frameCount % 300 == 0)
        {
            GetComponent<TrackedPoseDriver>().enabled = false;
            Camera.main.fieldOfView = 60;
            Debug.Log("DISABLE TRACKED POSE DRIVER TO RESET CAM");
        }
        else
        {
            GetComponent<TrackedPoseDriver>().enabled = true;
        }
    }
}
