using UnityEngine;
using System.Collections.Generic;
class QuadTree 
{
	public Rectangle boundry;
	Point node;
	bool divided = false;
	bool nodeInserted = false;
	QuadTree northEast, northWest, southEast, southWest;
	public QuadTree(Rectangle boundry) 
	{
		this.boundry = boundry;
	}	
	
	#region Methods

	//Clear all the nodes in the Quad-Tree
	public void ClearAllNodes() 
	{
		if(!nodeInserted) return;
		nodeInserted = false;

		if(divided) 
		{
			northEast.ClearAllNodes();
			northWest.ClearAllNodes();
			southEast.ClearAllNodes();
			southWest.ClearAllNodes();
		}
		divided = false;
	}
	/// <summary>Insert a node in the Quad-Tree</summary>
	public bool Insert(Point point) 
	{
		//Checking if the position is in the boundries of the node.
		if(!boundry.Contains(point)) return false; 
		if(!nodeInserted) 
		{ 
			this.node = point;
			nodeInserted = true;
			return true;
		}
		else 
		{
			if(!divided) 
				SubDivide();

		 	if(northEast.Insert(point)) return true;
			if(northWest.Insert(point)) return true;
			if(southEast.Insert(point)) return true;
			if(southWest.Insert(point)) return true;
		}
		return false;
	}

	public bool Insert(float x, float y, float radius, int id) 
	{
		//Checking if the position is in the boundries of the node.
		if(!boundry.Contains(x, y)) return false; 
		if(!nodeInserted) 
		{
			nodeInserted = true;
			node = new Point(x, y, radius, id);
			return true;
		}
		else 
		{
			if(!divided) 
				SubDivide();

		 	if(northEast.Insert(x, y, radius, id)) return true;
			if(northWest.Insert(x, y, radius, id)) return true;
			if(southEast.Insert(x, y, radius, id)) return true;
			if(southWest.Insert(x, y, radius, id)) return true;
		}
		return false;
	}
	/// <summary>Query through all the nodes in Quad-Tree, check if any of the point overlap's. Return -1 if nothing overlaps.</summary>
	public int Query(float x, float y, float radius, int id)
	{
		if(!nodeInserted) return -1;
		if(!boundry.Contains(x, y)) return -1;

		
		if(DoOverlap(node.x, node.y, x, y, radius) && node.id != id) 
				return node.id;	
		if(divided) 
		{
			return QueryLeafs(x, y, radius, id);
		}
		return -1;
	}

	// public int Query(Point searchingPoint)
	// {
	// 	if(numberOfNodes == 0) return -1;
	// 	if(!boundry.Contains(searchingPoint)) return -1;
	
	// 	for (int i = 0; i < numberOfNodes; i++)
	// 	{
	// 		if(DoOverlap(nodes[i], searchingPoint) && nodes[i].id != searchingPoint.id) 
	// 		return nodes[i].id;
	// 	}	
	// 	if(divided)
	// 	{
	// 		QueryLeafs(searchingPoint);
	// 	}
	// 	return -1;
	// }

	// int QueryLeafs(Point searchingPoint)
	// {
	// 	int result = -1;
	// 	result = northEast.Query(searchingPoint);
	// 	if(result != -1)  return result;

	// 	result = northWest.Query(searchingPoint);
	// 	if(result != -1)  return result;

	// 	result = southEast.Query(searchingPoint);
	// 	if(result != -1)  return result;
		
	// 	result = southWest.Query(searchingPoint);
	// 	if(result != -1)  return result;

	// 	return -1;
	// }
	int QueryLeafs(float x, float y, float radius, int id)
	{
		int result = -1;
		result = northEast.Query(x, y, radius, id);
		if(result != -1)  return result;

		result = northWest.Query(x, y, radius, id);
		if(result != -1)  return result;

		result = southEast.Query(x, y, radius, id);
		if(result != -1)  return result;
		
		result = southWest.Query(x, y, radius, id);
		if(result != -1)  return result;

		return -1;
	}
	#endregion
	
	#region HelperMethods

	/// <summary>Divide the Quadtree into 4 equal parts and set it's boundries, NorthEast, NorthWest, SouthEast and SouthWest.</summary>
	private void SubDivide() 
	{
		//Size of the sub boundries 
		if(northEast == null) 
		{	
			int height = boundry.h / 2;
			int width = boundry.w / 2;
			int x = boundry.x;
			int y = boundry.y;

			Rectangle ne = new Rectangle(x + width, y + height, width, height);
			northEast = new QuadTree(ne);

			Rectangle nw = new Rectangle(x - width, y + height, width, height);
			northWest = new QuadTree(nw);

			Rectangle se = new Rectangle(x + width, y - height, width, height);
			southEast = new QuadTree(se);

			Rectangle sw = new Rectangle(x - width, y - height, width, height);
			southWest = new QuadTree(sw);
		} 
		divided = true;
	}

	// Shows boundires of the quadtree and SubNodes
	public void ShowBoundries()
	{ 
		float h = boundry.h;
		float w = boundry.w;
		float x = boundry.x;
		float y = boundry.y;

		Vector2 bottomLeftPoint = new Vector2(x - w, y - h);
		Vector2 bottomRightPoint = new Vector2(x + w, y - h);
		Vector2 topRightPoint = new Vector2(x + w, y + h);
		Vector2 topLeftPoint = new Vector2(x - w, y + h);
		
		Debug.DrawLine(bottomLeftPoint, bottomRightPoint, Color.red, 10);	//bottomLine
		Debug.DrawLine(bottomLeftPoint, topLeftPoint, Color.red, 10);		//leftLine
		Debug.DrawLine(bottomRightPoint, topRightPoint, Color.red, 10);	//rightLine
		Debug.DrawLine(topLeftPoint, topRightPoint, Color.red, 10);		//topLine

		if(divided)
		{
			northEast.ShowBoundries();
			northWest.ShowBoundries();
			southEast.ShowBoundries();
			southWest.ShowBoundries();
		}
	}
	
	public void ShowBound(float x, float y, float h, float w) 
	{
		Vector2 bottomLeftPoint = new Vector2(x - w, y - h);
		Vector2 bottomRightPoint = new Vector2(x + w, y - h);
		Vector2 topRightPoint = new Vector2(x + w, y + h);
		Vector2 topLeftPoint = new Vector2(x - w, y + h);

		Debug.DrawLine(bottomLeftPoint, bottomRightPoint, Color.blue, 5);	//bottomLine
		Debug.DrawLine(bottomLeftPoint, topLeftPoint,  Color.blue, 5);		//leftLine
		Debug.DrawLine(bottomRightPoint, topRightPoint,  Color.blue, 5);	//rightLine
		Debug.DrawLine(topLeftPoint, topRightPoint,  Color.blue, 5);		//topLine
	}
	public void LogNodes() 
	{
		if(!nodeInserted) return;
		Debug.Log(node.id);	

		if (divided) 
		{
			northEast.LogNodes();
			northWest.LogNodes();
			southWest.LogNodes();
			southEast.LogNodes();
		}
	}
	
	public bool DoOverlap(float x1, float y1, float x2, float y2, float radius)
	{
       	// If one rectangle is on left side of other  
        if (x1 - radius > x2 + radius || x2 - radius > x1 + radius) 
            return false; 
  
        // If one rectangle is above other  
        if (y1 + radius < y2 - radius || y2 + radius < y1 - radius)  
			return false; 

        return true; 
	}
	public bool DoOverlap(Point node1, Point node2)  
    { 
       	// If one rectangle is on left side of other  
        if (node1.x - node1.radius > node2.x + node2.radius || 
			node2.x - node2.radius > node1.x + node1.radius) 
            return false; 
  
        // If one rectangle is above other  
        if (node1.y + node1.radius < node2.y - node2.radius || 
			node2.y + node2.radius < node1.y - node1.radius )  
			return false; 

        return true; 
    } 

	#endregion
}
