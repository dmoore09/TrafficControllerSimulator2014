using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Car : MonoBehaviour {

	public GameObject cop_;
	public GameRoot root_;
	private CarSpawner car_spawner_;

	List<Destination> destinations = new List<Destination>();
	List<bool> haveVisited = new List<bool> ();

	private float stop_time_ = 10.0f;
	private float stop_timer_;
	private bool is_car_dead_;
	private Destination current_goal_;

	private float death_time_ = 6.0f;
	private float death_timer_;


	//stuff for moving
	public float car_speed_ = 4.0f; 
	private float speed = 1.0f;
	private float startTime;
	private float journeyLength;
	private Vector3 start_position_;
	private int points_visited_;
	private float time_stopped_;

	AudioSource audio_source_;
	

	// Use this for initialization
	void Start () 
	{
		Physics.IgnoreLayerCollision (this.gameObject.layer, 8, true);
		stop_timer_ = 0;
		time_stopped_ = 0;
		death_timer_ = 0;
		points_visited_ = 0;
		is_car_dead_ = false;
		//needs to be singleton get
		root_ = GameObject.Find ("GameRoot").GetComponent<GameRoot>();
		//start_position_ = this.transform.position;
		audio_source_ = this.gameObject.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {

		//Debug.Log (start_position_.ToString ());
		stop_timer_ -= Time.deltaTime;
		//Debug.Log ("timer: " + stop_timer_);

		//this is stupid brian
		if(death_timer_>0)
		{
			death_timer_ -=Time.deltaTime;
			if(death_timer_ < 1.0f)
			{
				resetCar();
			}
		}

		else if(stop_timer_ > 0)
		{
			time_stopped_ += Time.deltaTime;
			//stop
			//Debug.Log ("not moving?");
		}
		else
		{
			//init
			if(current_goal_ == null)
				getNextWaypoint();
			if(!is_car_dead_)
				drive();
		}
		
	
	}

	//move to next waypoint
	public void drive()
	{
		//Debug.Log (" start: " + start_position_.ToString()+" goal: " + current_goal_.getPosition ().ToString ());
		float distCovered = (Time.time - startTime - time_stopped_) * speed;
		float fracJourney = distCovered / journeyLength;
		this.transform.position = Vector3.Lerp (start_position_, current_goal_.getPosition(), fracJourney * car_speed_);
	}

	public void getNextWaypoint ()
	{
		for(int i=0;i<destinations.Count;i++)
		{
			Destination d = destinations[i];
			if(!haveVisited[i])
			{
				startTime = Time.time;
				journeyLength = Vector3.Distance(start_position_, d.getPosition());
				current_goal_ = d;
				return;
			}
		}
		//
		//found everything
		root_.notifyCarSaved();;
		resetCar();
	}

	public void setSpawner(CarSpawner cs)
	{
		car_spawner_ = cs;
	}

	public void setStartPosition(Vector3 position)
	{
		start_position_ = position;
	}

	//only init
	public void setPath(List<Destination> course )
	{
		//give car a path
		for(int i =0;i<course.Count;i++)
		{
			//Debug.Log("i: " + i + "  " +course[i].getPosition().ToString());
			destinations.Add(course[i]);
			haveVisited.Add(false);
		}
	}

	void OnTriggerEnter(Collider col)
	{
		//Debug.Log (col.gameObject.name);
		if(col.gameObject.GetComponent<Destination>() != null)
		{
			Destination d  = col.gameObject.GetComponent<Destination>();
			//new start to move to next waypoint
			haveVisited[points_visited_] = true;
			points_visited_++;
			time_stopped_ = 0;
			start_position_ = d.getPosition();
			//go to next waypoint
			getNextWaypoint();
		}
	}
	void OnCollisionEnter (Collision col)
	{
		//Debug.Log (col.gameObject.name);
		//should just use collision layers
		if(col.gameObject.GetComponent<Car>() != null && !is_car_dead_)
		{
			//make car fall
			this.setCarDead();

			GameObject c = (GameObject)Instantiate(Resources.Load("Detonator-Base"));
			Detonator explosion = c.GetComponent<Detonator>();
			explosion.Explode();

			audio_source_.PlayOneShot(audio_source_.clip);

			//Vector3 explosivePosition = new Vector3(this.transform.position.x,this.transform.position.y +.5f,this.transform.position.z);
			//this.gameObject.rigidbody.AddExplosionForce(500,explosivePosition,1.0f);
		}
		//else if (col)
	}

	public void setCarDead()
	{
		startTime = 0;
		journeyLength = 0;
		//Debug.Log ("dead");
		death_timer_ = death_time_;
		this.gameObject.rigidbody.useGravity = true;
		is_car_dead_ = true;
		root_.notifyCarLost ();
	}

	//reset car to be reused 
	public void resetCar()
	{
		//Debug.Log ("reset");
		for (int i =0; i<haveVisited.Count; i++) 
		{
			haveVisited [i] = false;
		}
		points_visited_ = 0;
		death_timer_ = 0;
		//this.gameObject.rigidbody.isKinematic = false;
		this.gameObject.rigidbody.useGravity = false;
		is_car_dead_ = false;
		current_goal_ = null;
		//disappear to be reclaimed later
		this.gameObject.SetActive(false);

	}

	//called by cop
	public void freeze()
	{
		car_spawner_.freezeAllTraffic();
	}
	public void setStop()
	{
		//Debug.Log ("stop" + stop_time_);

		stop_timer_ = stop_time_;
	}

	//called by cop
	public void resume()
	{
		car_spawner_.resumeTraffic();
	}
	public void setGo()
	{
		//Debug.Log ("go");
		stop_timer_ = 0;
	}
}

