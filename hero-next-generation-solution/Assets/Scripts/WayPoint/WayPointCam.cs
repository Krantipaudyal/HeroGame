using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointCam : MonoBehaviour
{

    private bool isActive;
    public Camera wayPointCamera;

    // 
    //Start is called before the first frame update
    void Start()
    {
        //at start this needs to be false for both 
        gameObject.SetActive(true);
        wayPointCamera.enabled = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        activateCamera();
    }

    //if waypoints are triggered othe camera needs to set active.
    //call this function from waypoints.cs to activate camera
    void activateCamera()
    {
        //if waypoint object trigers
        //needs if statement for that
        if(isActive == false)
        {
            isActive = true;
            wayPointCamera.enabled = true;
        }
    }
}
