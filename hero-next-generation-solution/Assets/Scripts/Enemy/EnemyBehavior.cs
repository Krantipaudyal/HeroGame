using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {

    private enum EnemyState
    {
        ePatrolState,
        eEnlargeState,
        eCWRotateState,
        eCCWRotateState,
        eShrinkState,
        stunnedState,
        eggState,
        eChaseState,
    };

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
    private const float kRotateRate = 90f / 60f;  // in degrees, around per second rate

    private int mStateFrameTick = 0;
    private EnemyState mState = EnemyState.ePatrolState;



    //end new

    // Use this for initialization
    void Start () {
        mWayPointIndex = sWayPoints.GetInitWayIndex();

    }
	
	// Update is called once per frame
	void Update () {
        UpdateFSM();
        if (mState == EnemyState.ePatrolState)
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
            if (mState == EnemyState.ePatrolState) {
                mState = EnemyState.eCCWRotateState;
            }
            else if(mState == EnemyState.eChaseState)
            {
                ThisEnemyIsHit();
            }
            else
            {
                return;
            }
            //ThisEnemyIsHit();
            print("State is: "+mState);

        } else if (g.name == "Egg(Clone)")
        {
            if(mNumHit == 0)
            {
                mNumHit++;
                mState = EnemyState.stunnedState;
            }
            else if(mNumHit == 1)
            {
                mState = EnemyState.eggState;
                mNumHit++;
            }
            else
            {
                ThisEnemyIsHit();
            }

        }
    }

    private void ThisEnemyIsHit()
    {
        if (mState == EnemyState.eggState)
        {
            sEnemySystem.OneEnemyDestroyed();
            Destroy(gameObject);
        }
        else
        {
            sEnemySystem.enemyTouched();
            Destroy(gameObject);
        }

    }
    #endregion

    #region FSM
    //Sets all of the possible states for enemy to be in. PatrolState is default
    //For hero contact, order is CCWRotate, CWRotate, Chase, Enlarge, Shrink. For egg contact, it's stun then egg.
    //Enemy can only be destroyed by shooting it in egg state and touching hero in chase state

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
            case EnemyState.eChaseState:
                ServiceChaseState();
                break;
            case EnemyState.ePatrolState:
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
            mState = EnemyState.eShrinkState;
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
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            // Transite to next state
            mState = EnemyState.ePatrolState;
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

    private void ServiceChaseState()
    {
        float distance = Vector3.Distance(transform.position, GameObject.Find("Hero").transform.position);

        if (distance < 40)
        {
            PointAtPosition(GameObject.Find("Hero").transform.position, 200f); 
            //Rotation's supposed to be instant; I just set it to be really fast. Works, but maybe a better way to do this?

            transform.position += (kSpeed * Time.smoothDeltaTime) * transform.up;
        }

        else
        {
            mState = EnemyState.eEnlargeState;
        }
            
        
    }

    private void ServiceCWState()
    {
        print("Current state: " + mState);
        if (mStateFrameTick > kRotateFrames)
        {
            // Transit to next state
            mState = EnemyState.eChaseState;
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
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        if (mStateFrameTick > kRotateFrames)
        {
            // Transit to next state
            mState = EnemyState.eCWRotateState;
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
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load("Textures/enemyStunned", typeof(Sprite)) as Sprite;

        Vector3 angles = transform.rotation.eulerAngles;
        angles.z += kRotateRate;
        transform.rotation = Quaternion.Euler(0, 0, angles.z);
        return;
        
    }

    private void ServiceEggState()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load("Textures/egg", typeof(Sprite)) as Sprite;
        //Above changes the texture, it needs to lerp when hit

        return;

    }

}

