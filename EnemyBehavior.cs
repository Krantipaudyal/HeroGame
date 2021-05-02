using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {

    private enum EnemyState
    {
        eRestState,
        eEnlargeState,
        eCWRotateState,
        eCCWRotateState,
        eShrinkState,
        stunnedState,
        eggState,


    };

    // All instances of Enemy shares this one WayPoint and EnemySystem
    static private WayPointSystem sWayPoints = null;
    static private EnemySpawnSystem sEnemySystem = null;
    static public void InitializeEnemySystem(EnemySpawnSystem s, WayPointSystem w) { sEnemySystem = s; sWayPoints = w; }

    private const float kSpeed = 20f;
    private int mWayPointIndex = 0;

    private const float kTurnRate = 0.03f/60f;
    private int mNumHit = 0;
    private const int kHitsToDestroy = 4;
    private const float kEnemyEnergyLost = 0.8f;

    //new
    private const float kSizeChangeFrames = 120f;
    private const float kRotateFrames = 80f;
    private const float kScaleRate = 0.5f / 60f;// around per second rate
    private const float kRotateRate = 45f / 60f;  // in degrees, around per second rate

    private int mStateFrameTick = 0;
    private EnemyState mState = EnemyState.eRestState;
    //end new

    // Use this for initialization
    void Start () {
        mWayPointIndex = sWayPoints.GetInitWayIndex();
	}
	
	// Update is called once per frame
	void Update () {
        UpdateFSM();
        if (mState == EnemyState.eRestState)
        {
            sWayPoints.CheckNextWayPoint(transform.position, ref mWayPointIndex);
            PointAtPosition(sWayPoints.WayPoint(mWayPointIndex), kTurnRate);
            transform.position += (kSpeed * Time.smoothDeltaTime) * transform.up;
        }
        
       
    }

    private void PointAtPosition(Vector3 p, float r)
    {
        Vector3 v = p - transform.position;
        transform.up = Vector3.LerpUnclamped(transform.up, v, r);
    }

    #region Trigger into chase or die
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("Enemy OnTriggerEnter");
        
        mStateFrameTick = 0;
        TriggerCheck(collision.gameObject);
        
    }

    private void TriggerCheck(GameObject g)
    {
        if (g.name == "Hero")
        {
            mState = EnemyState.eEnlargeState;
            //ThisEnemyIsHit();
            print("State is: "+mState);

        } else if (g.name == "Egg(Clone)")
        {
            mState = EnemyState.stunnedState;
            //print("State is: " + mState);
            //mState = EnemyState.eEnlargeState;
            //mStateFrameTick = 0;
            mNumHit++;
            if (mNumHit < kHitsToDestroy)
            {
                Color c = GetComponent<Renderer>().material.color;
                c.a = c.a * kEnemyEnergyLost;
                GetComponent<Renderer>().material.color = c;
            } else
            {
                ThisEnemyIsHit();
            }
        }
    }

    private void ThisEnemyIsHit()
    {
        sEnemySystem.OneEnemyDestroyed();
        Destroy(gameObject);
    }
    #endregion

# region FSM
    private void UpdateFSM()
    {
        switch (mState)
        {
            case EnemyState.eEnlargeState:
                ServiceEnlargeState();
                break;
            case EnemyState.eCWRotateState:
                ServiceCWState();
                break;
            case EnemyState.eCCWRotateState:
                ServiceCCWState();
                break;
            case EnemyState.eShrinkState:
                ServiceShrinkState();
                break;
            case EnemyState.eRestState:
                break;

            case EnemyState.eggState:
                ServiceEggState();
                break;
            case EnemyState.stunnedState:
                ServiceStunnedState();
                break;

        }
    }

    private void ServiceEnlargeState()
    {
        print("Current state: " + mState);
        if (mStateFrameTick > kSizeChangeFrames)
        {
            // Transite to next state
            mState = EnemyState.eCWRotateState;
            mStateFrameTick = 0;
        }
        else
        {
            mStateFrameTick++;

            // assume scale in X/Y are the same
            float s = transform.localScale.x;
            s += kScaleRate;
            transform.localScale = new Vector3(s, s, 1);
        }
    }

    private void ServiceShrinkState()
    {
        print("Current state: " + mState);
        if (mStateFrameTick > kSizeChangeFrames)
        {
            // Transite to next state
            mState = EnemyState.eRestState;
            mStateFrameTick = 0;
        }
        else
        {
            mStateFrameTick++;

            // assume scale in X/Y are the same
            float s = transform.localScale.x;
            s -= kScaleRate;
            transform.localScale = new Vector3(s, s, 1);
            
        }
        
    }

    private void ServiceCWState()
    {
        print("Current state: " + mState);
        if (mStateFrameTick > kRotateFrames)
        {
            // Transite to next state
            mState = EnemyState.eCCWRotateState;
            mStateFrameTick = 0;
        }
        else
        {
            mStateFrameTick++;

            Vector3 angles = transform.rotation.eulerAngles;
            angles.z += kRotateRate;
            transform.rotation = Quaternion.Euler(0, 0, angles.z);
        }
    }

    private void ServiceCCWState()
    {
        print("Current state: " + mState);
        if (mStateFrameTick > kRotateFrames)
        {
            // Transite to next state
            mState = EnemyState.eShrinkState;
            mStateFrameTick = 0;
        }
        else
        {
            mStateFrameTick++;

            Vector3 angles = transform.rotation.eulerAngles;
            angles.z -= kRotateRate;
            transform.rotation = Quaternion.Euler(0, 0, angles.z);
        }
    }
    #endregion


    private void ServiceStunnedState()
    {
        print("Current state: " + mState);
        if (mStateFrameTick > kSizeChangeFrames)
        {
            // Transite to next state
            mState = EnemyState.eggState;
            mStateFrameTick = 0;
        }
        else
        {
            mStateFrameTick++;

            // assume scale in X/Y are the same
            float s = transform.localScale.x;
            s += kScaleRate;
            transform.localScale = new Vector3(s, s, 1);
        }
    }

    private void ServiceEggState()
    {
        print("Current state: " + mState);
        if (mStateFrameTick > kSizeChangeFrames)
        {
            // Transite to next state
            mState = EnemyState.eRestState;
            mStateFrameTick = 0;
        }
        else
        {
            mStateFrameTick++;

            // assume scale in X/Y are the same
            float s = transform.localScale.x;
            s -= kScaleRate;
            transform.localScale = new Vector3(s, s, 1);

        }

    }

}
