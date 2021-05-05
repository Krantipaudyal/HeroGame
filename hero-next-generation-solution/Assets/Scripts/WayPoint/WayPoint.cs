using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    private Vector3 mInitPosition = Vector3.zero;
    private int mHitCount = 0;
    private const int kHitLimit = 3;
    private const float kRepositionRange = 15f; // +- this value
    private Color mNormalColor = Color.white;
    
    //to activate camera of waypoint
    private WayPointCam toActivate;
    // Start is called before the first frame update
    void Start()
    {
        mInitPosition = transform.position;
    }

    private void Reposition() 
    {
        Vector3 p = mInitPosition;
        p += new Vector3(Random.Range(-kRepositionRange, kRepositionRange),
                         Random.Range(-kRepositionRange, kRepositionRange),
                         0f);
        transform.position = p;
        GetComponent<SpriteRenderer>().color = mNormalColor;
    }

    IEnumerator Shake()
    {
        Vector3 originalPosition = transform.position;
        float tempTime = mHitCount + 1;
        Debug.Log(tempTime);
        while (tempTime >= 0f)
        {
            float x = Random.Range(originalPosition.x-1*tempTime, originalPosition.x+1*tempTime);
            float y = Random.Range(originalPosition.y-1*tempTime, originalPosition.y+1*tempTime);
            transform.position = new Vector3(x, y, -1);
            //Debug.Log(tempTime);
            tempTime -= 1f/60f;
            yield return new WaitForSeconds(1f/60f);
        }
        transform.position = originalPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Egg(Clone)")
        {
            if(mHitCount < 3)
            {
                StartCoroutine(Shake());
            }
            mHitCount++;
            Color c = mNormalColor * (float)(kHitLimit - mHitCount + 1) / (float)(kHitLimit + 1);
            GetComponent<SpriteRenderer>().color = c;
            if (mHitCount > kHitLimit)
            {
                StopAllCoroutines();
                mHitCount = 0;
                Reposition();
            }
        }
    }
}
