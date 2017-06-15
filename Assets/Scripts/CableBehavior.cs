using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CableBehavior : MonoBehaviour {

    public float MIN_CABLE_LENGTH;
    public float MAX_CABLE_LENGTH;
    public float SPEED = 1;
    private bool extendCable = true;

    private float mDeltaHeight;
    private GameObject mCable;
    private string CABLE_TAG = "cable";


	// Use this for initialization
	void Start () {
        mCable = GameObject.FindWithTag(CABLE_TAG);
		mDeltaHeight = (MAX_CABLE_LENGTH - MIN_CABLE_LENGTH) / 1000;
		//SHOWING = MAX_CABLE_LENGTH - OFFSET;
        //Debug.Log(String.Format("Max cable: {0} , Offset: {1}, SHowing: {2}, Delta: {3}", MAX_CABLE_LENGTH, OFFSET, SHOWING, mDeltaHeight));
	}
	
	// Update is called once per frame
	void Update () {

        float step = SPEED * Time.deltaTime;
        float offset = transform.localPosition.x;
        float showing = MAX_CABLE_LENGTH - offset;

        if (showing < MAX_CABLE_LENGTH && extendCable){
            transform.Translate(Vector3.down * step);
        } else if (showing > MAX_CABLE_LENGTH && extendCable){
            extendCable = false;
        }

        if (showing > MIN_CABLE_LENGTH && !extendCable){
            transform.Translate(Vector3.up * step);
		}
        else if (showing < MIN_CABLE_LENGTH && !extendCable){
            extendCable = true;
		}

	}
}
