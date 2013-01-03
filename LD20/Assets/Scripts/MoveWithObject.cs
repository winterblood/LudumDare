using UnityEngine;
using System.Collections;

public class MoveWithObject : MonoBehaviour
{

	public Transform followObject;
	private Vector3 offset;

	// Use this for initialization
	void Start ()
	{
		offset = transform.position - followObject.position;
	}

	// Update is called once per frame
	void Update ()
	{
		transform.position = followObject.position + offset;
	}
}
