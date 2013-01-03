
public var hamsterBall : Transform;
private var hamsterRigid : Rigidbody;
public var playerCamera : Transform;
public var throwStrength : float = 1.0f;

enum PickupState
{
	CheckingForPickup,
	PickingUp,
	CheckingForDrop,
	Dropping
};

private var motor : CharacterMotor;
private var state : PickupState;

function Update ()
{
	switch (state)
	{
	case PickupState.CheckingForPickup:
		CheckForPickup();
		break;
	case PickupState.PickingUp:
		state = PickupState.CheckingForDrop;
		break;
	case PickupState.CheckingForDrop:
		CheckForDrop();
		break;
	case PickupState.Dropping:
		state = PickupState.CheckingForPickup;
		break;
	}
}

function CheckForPickup ()
{
	var dist : Vector3 = transform.position;
	dist -= hamsterBall.position;

	if (dist.sqrMagnitude < 4.0f)
	{
		if (motor.inputFireDB)
		{
			//Debug.Log("Pickup!");
			hamsterRigid.isKinematic = true;
			state = PickupState.PickingUp;
		}
	}
}

function CheckForDrop ()
{
	if (motor.inputFireDB)
	{
		//Debug.Log("Drop!");
		hamsterRigid.isKinematic = false;
		if (playerCamera.forward.y > 0.0f)
			hamsterRigid.AddForce( playerCamera.forward * throwStrength );
		else
			hamsterRigid.AddForce( playerCamera.forward );
		state = PickupState.Dropping;
	}

	hamsterBall.rotation = playerCamera.rotation;
	hamsterBall.position = playerCamera.position + playerCamera.forward * 0.6f + playerCamera.up * -0.5F;
}



function Awake ()
{
	hamsterRigid = hamsterBall.GetComponent(Rigidbody);
	motor = GetComponent(CharacterMotor);
	state = PickupState.CheckingForPickup;
}
