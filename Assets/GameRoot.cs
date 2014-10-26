using UnityEngine;
using System.Collections;

public class GameRoot : MonoBehaviour {

	public int dead_cars_;
	public int max_cars_can_lose_ = 10;
	public int cars_safe_;
	// Use this for initialization
	void Start () {

		dead_cars_ = 0;
		cars_safe_ = 0;
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (dead_cars_ > max_cars_can_lose_) 
		{
			//game overrrrr
			//Debug.Log ("YOU LOSE, you saved " + cars_safe_ + "from dying");
		}

	}

	public void notifyCarSaved()
	{
		cars_safe_++;
	}

	public void notifyCarLost()
	{
		dead_cars_++;
	}


}
