using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

class GameController : Singleton<GameController> {
    public event Action StartGameTriggered, GameOverTriggered;
	Asteroid[] asteroidsData;
	Dictionary<int, GameObject> visibleAsteroids;

	[Space(5)]
    [Header("Asteroid Parametrs")]
	[SerializeField] int asteroidSpeed;
	int respawnDelayTime = 1;
	int asteroidRadius = 1;
	int asteroidCount = 0;
	bool allAsteroidsSpawned = false;
	

	[Header("QuadTree Parameters")]
	[Space(5)]
	QuadTree quadTree;
	[SerializeField] Grid grid;
	[SerializeField] int updateQTAfterFrame;
	short framesPassed = 0;
	bool updateQT = true;

	[SerializeField] SpaceShipController spaceShip;
	[SerializeField] CameraController cameraController;

	int score = 0;
	public int Score { get { return score; } }

	[SerializeField] UIController UIController;
	void Start () 
	{
		Init();
		StartGame();
	} 
	void Update()
	{
		framesPassed++;
		if(framesPassed % updateQTAfterFrame == 0)
		{
			framesPassed = 0;
			updateQT = true;
		}
		
		if(allAsteroidsSpawned) 
		{ 
			UpdateAsteroidsPosition();
			MoveVisibleAsteroid();
			if (updateQT)
				updateQT = false;
		}
	}
	
	void Init()
	{
		quadTree = new QuadTree(new Rectangle(0, 0, grid.column * grid.distanceBetween * 2, grid.row * grid.distanceBetween * 2));
		asteroidsData = new Asteroid[grid.row * grid.column];
		visibleAsteroids = new Dictionary<int, GameObject>();
	}
	void StartGame() 
	{
		if(StartGameTriggered != null) 
		{
			StartGameTriggered();
		}
	}
	public void GameOver() 
	{
		score = 0;
		if(GameOverTriggered != null)
		{
			GameOverTriggered();
		}
	}
	public void AddPoints(int points)
	{
		score += points;
		UIController.UpdateScore();
	}
	
	IEnumerator SpawnAsteroid(int id, float delay = 0) 
	{
		if(delay != 0)
			yield return new WaitForSeconds(delay);
		if(cameraController.IsVisible(asteroidsData[id].position.x, asteroidsData[id].position.y))
		{
			GameObject obj = ObjectsPool.Instance.SpawnObject
			(
				PoolObjTypes.asteroid,
				new Vector3(asteroidsData[id].position.x, asteroidsData[id].position.y),
				Quaternion.identity
			);
			visibleAsteroids.Add(id, obj);
		}
		asteroidsData[id].isActive = true;
	}
	void HideAsteroid(int id)
	{
		ReturnToPool(id);
		visibleAsteroids.Remove(id);
	}
	void MoveVisibleAsteroid()
	{
		foreach (var element in visibleAsteroids)
		{
			element.Value.transform.position += asteroidsData[element.Key].speed * Time.deltaTime; 
		}
	}
	void SetAsteroidData() 
	{
		int positionX, positionY;
		int row = grid.row;
		int column = grid.column;
		int distanceBetween = grid.distanceBetween;
		asteroidCount = 0;

		for (int i = -row / 2; i < row / 2 ; i++)
		{
			positionX = i * distanceBetween + distanceBetween / 2;
			for (int j = -column / 2; j < column / 2; j++) 
			{
			 	positionY = j * distanceBetween + distanceBetween / 2; 

				Asteroid newAsteroid = new Asteroid
				(
					new Point(positionX, positionY, asteroidRadius, asteroidCount),
					new Vector2(GenerateRandomNumber(asteroidSpeed, 0), GenerateRandomNumber(asteroidSpeed, 0))
				);
				asteroidsData[asteroidCount] = newAsteroid;
				StartCoroutine(SpawnAsteroid(asteroidCount));
				asteroidCount++;
			}
		}
		allAsteroidsSpawned = true;
	}
	void UpdateAsteroidsPosition()
	{
		// Clearing QuadTree nodes
		if(updateQT)
			quadTree.ClearAllNodes();
		
		for (int i = 0; i < asteroidsData.Length; i++) 
		{	
			Asteroid asteroid = asteroidsData[i];
			if(asteroid.isActive)
			{ 
				asteroid.UpdatePosition(Time.deltaTime);

				//Displaying only objects which are visible to camera in the scene
				bool isVisible = cameraController.IsVisible(asteroid.position.x, asteroid.position.y);
				bool alreadyShowing = visibleAsteroids.ContainsKey(i);
		
				if(isVisible && !alreadyShowing) 
				{
					StartCoroutine(SpawnAsteroid(i));
				} 
				else if(!isVisible && alreadyShowing)
				{
					HideAsteroid(i);
				}

				if(updateQT)
				{
					quadTree.Insert(asteroid.position.x, asteroid.position.y, asteroidRadius, i);
				}
			}
		}
		if(updateQT)
		{	
			CheckAsteroidsCollision();
		}
	}
	void CheckAsteroidsCollision() 
	{
		for (int i = 0; i < asteroidsData.Length; i++) 
		{
			if(asteroidsData[i].isActive)
			{
				QuadTreeCollision(asteroidsData[i].position.x, asteroidsData[i].position.y, asteroidRadius, i);
			}
		}
	}

	//Check asteroids colliding with each other in the Quadtree
	public bool QuadTreeCollision(float x, float y, float radius, int id)
	{
		int foundPoint = quadTree.Query(x, y, radius, id);
		if(foundPoint != -1)  
		{
			ReRespawnAsteroid(id);
			ReRespawnAsteroid(foundPoint);	
			return true;
		}
		return false;
	}
	//Check other objects to asteroids colliding in QuadTree
	public bool QuadTreeCollision(float x, float y, float radius)
	{
		int foundPoint = quadTree.Query(x, y, radius, -1);
		if(foundPoint != -1)  
		{
			ReRespawnAsteroid(foundPoint);	
			return true;
		}
		return false;
	}
	
	void ReRespawnAsteroid(int id)
	{
		//Generating new position randomly outside of users view
		PositionOutOfFrustum(id);
		if(cameraController.IsVisible(asteroidsData[id].position.x, asteroidsData[id].position.y))
		{
			//Returning the collided object back to pool
			HideAsteroid(id);
			asteroidsData[id].isActive = false;

			//Respawning asteroid back on the scene with delay
			StartCoroutine(SpawnAsteroid(id, respawnDelayTime));
		}
	}


	#region Helper Methods
	void PositionOutOfFrustum(int id)
	{	
		float newX = GenerateRandomNumber(quadTree.boundry.w, spaceShip.position.x);
		float newY = GenerateRandomNumber(quadTree.boundry.h, spaceShip.position.y);

		//If new position is in player's frustum, regenerate again.
		while(cameraController.IsVisible(newX, newY))
		{
			newX = GenerateRandomNumber(quadTree.boundry.w, spaceShip.position.x);
			newY = GenerateRandomNumber(quadTree.boundry.h, spaceShip.position.y);
		}
		asteroidsData[id].position = new Point(newX, newY, asteroidRadius, id);
	}
	void ReturnAllToPool()
	{
		allAsteroidsSpawned = false;
		foreach (int asteroidID in visibleAsteroids.Keys)
		{
			ReturnToPool(asteroidID);
		}
		visibleAsteroids.Clear();
	}
	void ReturnToPool(int id)
	{
		ObjectsPool.Instance.ReturnToPool
		(
			PoolObjTypes.asteroid,
			visibleAsteroids[id]
		);
	}

	int GenerateRandomNumber(int range, float exceptionRange)
	{
		int random = UnityEngine.Random.Range(-range, range + 1);
	
		if(random == exceptionRange || random == -exceptionRange) 
			random = GenerateRandomNumber(range, exceptionRange);
		
		return random;
	}
	
	#endregion
	
	void OnEnable()
	{
		StartGameTriggered += UIController.StartGame;
		StartGameTriggered += SetAsteroidData;

		GameOverTriggered += UIController.GameOver;
		GameOverTriggered += ReturnAllToPool;
	}
	void OnDestroy() 
	{
		StartGameTriggered -= SetAsteroidData;
		StartGameTriggered -= UIController.StartGame;

		GameOverTriggered -= UIController.GameOver;
		GameOverTriggered -= ReturnAllToPool;
	}
}