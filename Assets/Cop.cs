using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class Cop : MonoBehaviour {

	Camera camera;
	float scalingFactor = .1f; // Bigger for slower
	bool rotate = false;
	float rotate_timer_;
	float rotate_time = .5f;
	float action_timer;
	float action_time = .5f;
	bool action_flag;
	Quaternion rotation;

	List<CarSpawner> spawners = new List<CarSpawner>();
	CarSpawner spawner_in_view_;
	// Use this for initialization
	void Start () {
		camera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
		CarSpawner[] hinges = FindObjectsOfType(typeof(CarSpawner)) as CarSpawner[];
		foreach (CarSpawner hinge in hinges) 
		{
			spawners.Add(hinge);
		}
		//Debug.Log (camera.name);
		//GameObject.Find
		rotate_timer_ = 0;
		rotation = this.transform.rotation;
		spawner_in_view_ = null;
	
	}
	
	// Update is called once per frame
	void Update () {
		rotate_timer_ -= Time.deltaTime;
		action_timer -= Time.deltaTime;

		//problems if you spam rotate
		if(rotate_timer_<0)
		{
			if (Input.GetKeyDown ("w")) 			
				rotate90 (3);			
			else if (Input.GetKeyDown ("s"))
				rotate90 (4);
			else if (Input.GetKeyDown ("a"))
				rotate90 (2);
			else if (Input.GetKeyDown ("d"))
				rotate90 (1);
			spawner_in_view_ = null;
		}
		//can be nothing

		this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, Time.deltaTime/scalingFactor);


		if (spawner_in_view_ == null)
			findCarSpawner ();

		if (Input.GetKeyDown ("space"))
			printSpawner ();

		if (action_timer > 0)
		{
			action_flag = true;
		}
		else{
			action_flag = false;
		}
		if (!action_flag) {
			rightHandActions ();
			leftHandActions(); 
		}
	
	}

	private void rotate90(int axis)
	{
		rotation = this.transform.rotation;
		switch (axis)
		{
			//y
		case 1:
			rotation *= Quaternion.Euler(0, 90, 0); 
			break;
			//y
		case 2:
			rotation *= Quaternion.Euler(0, -90, 0);
			break;
		case 3:
			rotation *= Quaternion.Euler(90, 0, 0);
			break;
		case 4:
			rotation *= Quaternion.Euler(-90, 0, 0);
			break;

		}
		rotate_timer_ = rotate_time;

			
	}

	private void printSpawner()
	{
		Debug.Log (spawner_in_view_.gameObject.name);
	}

	private void findCarSpawner()
	{
		for (int i=0;i<spawners.Count;i++)
		{
			CarSpawner c = spawners[i];
			Vector3 viewPos = camera.WorldToViewportPoint(c.transform.position);
			//visible?
			//Debug.Log ("pos" + viewPos.ToString() + " name: "+ c.gameObject.name);
			if (viewPos.x > 0 && viewPos.x < 1 && viewPos.y >0 && viewPos.y <1 && viewPos.z >0)
			{
				spawner_in_view_ = c;
				return;
			}

		}

	}

	public void rightHandActions(){
		GameObject hand = GameObject.Find ("CleanRobotRightHand(Clone)");
		//hand is in the current frame
		if (hand != null){
			SkeletalHand skeleton = hand.GetComponent <SkeletalHand>();
			
			//look for the gestures that exist
			foreach (Gesture g in skeleton.GetLeapHand().Frame.Gestures()){
				
				if (g.Type == Gesture.GestureType.TYPE_CIRCLE && g.Hands.Count == 1 && g.Hands.Frontmost.IsRight){
					
					
					spawner_in_view_.freezeAllTraffic();
					action_timer = action_time;
				}
				else if (g.Type == Gesture.GestureType.TYPESWIPE && g.Hands.Count == 1 && g.Hands.Frontmost.IsRight){
					spawner_in_view_.resumeTraffic();
					action_timer = action_time;
					
				}
				
			}
		}
	}

	public void leftHandActions(){
		GameObject hand = GameObject.Find ("CleanRobotLeftHand(Clone)");

		if (hand != null) {
			Debug.Log("ing here");
			SkeletalHand skeleton = hand.GetComponent <SkeletalHand>();
			foreach (Gesture g in skeleton.GetLeapHand().Frame.Gestures()){
				if (g.Type == Gesture.GestureType.TYPE_CIRCLE && g.Hands.Count == 1 && g.Hands.Frontmost.IsLeft){
					if(rotate_timer_<0){
						rotate90(1);
					}
				}
			}
		}
	}

}
