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
        var t = transform;
        var pos = t.localPosition;
        pos.y = y;
        t.localPosition = pos;
    }
}
