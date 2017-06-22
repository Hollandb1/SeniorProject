using UnityEngine;
using System.Collections;

public class PrimaryGantry : MonoBehaviour
{
    // Tags for identifying the GameObjects in the simulation
	private string PRIMARY_GANTRY1_TAG = "primaryGantry";
	private string PRIMARY_GANTRY2_TAG = "primaryGantry2";
	private string PRIMARY_GANTRY3_TAG = "primaryGantry3";
	private string PRIMARY_GANTRY4_TAG = "primaryGantry4";

    private string PRIMARY_GANTRY_Z_POS_TAG = "primaryGantryZPos";
	private string PRIMARY_GANTRY_Z_NEG_TAG = "primaryGantryZNeg";

	private int PRIMARY_GANTRY_SECTIONS = 4;
	private int PRIMARY_GANTRY_BEAMS = 3; //this will be 5 when we get the ends moving
    // Primary Gantry objects
	private GameObject[][] mPrimaryGantryList;
	private GameObject mPrimaryGantryPosBound;
	private GameObject mPrimaryGantryNegBound;

	private Vector3[][] mMovementList;
    private float[][] mSpeedList;
    private float zBoundPos;
    private float zBoundNeg;
	private float speedOffset = 0.5f;
    Vector3 beamMoveVector; 


	// Use this for initialization
	void Start(){
		mPrimaryGantryList = new GameObject[4][];
		for (int i = 0; i < mPrimaryGantryList.Length; i++) {
			mPrimaryGantryList[i] = new GameObject[3];
		}
        mPrimaryGantryList[0] = GameObject.FindGameObjectsWithTag(PRIMARY_GANTRY1_TAG);
		mPrimaryGantryList[1] = GameObject.FindGameObjectsWithTag(PRIMARY_GANTRY2_TAG);
		mPrimaryGantryList[2] = GameObject.FindGameObjectsWithTag(PRIMARY_GANTRY3_TAG);
		mPrimaryGantryList[3] = GameObject.FindGameObjectsWithTag(PRIMARY_GANTRY4_TAG);
        mPrimaryGantryPosBound = GameObject.FindGameObjectWithTag(PRIMARY_GANTRY_Z_POS_TAG);
        mPrimaryGantryNegBound = GameObject.FindGameObjectWithTag(PRIMARY_GANTRY_Z_NEG_TAG);
        beamMoveVector = new Vector3(0, 0, 1);
        zBoundPos = mPrimaryGantryPosBound.transform.localPosition.z;
		zBoundNeg = mPrimaryGantryNegBound.transform.localPosition.z;

		mMovementList = new Vector3[PRIMARY_GANTRY_SECTIONS][];
		mSpeedList = new float[PRIMARY_GANTRY_SECTIONS][];

		for (int i = 0; i < mMovementList.Length; i++) {
			mMovementList[i] = new Vector3[3];
			mSpeedList [i] = new float[3];
		}

        for (int i = 0; i < mMovementList.Length; i++){
			for (int j = 0; j < mMovementList [0].Length; j++) {
				mMovementList[i][j] = beamMoveVector;
				beamMoveVector *= -1;
			}
        }
		for (int i = 0; i < mMovementList.Length; i++) {
			SetObjectSpeed (i);
		}
	}

	// Update is called once per frame
	void FixedUpdate(){
        for (int i = 0; i < mMovementList.Length; i++){
			for (int j = 0; j < mMovementList[0].Length; j++) {
				GameObject gameObject = mPrimaryGantryList [i][j];
				gameObject.transform.Translate (mMovementList [i][j] * (mSpeedList [i][j] * Time.deltaTime / 2));
			}
		}
        SwitchDirections();
	}

	private bool CheckBounds(int index){
        bool switchDirections = false;
		GameObject obj = mPrimaryGantryList[index][0];

		for (int i = 1; i < mPrimaryGantryList[index].Length; i++){
			GameObject primaryGantry = mPrimaryGantryList[index][i];
            if (obj.transform.localPosition.z < zBoundNeg + 2){
				return true;
            }
			if (primaryGantry.transform.localPosition.z > zBoundPos - 2){
				return true;
            }
			float difference = Mathf.Abs(primaryGantry.transform.localPosition.z - obj.transform.localPosition.z);
            if ( difference < 2){
				return true;
            }
            obj = primaryGantry;
        }
        return switchDirections;
    }

    private void SwitchDirections(){
		for (int j = 0; j < mPrimaryGantryList.Length; j++) {
			if (CheckBounds (j)) {
				for (int i = 0; i < mPrimaryGantryList[j].Length; i++) {
					mMovementList [j][i] *= -1;
				}
				SetObjectSpeed (j);
			}
		}
    }

	private void SetObjectSpeed(int index){
		System.Random rand = new System.Random();
        for (int i = 0; i < mSpeedList[index].Length; i++){
			float speed = (float)(rand.NextDouble ()) + speedOffset;
			mSpeedList [index][i] = speed;
		}
	}
}
