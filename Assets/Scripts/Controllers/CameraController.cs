using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour 
{
    [SerializeField] float dampTime = 0.15f;
    new Camera camera;
    private Vector3 velocity = Vector3.zero;
    SpaceShipController spaceShip;
    Vector3 delta, point, destination;
    Vector3 topRightCorner, bottomLeftCorner;
	void Start() 
    {
        spaceShip = GameObject.FindObjectOfType<SpaceShipController>();
        camera = GetComponent<Camera>();
        UpdateClipPlanes();
	}
    public bool IsVisible(float x, float y)
    {
        return (
			x > bottomLeftCorner.x && x < topRightCorner.x && 
			y > bottomLeftCorner.y && y < topRightCorner.y
		);
    }
    void Update() 
    {
        if (spaceShip != null) 
        {
            UpdateClipPlanes();
            MoveCamera();
        }
    }
    void UpdateClipPlanes()
    {
        topRightCorner = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
        bottomLeftCorner = camera.ScreenToWorldPoint(Vector3.zero);
    }

    void MoveCamera()
    {
        point = camera.WorldToViewportPoint(spaceShip.transform.position);
        delta = spaceShip.transform.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
        destination = transform.position + delta;
        transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
    }
}
