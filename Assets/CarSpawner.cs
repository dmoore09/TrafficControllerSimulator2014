using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarSpawner : MonoBehaviour {
	
	List<Destination> destinations = new List<Destination>();

	List<Car> cars = new List<Car>();

	private float emit_time_  = 4.0f;
	private float emit_timer_;
	private int car_count_ = 30;
	private bool frozen_;
	Quaternion rotated_direction_;
	Vector3 direction;
	// Use this for initialization
	void Start () 
	{
		//Add waypoints to destinations,  all children are waypoints
		for(int i=0;i<transform.childCount;i++)
		{
			if(i==0)
			{
				direction = transform.GetChild(i).position - this.transform.position;
				rotated_direction_ = Quaternion.LookRotation(direction);
			}
			destinations.Add(transform.GetChild(i).gameObject.GetComponent<Destination>());
		}

		//destinations.Add (GameObject.Find (this.name+"/waypoint").GetComponent<Destination>());

		//add cars
		for(int i=0;i<car_count_;i++)
		{
			GameObject c  = (GameObject)Instantiate(Resources.Load("Car"));
			Car car = c.GetComponent<Car>();
			car.setPath(destinations);
			car.setSpawner(this);
			c.gameObject.SetActive(false);
			cars.Add (car);
		}

		emit_timer_ = emit_time_;
		frozen_ = false;
	
	}
	
	// Update is called once per frame
	void Update () 
	{ 
		if (Input.GetKeyDown ("w"))
			resumeTraffic ();
		if (Input.GetKeyDown ("s"))
			freezeAllTraffic ();

		if(!frozen_)
			emit_timer_ -= Time.deltaTime;
		if (emit_timer_ <= 0) 
		{
			//Debug.Log ("emit");
			emitCar();
			emit_timer_ = emit_time_;
		}
	
	}

	public void freezeAllTraffic()
	{
		frozen_ = true;
		for (int i=0; i<cars.Count; i++) 
		{
			Car c = cars[i];
			if(c.gameObject.activeSelf)
			{
				c.setStop();
			}
		}
	}

	public void resumeTraffic()
	{
		frozen_ = false;
		for (int i=0; i<cars.Count; i++) 
		{
			Car c = cars[i];
			if(c.gameObject.activeSelf)
			{
				c.setGo();
			}
		}
	}

	void emitCar()
	{
		for(int i=0;i<cars.Count;i++)
		{
			Car c = cars[i];
			//send car if not active
			if(!c.gameObject.activeSelf)
			{
				c.transform.position = this.transform.position;
				c.transform.rotation = rotated_direction_;
				c.gameObject.rigidbody.velocity = Vector3.zero;
				c.gameObject.rigidbody.angularVelocity = Vector3.zero;
				c.setStartPosition(this.transform.position); 
				c.gameObject.SetActive(true);
				//one car at a time
				return;
			}
		}
	}
}
