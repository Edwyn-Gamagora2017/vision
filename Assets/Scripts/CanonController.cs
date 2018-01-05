using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonController : MonoBehaviour {

	[SerializeField]
	GameObject bulletPrefab;
	[SerializeField]
	Camera camera;

	public float shootInterval = 0.5f;
	float lastShoot = -1f;

	Vector3 initialPos;

	// Use this for initialization
	void Start () {
		initialPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.Space) ){
			Shoot();
		}
	}

	public void Shoot(){
		if( lastShoot < 0 || lastShoot < Time.time - shootInterval ){
			lastShoot = Time.time;
			Instantiate( bulletPrefab, transform );
		}
	}

	public void setHorizontalPosition( float x ){
		int xWidth = 8;
		Vector3 lastPos = transform.position;
		transform.position = new Vector3( initialPos.x-xWidth/2f+x*xWidth, lastPos.y, lastPos.z );
	}
	public void setVerticalPosition( float y ){
		int yWidth = 6;
		Vector3 lastPos = transform.position;
		transform.position = new Vector3( lastPos.x, initialPos.y-yWidth/2f+y*yWidth, lastPos.z );
		//transform.rotation = 
	}
}
