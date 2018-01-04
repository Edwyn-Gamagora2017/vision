using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

	public int lifeTime = 5;
	public int speed = 30;

	// Use this for initialization
	void Start () {
		GameObject.Destroy( gameObject, lifeTime );
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position += transform.forward*Time.deltaTime*speed;
	}
}
