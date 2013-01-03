using UnityEngine;
using System.Collections;

public class MoveNavi : MonoBehaviour
{
	public Transform followTarget;
	public Transform towerCentre;
	public Transform lamp;
	public float damping = 0.8f;
	public GameGUI gui;

	public AudioClip dialogue01;
	public AudioClip dialogue02;
	public AudioClip dialogue03;
	public AudioClip dialogue04;
	public AudioClip dialogue05;
	public AudioClip dialogue06;
	public AudioClip dialogue07;
	public AudioClip dialogue08;
	public AudioClip dialogue09;
	public AudioClip dialogue10;
	public AudioClip dialogue11;
	public AudioClip dialogue12;
	public AudioClip dialogue13;
	public AudioClip dialogue14;
	public AudioClip dialogue15;
	public AudioClip dialogue16;
	public AudioClip dialogue17;
	public AudioClip dialogue18;
	public AudioClip dialogue19;
	public AudioClip dialogue20;
	public AudioClip dialogue21;
	public AudioClip dialogue22;
	public AudioClip dialogue23;

	private TowerGenerator towerMap;
	private Vector3 target_pos;
	private Vector3 velocity;
	private Vector3 dist_to_tower;
	private float timeToStaySilent = 0.0f;
	private float timeToLinger = 25.0f;
	private float timeSinceSpoke = 0.0f;
	private bool[] spokenLine = new bool[] {false,false,false,false,false,
											false,false,false,false,false,
											false,false,false,false,false,
											false,false,false,false,false,
											false,false,false,false,false };


	// Use this for initialization
	void Start ()
	{
		velocity = Vector3.zero;
		towerMap = towerCentre.GetComponent<TowerGenerator>();
	}

	// Update is called once per frame
	void Update ()
	{
		dist_to_tower = followTarget.position - towerMap.GetCentrePos();
		dist_to_tower.y = 0.0f;

		if (gui.IsActive())
		{
			target_pos = lamp.position;
			target_pos.y -= 1.5f;
		}
		else if (timeToLinger > 0.0f)
		{
			// Linger near lamp for first few seconds
			timeToLinger -= Time.deltaTime;
			target_pos = lamp.position;
			target_pos.y += 1.5f;
		}
		else if (dist_to_tower.sqrMagnitude < 25.0f * 25.0f)
		{
			// Stay near player but away from tower
			target_pos = followTarget.position;
			Vector3 away = dist_to_tower;
			away.y = 0.0f;
			away.Normalize();
			target_pos += away * 3.0f;
			target_pos.y += 1.5f;
		}
		else
		{
			// Get in front of player
			target_pos = followTarget.position;
			target_pos += followTarget.forward * 3.0f;
			target_pos += followTarget.right * 2.0f;
			target_pos.y += 1.5f;
		}

		Vector3 force = target_pos - transform.position;
		velocity += force * Time.deltaTime * 2.0f;
		velocity *= 1.0f - (damping * Time.deltaTime);

		transform.position += velocity * Time.deltaTime;

		Vector3 rot = new Vector3( 250.0f*Time.deltaTime, 0.0f, 64.0f * Time.deltaTime );
		transform.Rotate( rot );

		UpdateDialogue();
	}

	void UpdateDialogue()
	{
		if (gui.IsActive())
		{
			timeToStaySilent = 1.0f;
			return;
		}

		if (timeToStaySilent > 0.0f)
		{
			timeSinceSpoke = 0.0f;
			timeToStaySilent -= Time.deltaTime;
			return;
		}
		
		timeSinceSpoke += Time.deltaTime;

		if (!spokenLine[1])
		{
			spokenLine[1] = true;
			audio.PlayOneShot( dialogue01 );
			timeToStaySilent = 28.0f;
			return;
		}
		if (!spokenLine[2])
		{
			spokenLine[2] = true;
			audio.PlayOneShot( dialogue02 );
			timeToStaySilent = 4.0f;
			return;
		}
		if ((!spokenLine[4]) && followTarget.position.y > 2.5f)
		{
			spokenLine[4] = true;
			audio.PlayOneShot( dialogue04 );
			timeToStaySilent = 6.0f;
			return;
		}
		if ((!spokenLine[5]) && followTarget.position.y > 10.0f)
		{
			spokenLine[5] = true;
			audio.PlayOneShot( dialogue05 );
			timeToStaySilent = 4.0f;
			return;
		}
		if ((!spokenLine[6]) && followTarget.position.y > 15.0f)
		{
			spokenLine[6] = true;
			audio.PlayOneShot( dialogue06 );
			timeToStaySilent = 4.0f;
			return;
		}
		if ((!spokenLine[8]) && followTarget.position.y > 2.5f * 9.0f)
		{
			spokenLine[8] = true;
			audio.PlayOneShot( dialogue08 );
			timeToStaySilent = 13.0f;
			return;
		}
		if ((!spokenLine[22]) && dist_to_tower.sqrMagnitude < 3.0f * 3.0f)	// Reached centre of tower (the triforce)
		{
			spokenLine[22] = true;
			audio.PlayOneShot( dialogue22 );
			timeToStaySilent = 6.0f;
			// Hide Tridforce
			return;
		}
		if ((!spokenLine[16]) && dist_to_tower.sqrMagnitude > 150.0f * 150.0f)
		{
			spokenLine[16] = true;
			audio.PlayOneShot( dialogue16 );
			timeToStaySilent = 16.0f;
			return;
		}

		//
		// Say random things after quiet period
		//
		if (timeSinceSpoke > 6.0f)
		{
			timeSinceSpoke = 0.0f;

			if (!spokenLine[23])
			{
				spokenLine[23] = true;
				audio.PlayOneShot( dialogue23 );
				timeToStaySilent = 5.0f;
				return;
			}

			if (followTarget.position.y > 2.5f * 9.0f)	// Height-related comments
			{
				if (!spokenLine[20])
				{
					spokenLine[20] = true;
					audio.PlayOneShot( dialogue20 );
					timeToStaySilent = 4.0f;
					return;
				}
				if (!spokenLine[11])
				{
					spokenLine[11] = true;
					audio.PlayOneShot( dialogue11 );
					timeToStaySilent = 5.0f;
					return;
				}
				if (!spokenLine[13])
				{
					spokenLine[13] = true;
					audio.PlayOneShot( dialogue13 );
					timeToStaySilent = 4.0f;
					return;
				}
				if (!spokenLine[14])
				{
					spokenLine[14] = true;
					audio.PlayOneShot( dialogue14 );
					timeToStaySilent = 4.0f;
					return;
				}
			}
			if (!spokenLine[17])
			{
				spokenLine[17] = true;
				audio.PlayOneShot( dialogue17 );
				timeToStaySilent = 4.0f;
				return;
			}
			if (!spokenLine[18])
			{
				spokenLine[18] = true;
				audio.PlayOneShot( dialogue18 );
				timeToStaySilent = 5.0f;
				return;
			}
			if (!spokenLine[19])
			{
				spokenLine[19] = true;
				audio.PlayOneShot( dialogue19 );
				timeToStaySilent = 2.0f;
				return;
			}
			if (!spokenLine[10])
			{
				spokenLine[10] = true;
				audio.PlayOneShot( dialogue10 );
				timeToStaySilent = 9.0f;
				return;
			}
			if (!spokenLine[15])
			{
				spokenLine[15] = true;
				audio.PlayOneShot( dialogue15 );
				timeToStaySilent = 10.0f;
				return;
			}

		}
	}
}
