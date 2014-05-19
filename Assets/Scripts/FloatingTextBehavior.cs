using UnityEngine;
using System.Collections;

public class FloatingTextBehavior : MonoBehaviour {

	public Vector3 end_pos;
	public float move_time;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position != end_pos)
		{
			transform.position = Vector3.Lerp(transform.position, end_pos, move_time);
		}
	}
}
