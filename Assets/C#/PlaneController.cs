using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlaneController : MonoBehaviour {

	public float speed;

	public float timeToBoost;
	public ParticleSystem booster;
	public int InputMethod = 0; //-1 for immobile, 0 for player, 1 for AI

	private float timeSinceBoost;
	private Rigidbody myRigid;
	private WeaponController myWep;
	private float timeGrounded, timeUpsideDown;
	private bool lastDrifting;
	private bool dead;



	private float vertAxis = 0;
	private float horizAxis = 0;
	private float throttleUp = 0;
	private float throttleDown = 0;
	private bool boosting = false;
	private bool[] firing = new bool[0];

	public void Start() {
		myWep = this.GetComponent<WeaponController> ();
		myRigid = transform.GetComponent<Rigidbody>();

	}
		



	public void FixedUpdate()
	{
		ProcessInputs ();

		if (!dead) {
			FixInverted ();
			ProcessFlight ();
			ActivateWeapons ();
		}


	}

	public void ProcessInputs() {
		//are we AI or something else
		switch (InputMethod) {
		case 0:
			vertAxis = Input.GetAxis ("Vertical");
			horizAxis = Input.GetAxis ("Horizontal");
			brakeForce = Input.GetAxis ("Brakes");
			driftAxis = Input.GetAxis ("Drift");
			boosting = (Input.GetAxis ("Boost") > 0);
			firing = new bool[myWep.weapons.Length];
			int length = myWep.weapons.Length;
			for (int index = 0; index < length; index++) {
				firing[index] = (Input.GetAxis ("Fire" + (index + 1)) > 0);
			}

			break;
		case 1:
			vertAxis = 0;
			horizAxis = 0;
			brakeForce = 0;
			driftAxis = 0;
			boosting = false;
			firing = new bool[0];
			break;
		}
	}
	public void ActivateWeapons() {
		myWep.ApplyInput (firing);
	}

	public void AirMovement(float forwardRotation, float sideRotation) {
		//print ("airborne");
		myRigid.AddRelativeTorque (new Vector3 (forwardRotation * Time.deltaTime * 1000000, sideRotation * Time.deltaTime * 1000000, 0));
		//print (myRigid.rotation);
	}

	public void ProcessFlight() {







		float vertMovement = speed * vertAxis;
		float horizMovement = speed * horizAxis;
		//todo change direction

		bool airborne = false;


		timeSinceBoost += Time.deltaTime;
		if (boosting && timeSinceBoost > timeToBoost) { 
			myRigid.AddRelativeForce (Vector3.up * 500 * myRigid.mass);
			myRigid.AddRelativeForce(Vector3.forward * 2000 * myRigid.mass);
			timeSinceBoost = 0;
			booster.Play ();
		}




	}
	void Death() {
		dead = true;
	}
}