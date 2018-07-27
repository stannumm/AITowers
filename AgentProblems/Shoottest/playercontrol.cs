using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class playercontrol : MonoBehaviour {


	public float speed;
	public float rotationspeed;

	public Animator animator;
	public float translationVertical;
	public float translationHorizontal;
	public float rotation;
	bool running = false;
	bool punching = false;
	public bool dying = false;
	GameObject[] objects;
	int i = 0;

	void Start()
	{
		animator = GetComponent<Animator>();
	
	}
	// Update is called once per frame
	void Update () {
		/*objects = GameObject.FindGameObjectsWithTag("turret").OrderBy( go => go.name ).ToArray();

		if(objects[i] == null){
			i++;
		}

		if (i > 3)
			i = 0;

		transform.position = Vector3.MoveTowards(transform.position, objects[i].transform.position, 0.03f);*/
	
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("death")) {

			if (Input.GetKeyDown (KeyCode.F1)) {
				animator.SetTrigger ("restart");
				dying = false;
				transform.position = new Vector3 (0, 0, 0);
				GameObject[] a =  GameObject.FindGameObjectsWithTag ("turret");
				GameObject[] b = GameObject.FindGameObjectsWithTag ("bullet");
				a[0].GetComponentInParent <NEATGeneticControllerV2>().TestFinished ();
				foreach(GameObject t in a)
					Destroy(t);
				foreach(GameObject t in b)
					Destroy(t);
				a [0].GetComponentInParent <NEATGeneticControllerV2> ().numberOfGenerationsToRun++;

			}


		}
		else{
			translationHorizontal = Input.GetAxis ("Horizontal") * speed * Time.deltaTime;
			translationVertical = Input.GetAxis ("Vertical") * speed * Time.deltaTime;
			rotation = Input.GetAxis ("Mouse X") * rotationspeed * Time.deltaTime;




			transform.Rotate (0, rotation, 0);

			if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("punch")) {
				transform.Translate (0, 0, translationVertical);
				transform.Translate (translationHorizontal, 0, 0);
			}

			if (translationVertical > 0)
				running = true;

			animator.SetBool ("running", running);
			running = false;

			if (Input.GetKeyDown (KeyCode.Mouse0)) {
				punching = true;

			}
			animator.SetBool ("punching", punching);
			punching = false;

			//animator.SetBool ("dying",dying);
		}

	}


}
