using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour {

	MeshRenderer mRenderer;

	public Color originalColor = Color.blue;
	public Color hitColor = Color.green;

	// Use this for initialization
	void Start () {
		mRenderer = GetComponent<MeshRenderer> ();
		backToColor();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void change(){
		mRenderer.material.color = hitColor;
		Invoke ("backToColor", 1);
	}

	void backToColor(){
		mRenderer.material.color = originalColor;
	}
}
