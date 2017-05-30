using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourScript : MonoBehaviour {
	int xDirection = 1;
	int zDirection = 1;
	int flowerBound = 1;
	int beamBound = 7;
	GameObject[] flowers;
	GameObject[] beams;
	Vector3 flowerMoveVector;
	Vector3 beamMoveVector;
	int temp = 0;
	// Use this for initialization
	void Start () {
		flowerMoveVector = new Vector3 (1,0,0);
		beamMoveVector = new Vector3 (0,0,1);

		flowers = GameObject.FindGameObjectsWithTag ("flowers");
		beams = GameObject.FindGameObjectsWithTag ("beam");
	}
	
	// Update is called once per frame
	void Update () {
		if (temp < 3) {
			moveSimple (flowers);
		}
		else if (temp > 2 && temp < 6)
			moveSimple (beams);
	}

	void moveSimple(GameObject[] objects){
		
		checkBounds (objects);
		if (objects [0].CompareTag ("beam")) {
			foreach (GameObject obj in objects) {
				obj.transform.Translate (beamMoveVector * (3 * Time.deltaTime));
				print ("switch");
			}
		}
		if (objects [0].CompareTag ("flowers")) {
			foreach (GameObject obj in objects) {
				obj.transform.Translate (flowerMoveVector * (3 * Time.deltaTime));
			}
		}
	}
	void checkBounds(GameObject[] objects){
		
		if (objects [0].CompareTag ("beam")) {
			foreach (GameObject obj in objects) {
				if (obj.transform.position.z > 7.25 || obj.transform.position.z < -6.75) {
					zDirection *= -1;
					temp++;
					beamMoveVector.Set (0, 0, zDirection);
					return;
				}
			}
		} else if (objects [0].CompareTag ("flowers")) {
			foreach (GameObject obj in objects) {
				if (obj.transform.localPosition.y > 1 || obj.transform.localPosition.y < -1) {
					xDirection *= -1;
					temp++;
					flowerMoveVector.Set (xDirection, 0, 0);
					print ("flower" + temp);
					return;
				}	
			}
		}

		return;
	}
}
