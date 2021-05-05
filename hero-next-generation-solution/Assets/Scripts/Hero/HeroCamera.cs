using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCamera : MonoBehaviour
{
    public bool canShake = true;
    //public float smoothSpeed = 0.125f; //is this needed?
    //public Vector3 offset; //^^^
    public GameObject objectToFollow;
    
    public float speed = 0.5f;
    
    //for camera shake
    public Transform target;
    public float shakeMagnitude = 1.0f;


    public string getHeroCamState()
    {
        return "Egg Cool Down Hero Camera";
    }
    void Start()
    {
        objectToFollow = GameObject.Find("Hero");
        //retrieving transform of hero camera
        if(target == null)
        {
            target = GetComponent(typeof(Transform)) as Transform;
        }
    }

    
    void Update () {
        float interpolation = speed * Time.deltaTime;
        
        Vector3 position = this.transform.position;
        position.y = Mathf.Lerp(this.transform.position.y, objectToFollow.transform.position.y, interpolation);
        position.x = Mathf.Lerp(this.transform.position.x, objectToFollow.transform.position.x, interpolation);
        
        this.transform.position = position;

        //canShake prevents this from being called every single Update
        if (Input.GetKey("space") && canShake)
        {
            canShake = false;
            StartCoroutine(shakeCamera());
        }
    }

    IEnumerator shakeCamera()
    {
        Debug.Log("CameraShake");
        Vector3 originalPosition = transform.position;
        float dampening = 1f; //Must always stay 1f or time will change!
        while (dampening >= 0f)
        {
            float x = Random.Range(originalPosition.x - shakeMagnitude * dampening,
                originalPosition.x + shakeMagnitude * dampening);
            float y = Random.Range(originalPosition.y - shakeMagnitude * dampening, 
                originalPosition.y + shakeMagnitude * dampening);

            transform.position = new Vector3(x, y, -1);

            //If both are 1/60f, lasts for 1 second with a "frame rate" of 60fps
            //Dampening is more so FPS is still 60 but it ends faster. 1/6 of a second
            //is the total time now. Egg spawn rate was changed to match this
            dampening -= 1f / 10f;
            yield return new WaitForSeconds(1f / 60f);
        }
        transform.position = originalPosition;
        canShake = true; //Buffer to prevent call every single frame
    }


}
