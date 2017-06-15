using UnityEngine;
using System.Collections;

public class PrimaryGantry : MonoBehaviour
{
    // Tags for identifying the GameObjects in the simulation
	private string PRIMARY_GANTRY_TAG = "primaryGantry";
    private string PRIMARY_GANTRY_Z_POS_TAG = "primaryGantryZPos";
	private string PRIMARY_GANTRY_Z_NEG_TAG = "primaryGantryZNeg";

    // Primary Gantry objects
	private GameObject[] mPrimaryGantryList;
	private GameObject mPrimaryGantryPosBound;
	private GameObject mPrimaryGantryNegBound;

    private Vector3[] mMovementList;
    private float[] mSpeedList;
    private float zBoundPos;
    private float zBoundNeg;

    Vector3 beamMoveVector; 


	// Use this for initialization
	void Start(){
        mPrimaryGantryList = GameObject.FindGameObjectsWithTag(PRIMARY_GANTRY_TAG);
        mPrimaryGantryPosBound = GameObject.FindGameObjectWithTag(PRIMARY_GANTRY_Z_POS_TAG);
        mPrimaryGantryNegBound = GameObject.FindGameObjectWithTag(PRIMARY_GANTRY_Z_NEG_TAG);
        beamMoveVector = new Vector3(0, 0, 1);

        zBoundPos = mPrimaryGantryPosBound.transform.position.z;
		zBoundNeg = mPrimaryGantryNegBound.transform.position.z;

        mMovementList = new Vector3 [mPrimaryGantryList.Length];
        for (int i = 0; i < mMovementList.Length; i++){
            mMovementList[i] = beamMoveVector;
            beamMoveVector *= -1;
        }

        mSpeedList = new float[mPrimaryGantryList.Length];
        SetObjectSpeed();
	}

	// Update is called once per frame
	void Update(){
        for (int i = 0; i < mMovementList.Length; i++){
            GameObject gameObject = mPrimaryGantryList[i];
            gameObject.transform.Translate(mMovementList[i] * (mSpeedList[i] * Time.deltaTime));
		}
        SwitchDirections();
	}

    private bool CheckBounds(){
        bool switchDirections = false;
        GameObject obj = mPrimaryGantryList[0];

        for (int i = 1; i < mPrimaryGantryList.Length; i++){
            GameObject primaryGantry = mPrimaryGantryList[i];

            if (primaryGantry.transform.position.z >= zBoundPos - 2){
                switchDirections = true;
            }
            if (primaryGantry.transform.position.z <= zBoundNeg + 2){
                switchDirections = true;
            }
            float difference = primaryGantry.transform.localPosition.z - obj.transform.localPosition.z;
            if ( difference <= 2){
                switchDirections = true;
            }
            obj = primaryGantry;
        }
        return switchDirections;
    }

    private void SwitchDirections(){
        if (CheckBounds()){
            for (int i = 0; i < mPrimaryGantryList.Length; i++){
                mMovementList[i] *= -1;
			}
            SetObjectSpeed();
        }
    }

    private void SetObjectSpeed(){
		System.Random rand = new System.Random();
        for (int i = 0; i < mSpeedList.Length; i++){
            float speed = (float) (rand.NextDouble()) + 1;
            mSpeedList[i] = speed;
		}
	}

}
