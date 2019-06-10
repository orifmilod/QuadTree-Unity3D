using UnityEngine;
class Asteroid 
{	
	public bool isActive;
	public Point position;
	public Vector3 speed;
	
	public Asteroid(Point position, Vector2 speed, bool isActive = true) 
	{
		this.isActive = isActive;
		this.position = position;
		this.speed = speed;
	}
	public void UpdatePosition(float multiplier)
	{
		position.x += speed.x * multiplier;
		position.y += speed.y * multiplier;
	}
}