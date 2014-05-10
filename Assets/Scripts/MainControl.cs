using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DungeonNamespace;
using BattleNamespace;
using MessageHandle;

public class MainControl : MonoBehaviour {

	public GameObject tile;

	private static bool battle = false;

	private static DungeonControl dControl;
	private static BattleControl bControl;


	// Use this for initialization
	void Start () {
		RoomStorage.Init ();
		Party.Init();
		MessageHandler.Init();
		MainControl.dControl = new DungeonControl();
		MainControl.bControl = new BattleControl();
	}
	
	// Update is called once per frame
	void Update () {

		if (battle)
		{
			MainControl.bControl.Update ();
		}
		else
		{
			MainControl.dControl.Update ();
		}
	}

	public static void StartBattle(MonsterGroup monsters)
	{
		MainControl.bControl.StartBattle(monsters);
		MainControl.battle = true;
	}

	public static void EndBattle()
	{
		MainControl.battle = false;
	}


}
