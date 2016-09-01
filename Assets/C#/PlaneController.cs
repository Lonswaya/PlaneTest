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


	private float throttle = 0;
	private float vertAxis = 0;
	private float horizAxis = 0;
	private bool boosting = false;
	private bool[] firing = new bool[0];

	public void Start() {
		//myWep = this.GetComponent<WeaponController> ();
		myRigid = transform.GetComponent<Rigidbody>();

	}
		



	public void FixedUpdate()
	{
		//print("updating");
		ProcessInputs ();

		if (!dead) {
			//FixInverted ();
			ProcessFlight ();
			ActivateWeapons ();
		}


	}

	public void ProcessInputs() {
		//are we AI or something else
		switch (InputMethod) {
		case 0:
			vertAxis = Input.GetAxis ("Vertical") + Input.GetAxis("Mouse Y");
			horizAxis = Input.GetAxis ("Horizontal") + Input.GetAxis("Mouse X");
			//brakeForce = Input.GetAxis ("Brakes");
			//driftAxis = Input.GetAxis ("Drift");
			boosting = ((Input.GetAxis ("Boost") + Input.GetAxis ("Jump")) > 0);
			float val = (Input.GetAxis("Throttle"));
			throttle += val * Time.deltaTime * .5f;
			if (throttle < 0) throttle = 0;
			if (throttle > 1) throttle = 1;
			/*firing = new bool[myWep.weapons.Length];
			int length = myWep.weapons.Length;
			for (int index = 0; index < length; index++) {
				firing[index] = (Input.GetAxis ("Fire" + (index + 1)) > 0);
			}*/

			break;
		case 1:
			vertAxis = 0;
			horizAxis = 0;
			//driftAxis = 0;
			boosting = false;
			firing = new bool[0];
			break;
		}
	}
	public void ActivateWeapons() {
		//myWep.ApplyInput (firing);
	}



	public void ProcessFlight() {







		float vertMovement = speed * vertAxis;
		float horizMovement = speed * horizAxis;


		AirMovement(vertMovement, horizMovement, throttle);

		timeSinceBoost += Time.deltaTime;
		if (boosting && timeSinceBoost > timeToBoost) { 
			myRigid.AddRelativeForce (Vector3.up * 500 * myRigid.mass);
			myRigid.AddRelativeForce(Vector3.forward * 2000 * myRigid.mass);
			timeSinceBoost = 0;
			booster.Play ();
		}




	}
	public void AirMovement(float forwardRotation, float sideRotation, float throttle) {
		//print ("airborne");
		myRigid.AddRelativeForce ((Vector3.up * 10 * myRigid.velocity.magnitude +  Vector3.forward * 1300 * throttle) * myRigid.mass * Time.deltaTime);

		//print(myRigid.velocity.magnitude);

	

		if (myRigid.velocity.magnitude > 30) {
			forwardRotation = forwardRotation / ((myRigid.velocity.magnitude-26)*.25f);
			sideRotation = sideRotation / ((myRigid.velocity.magnitude-20)*.10f);
		} else if (myRigid.velocity.magnitude < 30) {
			forwardRotation = forwardRotation * myRigid.velocity.magnitude/30;
			sideRotation = sideRotation * myRigid.velocity.magnitude/30;

		} else {
			//forward rotation stays the same
		}
		if (Mathf.Abs(myRigid.angularVelocity.y) > .3f) {
			forwardRotation = forwardRotation/(Mathf.Abs(myRigid.angularVelocity.y)+.7f);
			//forwardRotation = (myRigid.angularVelocity.y>0)?(-.3f):(.3f);
		}

		myRigid.AddRelativeTorque (new Vector3 (forwardRotation * Time.deltaTime * 200, 0, sideRotation * Time.deltaTime * -250));
		//print (myRigid.rotation);
	}
	void Death() {
		dead = true;
	}
}