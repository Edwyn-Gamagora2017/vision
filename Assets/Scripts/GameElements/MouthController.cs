using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthController : FaceAction {

	[SerializeField]
	Animator mouthAnimator;

	[SerializeField]
	ChangeColor colorWrong;
	[SerializeField]
	ChangeColor colorRight;

	#region implemented abstract members of FaceAction
	public override void OpenMouth ()
	{
		Debug.Log("open");
		if( mouthAnimator != null ){
			mouthAnimator.SetTrigger( "activate" );
		}
	}
	public override void CloseMouth ()
	{
	}
	#endregion

	Vector3 initialPos;

	// Use this for initialization
	void Start () {
		initialPos = transform.position;

		colorRight.originalColor = Color.green;
		colorWrong.originalColor = Color.green;
	}
	
	// Update is called once per frame
	void Update () {
		//setHorizontalPosition( this.horizontalFacePosition );
	}

	public void setHorizontalPosition( float x ){
		int xWidth = 8;
		Vector3 lastPos = transform.position;
		transform.position = new Vector3( initialPos.x-xWidth/2f+x*xWidth, lastPos.y, lastPos.z );
	}
}
