﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
	[SerializeField] AudioClip shootSound;
	[SerializeField] AnimationClip animationDestroyed;
	[SerializeField] Animator DroneAnimator;

	[Header("Sounds")]
	[SerializeField] AudioClip SoundDisable;
	[SerializeField] AudioSource droneAudioSource;

	public Transform target;
	public GameObject projectile;
	public float projectileSpawnOffset;
	public float projectileStrength;

	[SerializeField] GameObject drone;

	[Range(2,5)]
	public float shootInterval;

	public int moveSpeed;
	public float hoverHeight;
	public float hoverStrength;
	public double maxDistance;
	public double minDistance;

	private float YPos;

	private Rigidbody rb;
	private GameObject bullet;
	private bool fire = true;

	void Start()
	{
		droneAudioSource = drone.GetComponent<AudioSource>();
		rb = GetComponent<Rigidbody> ();
	}

	void Awake()
	{
		YPos = transform.position.y;
	}

	void Update()
	{
		if (target == null)
		{
			if (GameObject.FindGameObjectWithTag ("Player"))
			{
				Transform player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ();
				target = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ();
			}

		}
		else
		{
			transform.LookAt (target);

			if (fire == true)
			{
				StartCoroutine (shoot ());
			}
		}
	}

	void FixedUpdate()
	{
		if (transform.position.y < (YPos + hoverHeight))
		{
			rb.AddForce (Vector3.up * hoverStrength);
		}
		else if (transform.position.y > (YPos + hoverHeight))
		{
			rb.AddForce (Vector3.down * (hoverStrength * 0.75f));
		}

		if (target != null)
		{
			if (Vector3.Distance(transform.position, target.position) >= minDistance)
			{
				rb.AddRelativeForce(Vector3.forward * moveSpeed);
			}
			else if (Vector3.Distance(transform.position, target.position) <= minDistance)
			{
				rb.AddRelativeForce (Vector3.back * (moveSpeed / 0.75f));
			}
		}
	}

	IEnumerator shoot()
	{
		Transform bulletSpawn = transform.Find ("bulletSpawn");
		AudioSource sfx_shoot = bulletSpawn.GetComponent<AudioSource>();

		sfx_shoot.PlayOneShot(shootSound);
		bullet = Instantiate (projectile, bulletSpawn.transform.position, bulletSpawn.transform.rotation);

		//shoots at random interval
		fire = false;
		shootInterval = Random.Range(2,5);
		yield return new WaitForSeconds (shootInterval);
		fire = true;
	}

	public IEnumerator destroyed(){

		AudioSource mainAudioSource = gameObject.GetComponent<AudioSource>();


		//disable engine sound
		mainAudioSource.enabled = false;
		
		//play disable sound effects
		droneAudioSource.PlayOneShot(SoundDisable);

		DroneAnimator.SetBool("Alive",false);
		yield return new WaitForSeconds(animationDestroyed.length);

	}
}
