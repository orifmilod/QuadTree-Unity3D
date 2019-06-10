using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceShipController : MonoBehaviour 
{
	
	[Header("Space Ship Settings")]
	[SerializeField] int speed;
	[SerializeField] int speedRotation;

	[Space(5)]
	[Header("Bullet Settings")]
	[SerializeField] float bulletLifeTime;
	[SerializeField] float bulletSpeed;
	[SerializeField] float fireTime;

	List<Bullet> bulletsData;
	List<GameObject> bullets;

	public Vector2 position;
	PlayerState playerState;
	public bool isMoving = false;
	float bulletRadius = .5f;
	float spaceShipRadius = 1;
	float nextFire;

	void Start() 
	{
		bullets = new List<GameObject>();
		bulletsData = new List<Bullet>();
	}

	void Update() 
	{
		if(playerState == PlayerState.Alive)
		{
			SpaceShipMovement();
			ShootBullet();
			UpdateBulletsPosition();
		}
	}

	void SpaceShipMovement() 
	{
		if(Input.GetAxis("Horizontal") != 0) 
		{
			float z = transform.rotation.eulerAngles.z - Input.GetAxis("Horizontal") * speedRotation * Time.deltaTime;
			transform.rotation = Quaternion.Euler(0, 0, z);
		}
		if(Input.GetAxis("Vertical") != 0) 
		{	
			isMoving = true;
			transform.position += transform.rotation * new Vector3(0, Input.GetAxis("Vertical"), 0) * speed * Time.deltaTime;
			position = transform.position;
		}
		else if(isMoving)
		{
			isMoving = false;
		}

		bool asteroidCollided = GameController.Instance.QuadTreeCollision(position.x, position.y, spaceShipRadius);
		if(asteroidCollided) 
		{
			playerState = PlayerState.Dead;
			ResetSpaceShip();
			GameController.Instance.GameOver();
		}
	}
	void ShootBullet () 
	{
		//calculating next fire
		nextFire += Time.deltaTime;
		if(nextFire >= fireTime) 
		{
			nextFire = 0;

			SpawnFromPool();

			Bullet newBulletData = new Bullet(new Point(transform.position.x, transform.position.y, bulletRadius));
			bulletsData.Add(newBulletData);
		}
	}

	void SpawnFromPool()
	{
		GameObject obj = ObjectsPool.Instance.SpawnObject(
			PoolObjTypes.bullets, 
			transform.position, 
			transform.rotation
		);
		bullets.Add(obj);
	}
	void UpdateBulletsPosition()
	{
		for (int i = 0; i < bulletsData.Count; i++) 
		{
			//Check for the lifetime 
			if(bulletsData[i].timePassed >= bulletLifeTime)
			{
				RemoveBullet(i);
			}
			else
			{
				bulletsData[i].timePassed += Time.deltaTime;
				bullets[i].transform.position += bullets[i].transform.rotation * new Vector3(0, 1, 0) * bulletSpeed * Time.deltaTime;
				
				//Updating the bullets data
				bulletsData[i].position.x = bullets[i].transform.position.x;
				bulletsData[i].position.y = bullets[i].transform.position.y;

				bool collided = GameController.Instance.QuadTreeCollision(bulletsData[i].position.x, bulletsData[i].position.y, bulletRadius);
				if(collided) 
				{
					RemoveBullet(i);
					GameController.Instance.AddPoints(1);
				}	
			}
		}
	}
	void RemoveBullet(int index)
	{
		bulletsData[index].timePassed = 0;
		ObjectsPool.Instance.ReturnToPool(PoolObjTypes.bullets, bullets[index].gameObject);

		bulletsData.RemoveAt(index);
		bullets.RemoveAt(index);
	}
	IEnumerator asd()
	{
		yield return new WaitForFixedUpdate();
		
	}
	public void ResetSpaceShip() 
	{
		transform.position = Vector3.zero;
		playerState = PlayerState.Alive;
	}
}