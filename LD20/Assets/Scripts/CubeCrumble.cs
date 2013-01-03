using UnityEngine;
using System.Collections;

public class CubeCrumble : MonoBehaviour
{
	public Object templateDustPtl;
	public AudioClip sfxCrack;
	public AudioClip sfxCrumble;

	private TowerGenerator towerMap;

	private float timer = -1.0f;

	// Use this for initialization
	void Start ()
	{
		GameObject towerObj = GameObject.Find("TowerRoot");
		towerMap = towerObj.GetComponent<TowerGenerator>();
	}

	// Update is called once per frame
	void Update ()
	{
		if (timer >= 0.0f)
			timer += Time.deltaTime;

		if (timer > 0.6f)
		{
			//Debug.Log("Collapse!");
			audio.PlayOneShot( sfxCrumble );
			GameObject clone = (GameObject)Instantiate( templateDustPtl, transform.position, Quaternion.identity );
			Rigidbody rig = GetComponent<Rigidbody>();
			rig.isKinematic = false;	// Release the block!
			rig.WakeUp();
			Vector3 force = transform.position - towerMap.GetCentrePos();
			force.y = 0.0f;
			force.Normalize();
			rig.AddForce( force * 300.0f );
			timer = -2.0f;				// Prevent it triggering again
		}
	}

	void OnStandOn()
	{
		if (timer >= -1.0f)
		{
        	//Debug.Log("Crumbling!");
        	audio.PlayOneShot( sfxCrack );
			timer = 0.0f;
		}
    }
}
