using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public float speed;
    public Text countText;
    public Text debugText;


    bool isObjectObtained = false;

    public GameObject mPickUpPrefab;
    private GameObject mPickUp;
    private Rigidbody mBall;
    private int count;
    private float originX = -10;
    private float originZ = -10;


    // Gets reference to the attached rigid body if there is one.
    void Start(){
        mBall = GetComponent<Rigidbody>();
        count = 0;
        SetCountText ();
        if(mPickUp == null){
            mPickUp = GameObject.FindGameObjectWithTag(mPickUpPrefab.tag);
        }
        ShowPickUpPosition();
        Debug.Log("This is a debug message");
    }


    // Called before rendering a frame
    void Update(){
        
    }

    void LateUpdate(){
        if (!isObjectObtained) {
            mBall.AddForce(GetMovementVector() * speed);
            //mBall.AddForce(GetParabolaMovementVector() * speed);
        }
    }

    // Called just before performing any physics calculations
    void FixedUpdate(){

    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.CompareTag(mPickUpPrefab.tag)) {
            mPickUp.SetActive (false);
            count++;
            SetCountText ();
            StopBallMovement();
            RespawnPickUp();
            StartBallMovement();
        }
    }

    private void StartBallMovement(){
        isObjectObtained = false;
        mBall.isKinematic = false;
    }

    private void StopBallMovement(){
        isObjectObtained = true;
        mBall.velocity = Vector3.zero;
        mBall.isKinematic = true;
    }

    private void SetCountText(){
        countText.text = "Count: " + count.ToString ();
    }

    private void ShowPickUpPosition() {
        Vector3 pickUpPosition = GetPickUpPositon();

        // Only care about x and z since this makes the horizontal plane in Unity.
        float x = pickUpPosition.x;
        float z = pickUpPosition.z;
        String s = String.Format("Postion of object: ({0}, {1})", x, z);
        UpdateDebugText(s);
    }

    private void RespawnPickUp(){
        Instantiate(mPickUpPrefab);
        mPickUp = GameObject.FindGameObjectWithTag(mPickUpPrefab.tag);
        System.Random rand = new System.Random();

        float x = rand.Next(-9, 9);
        float y = 0.5f;
        float z = rand.Next(-9, 9);

        UpdateDebugText(String.Format("Postion of object: ({0}, {1}, {2})", x, y, z));
        mPickUp.transform.localPosition = new Vector3(x, y, z);
        mPickUp.SetActive(true);
    }

    /**
     * Gets a Vector3 Object for moving the ball directly to the destination 
     * point.
     */
    private Vector3 GetMovementVector(){
        Vector3 item = GetPickUpPositon();
        Vector3 player = mBall.transform.localPosition;
        float x = 0;
        float z = 0;

        // Get the x direction to move in.
        if(player.x <= item.x){
            x = item.x - player.x;
        } else{
            x = -(player.x - item.x);
        }

        // Get the z direction to move in. 
        if(player.z <= item.z){
            z = item.z - player.z;
        } else {
            z = -(player.z - item.z);
        }

        return new Vector3(x, 0, z);
    }

    private Vector3 GetParabolaMovementVector(){
        Vector3 item = GetPickUpPositon();
        Vector3 player = mBall.transform.localPosition;
        float x = 0.2f;
        float z = (x * (player.x)) + (49 / 70);
        return new Vector3(x, 0, z);
    }

    private float GetParabola(){
        //Vector3 item = GetPickUpPositon();
        //Vector3 player = mBall.transform.localPosition;
        //float a = 0;
        //float b = 0;
        //float c = 0;

        //// a*x^2 + b*x + c = f(x)
        //float px1 = item.x;
        //float py1 = item.y;
        //float px2 = player.x;
        //float py2 = player.y;
        return 0;
    }

    /**
     * Gets the position of the Pick Up object.
     */
    private Vector3 GetPickUpPositon(){
        return mPickUp.transform.localPosition;
    }

    /**
     * Print text to the screen.
     */
    private void UpdateDebugText(string text){
        debugText.text = text;
    }
}

