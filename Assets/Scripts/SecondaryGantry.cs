using System;
using UnityEngine;
using System.Collections;

public class SecondaryGantry : MonoBehaviour
{
	public float moveSpeed;
	
    // Tags for identifying the GameObjects in the simulation
	private readonly string SECONDARY_GANTRY0 = "secondaryGantry0";
	private readonly string SECONDARY_GANTRY1 = "secondaryGantry1";
	private readonly string SECONDARY_GANTRY2 = "secondaryGantry2";
	private readonly string SECONDARY_GANTRY3 = "secondaryGantry3";
	private readonly string SECONDARY_GANTRY4 = "secondaryGantry4";
	
	private readonly string _SECTION0 = "section0";
	private readonly string _SECTION1 = "section1";
	private readonly string _SECTION2 = "section2";
	private readonly string _SECTION3 = "section3";
	
	private GameObject _section0;
	private GameObject _section1;
	private GameObject _section2;
	private GameObject _section3;

	private static bool _isAudioDesired = true;
	
	//this will be 5 when we get the ends moving
	private int PRIMARY_GANTRY_SECTIONS = 4;
	private int PRIMARY_GANTRY_BEAMS = 3; 
	
    // Secondary Gantry objects
	private GameObject[] _gantry0;
	private GameObject[] _gantry1;
	private GameObject[] _gantry2;
	private GameObject[] _gantry3;	

	private Vector3 _topBound;
	private Vector3 _lowBound;

	// Corresponds to each gantry 0-4
	private readonly Vector3[] _bottomStatePositions = {
		new Vector3(9, -0.5f, 10.5f), 
		new Vector3(9, -0.5f, 8.5f), 
		new Vector3(9, -0.5f, 6.5f), 
		new Vector3(9, -0.5f, 4.5f), 
		new Vector3(9, -0.5f, 2.5f)
	};

	private readonly Vector3[] _balancedState =
	{
		new Vector3(9, -0.5f, 10.5f),
		new Vector3(9, -0.5f, 5f),
		new Vector3(9, -0.5f, 0f),
		new Vector3(9, -0.5f, -5f),
		new Vector3(9, -0.5f, -10f),
	};
	
	// Corresponds to each gantry 0-4
	private readonly Vector3[] _topStatePositions = {
		new Vector3(9, -0.5f, -2f), 
		new Vector3(9, -0.5f, -4f), 
		new Vector3(9, -0.5f, -6f), 
		new Vector3(9, -0.5f, -8f), 
		new Vector3(9, -0.5f, -10f)
	};
	
	// Corresponds to each gantry 0-4
	private readonly Vector3[] _splitTopHeavy = {
		new Vector3(9, -0.5f, 10.5f), 
		new Vector3(9, -0.5f, 8.5f), 
		new Vector3(9, -0.5f, -6f), 
		new Vector3(9, -0.5f, -8f), 
		new Vector3(9, -0.5f, -10f)
	};
	
	
	
 	private GameObject[][] mSecondaryGantryList;
	private GameObject mPrimaryGantryPosBound;
	private GameObject mPrimaryGantryNegBound;

	private Vector3[][] mMovementList;
    private float[][] mSpeedList;
    private float zBoundPos;
    private float zBoundNeg;
	private float speedOffset = 0.5f;
    Vector3 beamMoveVector;


	GameObject[] GetSecondaryGantries(GameObject section)
	{
		var gantry = new GameObject[5];
		GameObject.FindWithTag(SECONDARY_GANTRY0);
		gantry[0] = section.transform.GetChild(0).gameObject;
		gantry[1] = section.transform.GetChild(1).gameObject;
		gantry[2] = section.transform.GetChild(2).gameObject;
		gantry[3] = section.transform.GetChild(3).gameObject;
		gantry[4] = section.transform.GetChild(4).gameObject;
		
		return gantry;
	}

	// Use this for initialization
	void Start(){
		
		// Each of the sections containing 5 secondary gantrys
		_section0 = GameObject.FindWithTag(_SECTION0);
		_section1 = GameObject.FindWithTag(_SECTION1);
		_section2 = GameObject.FindWithTag(_SECTION2);
		_section3 = GameObject.FindWithTag(_SECTION3);
		
		// Arrays to control the secondary gantrys in each section.
		_gantry0 = GetSecondaryGantries(_section0);
		_gantry1 = GetSecondaryGantries(_section1);
		_gantry2 = GetSecondaryGantries(_section2);
		_gantry3 = GetSecondaryGantries(_section3);
		
        mPrimaryGantryPosBound = GameObject.FindGameObjectWithTag(SECONDARY_GANTRY0);
        mPrimaryGantryNegBound = GameObject.FindGameObjectWithTag(SECONDARY_GANTRY4);

		_topBound = mPrimaryGantryPosBound.transform.localPosition;
		_lowBound = mPrimaryGantryNegBound.transform.localPosition;
		
		print(String.Format("Top Bound: {0}", _topBound));
		print(String.Format("Low Bound: {0}", _lowBound));


        beamMoveVector = new Vector3(0, 0, 1);
        zBoundPos = mPrimaryGantryPosBound.transform.localPosition.z;
		zBoundNeg = mPrimaryGantryNegBound.transform.localPosition.z;
		print (zBoundNeg);
		print(zBoundPos);
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

	
	void Move(GameObject[] gantryList, Vector3[] directions)
	{
		float step = moveSpeed * Time.deltaTime;
				
		for (int i = 0; i < gantryList.Length; i++)
		{
			var gantry = gantryList[i];
			var currentPos = gantry.transform.localPosition;
			var target = directions[i];
			gantry.transform.localPosition = Vector3.MoveTowards(currentPos, target, step);
		}
	}
	

	// Update is called once per frame
	void FixedUpdate(){
		if (_isAudioDesired)
		{
			Move(_gantry0, _balancedState);
			Move(_gantry1, _balancedState);
			Move(_gantry2, _balancedState);
			Move(_gantry3, _balancedState);
		}
		else
		{
			Move(_gantry0, _bottomStatePositions);
			Move(_gantry1, _bottomStatePositions);
			Move(_gantry2, _topStatePositions);
			Move(_gantry3, _topStatePositions);
		}

	}

	private bool CheckBounds(int index, GameObject[] gantryList){
        bool switchDirections = false;
		GameObject obj = gantryList[0];

		for (int i = 1; i < mSecondaryGantryList[index].Length; i++){
			GameObject primaryGantry = mSecondaryGantryList[index][i];
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

    private void SwitchDirections(int section, GameObject[] gantryList){
		if (CheckBounds (section, gantryList)) {
			for (int i = 0; i < mSecondaryGantryList[section].Length; i++) {
				mMovementList [section][i] *= -1;
			}
			SetObjectSpeed (section);
		}
    }

	private void SetObjectSpeed(int index){
		System.Random rand = new System.Random();
        for (int i = 0; i < mSpeedList[index].Length; i++){
			float speed = (float)(.5 + speedOffset);
			mSpeedList [index][i] = speed;
		}
	}

	public static void SetAudioDesired(bool isDesired)
	{
		_isAudioDesired = isDesired;
	}
}
