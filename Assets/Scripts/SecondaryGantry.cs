using UnityEngine;
using System.Collections;

public class SecondaryGantry : MonoBehaviour
{
    private string SECONDARY_GANTRY_TAG = "secondaryGantry";
    private string SECONDARY_GANTRY_Z_NEG_TAG = "secondaryGantryBottom";
    private string SECONDARY_GANTRY_Z_POS_TAG = "secondaryGantryTop";

    private GameObject[] mSecondaryGantryList;
    private GameObject mSecondaryGantryPosBound;
    private GameObject mSecondaryGantryNegBound;

    private float zBoundPos;
    private float zBoundNeg;

    Vector3 beamMoveVector;


	// Use this for initialization
	void Start() {
		mSecondaryGantryList = GameObject.FindGameObjectsWithTag(SECONDARY_GANTRY_TAG);
        mSecondaryGantryPosBound = GameObject.FindGameObjectWithTag(SECONDARY_GANTRY_Z_POS_TAG);
        mSecondaryGantryNegBound = GameObject.FindGameObjectWithTag(SECONDARY_GANTRY_Z_NEG_TAG);
        beamMoveVector = new Vector3(0, 0, 1);

        zBoundPos = mSecondaryGantryPosBound.transform.localPosition.z;
        zBoundNeg = mSecondaryGantryNegBound.transform.localPosition.z;
	}

	// Update is called once per frame
	void Update()
	{
		foreach (GameObject gameobject in mSecondaryGantryList) {
			gameobject.transform.Translate(beamMoveVector * (3 * Time.deltaTime));
		}
		SwitchDirections();
	}

	private bool CheckBounds() {
		bool switchDirections = false;
		foreach (GameObject secondaryGantry in mSecondaryGantryList) {
            
            if (secondaryGantry.transform.localPosition.z >= zBoundPos - 1) {
				switchDirections = true;
			}

            if (secondaryGantry.transform.localPosition.z >= zBoundNeg + 1) {
				switchDirections = true;
			}

		}
		return switchDirections;
	}

	private void SwitchDirections(){
		if (CheckBounds()) {
			beamMoveVector *= -1;
		}
	}
}
