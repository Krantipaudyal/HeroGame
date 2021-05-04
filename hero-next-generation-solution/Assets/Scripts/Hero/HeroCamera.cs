using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCamera : MonoBehaviour
{
 
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public GameObject objectToFollow;
    
    public float speed = 0.5f;
    
    //for camera shake
    public Transform target;
    public float shakeDuration = 1.0f;
    private float shakeMagnitude = 1.0f;
    private float dampingSpeed = 1.0f;
    Vector3 initialPos;


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

        shakeCamera();
    }

    void shakeCamera()
    {
         if (Input.GetKey("space")) 
         {
            if (shakeDuration > 0)
            {
            target.localPosition = initialPos + Random.insideUnitSphere * shakeMagnitude;
            
            shakeDuration -= Time.deltaTime * dampingSpeed;
            }
            else
            {
            shakeDuration = 0f;
            target.localPosition = initialPos;
            }
         }
    }


}
