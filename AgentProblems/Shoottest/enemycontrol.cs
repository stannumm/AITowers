using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemycontrol : MonoBehaviour
{
	public GameObject ExplodeFX;
	public GameObject FireFX;
	public GameObject ProjectilePrefab;
	public bool shot = false;
	float shootTime = 1;
	GameObject a;
	float delta;
	public bool punched = false;
	public Transform bullettra;
	/// <summary>
	/// Event subscriptions to notify controller when test is finished
	/// </summary>
	/// <param name="source">Source of the event (this)</param>
	/// <param name="args">Nothing</param>
	public delegate void TestFinishedEventHandler(object source, EventArgs args);
	public event TestFinishedEventHandler TestFinished;

	private bool isActive = false; // is this agent active
	private bool finished = false; // is this agent finished.  Making sure only 1 event is sent.

	private NEATNet net; //The brain

	private const string ACTION_ON_FINISHED = "OnFinished"; //On finished method

	private NEATGeneticControllerV2 controller; //Controller

	private Rigidbody rb;
	private GameObject player;
	float[] input = new float[3]; //input size
	GameObject[] turrets;

	/// <summary>
	/// Set Color to this agent. Looks visually pleasing and may help in debugging? 
	/// </summary>
	/// <param name="color"> color</param>
	public void SetColor(Color color)
	{
		Renderer rend = transform.GetComponent<Renderer>();
		rend.material.color = color;

	}

	/// <summary>
	/// Start up tasks for this agent game object.
	/// </summary>
	void Start()
	{
		turrets = GameObject.FindGameObjectsWithTag("turret");
		rb = GetComponent<Rigidbody>();
		player = GameObject.FindGameObjectWithTag("Player");
		// player = GameObject.Find("trainobject"); //for testing
	}

	public void IfShoots(float distance)
	{
		net.AddNetFitness((180 - Math.Abs(delta)) * 3);

	}
	/// <summary>
	/// Tick
	/// </summary>
	public void UpdateNet()
	{

		Shoot();
		if (shot)
		{
			Vector3 target = transform.position - bullettra.position;
			float delta = Vector3.SignedAngle(target, -transform.right, Vector3.up); 
			IfShoots(input[1]);
			/* GetComponentInParent<NEATGeneticControllerV2>().TestFinished();
             foreach(GameObject t in turrets)
                 Destroy(t);*/
		}
		else
			net.AddNetFitness(-1f);

		shot = false;
		//between 1,-1
		Vector3 targetdir = transform.position - player.transform.position;
		float deltavectorangle = Vector3.SignedAngle(targetdir, -transform.right, Vector3.up);
		input[0] = deltavectorangle / 180;
		input[1] = Vector3.Distance(player.transform.position, transform.position);
		input[2] = player.GetComponent<playercontrol>().translationVertical;


		net.AddNetFitness((float)(1 - Mathf.Abs(deltavectorangle / 180)));
		net.AddNetFitness((float)(1 - Mathf.Abs(delta / 180)));

		float[] output = net.FireNet(input);
		transform.Rotate(0, output[0] * 3.5f,0);
		//transform.Translate(0, 0, output[1] * Time.deltaTime * 20);
		transform.Translate(output[1] * -transform.right * Time.deltaTime * 20);

		if (input [1] < 6)
			net.AddNetFitness (-1f);

		if ((transform.position.x < -16 || transform.position.x > 16) || (transform.position.z > 14 || transform.position.z < -18))
		{
			net.AddNetFitness(-10f);
			OnFinished();
		}
	}
	private void OnCollisionEnter(Collision col)
	{

		if (col.collider.tag == "Player")
		{
				net.AddNetFitness (-100f);
				OnFinished ();
		}

	}

	public void Shoot()
	{
		Vector3 bulletStartPos = (-transform.right*2.5f + transform.position);
		bulletStartPos.y += 0.8f; //height of turret


		shootTime -= Time.deltaTime;
		if (shootTime < 0)
		{	
			Destroy(Instantiate(FireFX, bulletStartPos, Quaternion.Euler(0, 90, 0)),0.1f);

			a = Instantiate(ProjectilePrefab, bulletStartPos, Quaternion.Euler(0, 90, 0));
			a.GetComponent<Rigidbody>().AddForce(-transform.right * 400f);
			a.name = gameObject.name + " xx";
			shootTime = 0.1f;
		}

	}


	/// <summary>
	/// Some fail condition for this agent
	/// </summary>
	/// <returns></returns>
	public bool FailCheck()
	{
		return false;
	}

	/// <summary>
	/// Fitness update per tick. Does not have to happen here! But good practice.
	/// </summary>
	public void CalculateFitnessOnUpdate()
	{

	}

	/// <summary>
	/// Final fitness calculation once this agent is finished or failed
	/// </summary>
	public void CalculateFitnessOnFinish()
	{
		//  GetComponentInParent<NEATGeneticControllerV2>().TestFinished(); bu satır buradaki metodun çalışmasına sebep OLMUYOR
		//net.AddNetFitness(-100f); // when player touches the turret it gets negative fitness
	}

	/// <summary>
	/// No need to worry about this method! You just need to code in UpdateNet and CalculateFitnessOnUpdate :D
	/// </summary>
	void FixedUpdate()
	{
		if (isActive == true)
		{
			UpdateNet(); //update neural net
			CalculateFitnessOnUpdate(); //calculate fitness

			if (FailCheck() == true)
			{
				OnFinished();
			}
		}


	}



	/// <summary>
	/// OnFinished is called when we want to notify controller this agent is done. 
	/// Automatically handels notification.
	/// </summary>
	public void OnFinished()
	{
		if (TestFinished != null)
		{
			if (!finished)
			{
				finished = true;
				CalculateFitnessOnFinish();
				TestFinished(net.GetNetID(), EventArgs.Empty);
				TestFinished -= controller.OnFinished; //unsubscrive from the event notification
				Destroy (Instantiate (ExplodeFX,transform.position,Quaternion.identity),2);
				Destroy(gameObject); //destroy this gameobject
			}
		}
	}

	/// <summary>
	/// Activated the agent when controller give it a brain. 
	/// </summary>
	/// <param name="net">The brain</param>
	public void Activate(NEATNet net)
	{
		this.net = net;
		Invoke(ACTION_ON_FINISHED, net.GetTestTime());
		isActive = true;
	}

	/// <summary>
	/// Getting net. 
	/// This could be used by some other objects that have reference to this game object 
	/// and want to see the brain.
	/// </summary>
	/// <returns> The brain</returns>
	public NEATNet GetNet()
	{
		return net;
	}

	/// <summary>
	/// Adds controller and subscribes to an event listener in controller
	/// </summary>
	/// <param name="controller">Controller</param>
	public void SubscriveToEvent(NEATGeneticControllerV2 controller)
	{
		this.controller = controller;
		TestFinished += controller.OnFinished; //subscrive to an event notification
	}


	////turret and player collision
	//private void OnCollisionEnter(Collision col)
	//{
	//    if (col.collider.tag == "Player")
	//    { 
	//        OnFinished();

	//    }
	//}
}
