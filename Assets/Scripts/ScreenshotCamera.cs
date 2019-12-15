using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotCamera : MonoBehaviour
{
    public bool takeScreenshot = false;
    public string screenshotName = "some_name";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(takeScreenshot)
        {
            ScreenCapture.CaptureScreenshot(screenshotName);
            takeScreenshot =false;
        }
    }
}
