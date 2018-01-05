using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscController : MonoBehaviour {

	int speed = 3;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position -= transform.forward*Time.deltaTime*speed;

		if( Input.GetKeyDown( KeyCode.Space ) ){
			Hit();
		}
	}

	bool Hit(){
		this.GetComponent<ChangeColor>().change();
		return true;
	}
}
