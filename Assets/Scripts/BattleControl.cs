using SpriteObj;
using System.Collections.Generic;
using MessageHandle;
using UnityEngine;

namespace BattleNamespace
{
	public class BattleControl
	{
		public SpriteObject background;

		List<Nametag> player_tags;
		List<Nametag> monster_tags;

		List<Nametag> standard_actions;
		List<Nametag> abilities;

		List<Monster> monsters;



		bool waiting = false;
		
		public BattleControl ()
		{
			player_tags = new List<Nametag>();
			monster_tags = new List<Nametag>();
			standard_actions = new List<Nametag>();
			abilities = new List<Nametag>();
			monsters = new List<Monster>();
		}

		public void CleanUp()
		{
			foreach (Nametag n in player_tags)
			{
				GameObject.Destroy(n.obj);
			}
			foreach (Nametag n in monster_tags)
			{
				GameObject.Destroy(n.obj);
			}
			player_tags.Clear();
			monster_tags.Clear();
			monsters.Clear();
			MainControl.EndBattle();
		}

		public void Update()
		{
			if (!waiting)
			{
				StartPlayerTurn();
				waiting = true;
			}
			else
			{
				GetMessage();
			}
		}

		private void StartPlayerTurn()
		{
			float camheight = Camera.main.orthographicSize * 2.0f;
			float camwidth = Camera.main.aspect * camheight;
			float camx = Camera.main.transform.position.x - camwidth / 2;
			float camy = Camera.main.transform.position.y + camheight / 2;

			Nametag l = new Nametag();
			l.SetText("Attack");
			l.SetMessageDetails("standard_action", 0);
			l.SetPosition(camx + (camwidth / 2) - 256.0f, camy - camheight + 64.0f * 3 + 16.0f);
			l.SetScale(8, 2);
			standard_actions.Add(l);

			l = new Nametag();
			l.SetText("Abilities");
			l.SetMessageDetails("standard_action", 1);
			l.SetPosition(camx + (camwidth / 2), camy - camheight + 64.0f * 3 + 16.0f);
			l.SetScale(8, 2);
			standard_actions.Add(l);

			l = new Nametag();
			l.SetText("Defend");
			l.SetMessageDetails("standard_action", 2);
			l.SetPosition(camx + (camwidth / 2) + 256.0f, camy - camheight + 64.0f * 3 + 16.0f);
			l.SetScale(8, 2);
			standard_actions.Add(l);
		}

		private void GetMessage()
		{
			//Pull things off the message queue
			while (MessageHandler.HasMessages())
			{
				Message m = MessageHandler.GetMessage();
				Debug.Log(m.type + " " + m.index);
			}
		}

		public void StartBattle(MonsterGroup mg)
		{
			float camheight = Camera.main.orthographicSize * 2.0f;
			float camwidth = Camera.main.aspect * camheight;
			float camx = Camera.main.transform.position.x - camwidth / 2;
			float camy = Camera.main.transform.position.y + camheight / 2;

			float namewidth = 32.0f * 8.0f;
			float nameheight = 32.0f * 3.0f;
			
			player_tags.Clear();
			monster_tags.Clear();
			monsters.Clear();
			for (int i = 0; i < Party.party_list.Count; i++)
			{
				PlayerCharacter pc = Party.party_list[i];
				player_tags.Add(new Nametag());
				player_tags[i].SetMessageDetails("player", i);
				player_tags[i].SetText(pc.name);
				player_tags[i].SetPosition(camx + (camwidth / 2) + (i-2) * namewidth, camy - camheight + nameheight);
				player_tags[i].SetScale(8.0f, 3.0f);
			}

			for (int i = 0; i < mg.monsters.Count; i++)
			{
				Monster m = mg.monsters[i];
				monster_tags.Add(new Nametag());
				monster_tags[i].SetMessageDetails("monster", i);
				monster_tags[i].SetText(m.name);
				monster_tags[i].SetPosition(camx + (camwidth / 2) - (mg.monsters.Count * namewidth / 2) + i * namewidth, camy);
				monster_tags[i].SetScale(8.0f, 3.0f);
			}
		}
	}
}

