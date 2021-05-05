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
        gameObject.SetActive(false);
        wayPointCamera.enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    //if waypoints are triggered othe camera needs to set active.
    //call this function from waypoints.cs to activate camera
    public IEnumerator activateCamera(float activeTime)
    {

        if(isActive == true)
        {
            isActive = true;
            wayPointCamera.enabled = true;
        }
        yield return new WaitForSeconds(activeTime);
    }
}
