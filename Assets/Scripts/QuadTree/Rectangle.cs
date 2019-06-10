class Rectangle
{
	public int x; 
	public int y; 
	public int h;
	public int w;
	public Rectangle(int x, int y, int w, int h) 
	{
		this.y = y;
		this.x = x;
		this.h = h;
		this.w = w;
	}
	//For checking if the point is in the boundries of the rectangle
	public bool Contains(Point point) {
		return 
		point.x >= x - w - 1 && 
		point.x <= x + w + 1 &&
		point.y >= y - h - 1 && 
		point.y <= y + h + 1;
	}
	public bool Contains(float posX, float posY) {
		return 
		posX >= x - w - 1 && 
		posX <= x + w + 1 &&
		posY >= y - h - 1 && 
		posY <= y + h + 1;
	}
}


