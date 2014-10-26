using UnityEngine;
using System.Collections;

public class Destination : MonoBehaviour {

	
	Vector3 position;
	
	// Use this for initialization
	void Start () {
		position = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//void OnTriggerEnter(Collider col) 
	//{
	//	if(col.gameObject.GetComponent<Car>() != null)
	//	{
	//		this.setVisited(true);
	//	}
	//}

	public Vector3 getPosition()
	{
		return position;
	}
	

}
