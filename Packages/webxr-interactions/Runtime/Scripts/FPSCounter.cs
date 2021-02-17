using UnityEngine;
using UnityEngine.UI;

namespace WebXR.Interactions
{
  public class FPSCounter : MonoBehaviour
  {
    public TextMesh text;
    public Text text2;
    
    private float fps = 0;
    private float framesCount = 0;
    private float lastCheck = 0;
    private float rate = 0.5f;


    void Update()
    {
      framesCount++;
      if (Time.time >= lastCheck + rate)
      {
        fps = framesCount / (Time.time - lastCheck);
        lastCheck = Time.time;
        framesCount = 0;
        var res = fps.ToString("F0");
        text.text = res;
        if (text2) text2.text = res;
      }
    }
  }
}
