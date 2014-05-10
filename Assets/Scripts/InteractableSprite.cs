using UnityEngine;
using System.Collections;
using MessageHandle;

public class InteractableSprite : MonoBehaviour {

	public string type;
	public int index;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseUpAsButton()
	{
		MessageHandler.AddMessage(new Message(type, index));
	}
}
