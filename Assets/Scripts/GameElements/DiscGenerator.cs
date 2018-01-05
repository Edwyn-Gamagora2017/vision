using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscGenerator : MonoBehaviour {

	[SerializeField]
	GameObject discPrefab;

	[SerializeField]
	GameObject[] spawners;

	float shootInterval = 1f;
	float lastShoot = -1f;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		if( lastShoot < 0 || lastShoot+shootInterval < Time.time ){
			Shoot();
		}
	}

	public void Shoot(){
		lastShoot = Time.time;
		if( spawners != null && spawners.Length > 0 ){
			int spawnerIndex = Random.Range( 0, spawners.Length );
			GameObject spawner = spawners[spawnerIndex];
			Instantiate( discPrefab, spawner.transform.position, transform.rotation );
		}
	}
}
