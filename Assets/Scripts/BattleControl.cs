using SpriteObj;
using System.Collections.Generic;
using MessageHandle;
using UnityEngine;

namespace BattleNamespace
{
	public class BattleControl
	{
		const int DEFEND_INDEX = -2;
		const int ATTACK_INDEX = -1;
		
		const string STANDARD_ACTION = "standard_action";
		const string ABILITY_ACTION = "ability";

		const string TARGET_PLAYER = "player";
		const string TARGET_MONSTER = "monster";
		
		const int STANDARD_ATTACK = 0;
		const int STANDARD_ABILITY = 1;
		const int STANDARD_DEFEND = 2;

		const int DELAY_TIME = 120;

		private int turn_delay = 0;

		SpriteObject background;

		List<Nametag> player_tags;
		List<Nametag> monster_tags;

		List<Button> standard_actions;
		List<Button> abilities;

		MonsterGroup monster_group;

		bool player_turn = true;
		int actor_index = 0;
		int target_index = 0;
		bool target_player_bool = false;

		bool waiting = false;

		bool waiting_target = false; // Have an ability, waiting on target
		bool waiting_ability = false; // Waiting for an ability selection
		bool open_ability_menu = false;

		int selected_ability = ATTACK_INDEX;
		
		public BattleControl ()
		{
			player_tags = new List<Nametag>();
			monster_tags = new List<Nametag>();
			standard_actions = new List<Button>();
			abilities = new List<Button>();
		}

		public void Update()
		{
			foreach (Nametag n in player_tags)
			{
				if (n != null)
				{
					n.Update();
				}
			}
			foreach (Nametag n in monster_tags)
			{
				if (n != null)
				{
					n.Update();
				}
			}

			if (turn_delay <= 0)
			{
				if (!waiting)
				{
					StartPlayerTurn();

				}
				else
				{
					Message m = MessageHandler.GetMessage();
					if (m != null)
					{
						HandleInput(m);
					}
					if (!waiting_target && !waiting_ability)
					{
						TakePlayerTurn();
					}
				}
			}
			else
			{
				turn_delay -= 1;
				if (turn_delay <= 0)
				{
					bool battle_end;
					battle_end = EndTurn();
					if (battle_end)
					{
						EndBattle();
					}
				}
			}
		}



		private bool EndTurn()
		{
			bool battle_end = true;
			for (int i = 0; i < Party.party_list.Count; i++)
			{

			}
			for (int i = 0; i < monster_group.monsters.Count; i++)
			{
				if (monster_tags[i] != null)
				{
					if (monster_group.monsters[i].hp <= 0)
					{
						monster_tags[i].Delete();
						monster_tags[i] = null;
					}
					else
					{
						battle_end = false;
					}
				}
			}
			return battle_end;
		}

		private Actor GetSource()
		{
			if (player_turn)
			{
				return Party.party_list[actor_index];
			}
			else
			{
				return monster_group.monsters[actor_index];
			}
		}

		private Actor GetTarget()
		{
			if (target_player_bool)
			{
				return Party.party_list[target_index];
			}
			else
			{
				return monster_group.monsters[target_index];
			}
		}

		private void HandleAbility(Actor source, Actor target, Ability ability)
		{
			ability.HandleAbility(source, target);
			FloatingText f = new FloatingText();
			f.SetPosition(monster_tags[target_index].GetPosition().x + 96, monster_tags[target_index].GetPosition().y - 32);
			f.SetMovePosition(f.GetPosition().x, f.GetPosition().y - 128, .05f);
			f.SetText(source.GetAttack().GetDamageValue(source).ToString());
			f.SetTimedLife(1.0f);
			MessageLog.AddMessage(string.Format("{0} deals {1} damage to {2}", source.name, source.GetAttack().GetDamageValue(source).ToString(), target.name));
		}

		private void TakePlayerTurn()
		{
			waiting_ability = true;
			Actor source = GetSource();
			Actor target = GetTarget();
			Ability a;
			if (selected_ability == ATTACK_INDEX)
			{
				HandleAbility(source, target, source.GetAttack());
			}
			else if (selected_ability == DEFEND_INDEX)
			{

			}
			else
			{
				Debug.Log("Using Ability " + selected_ability.ToString());
			}
			CleanUpPlayerTurn();
		}

		private void HandleInput(Message m)
		{
			PlayerCharacter pc = Party.party_list[actor_index];
			bool clear_messages = true;
			if (m.type == STANDARD_ACTION)
			{
				switch (m.index)
				{
				case STANDARD_ATTACK:
					selected_ability = ATTACK_INDEX;
					waiting_ability = false;
					waiting_target = true;
					break;
				case STANDARD_ABILITY:
					if (open_ability_menu)
					{
						CleanUpAbilityButtons();
						open_ability_menu = false;
					}
					else
					{
						CreateAbilityButtons();
						open_ability_menu = true;
					}
					waiting_ability = true;
					waiting_target = false;
					break;

				case STANDARD_DEFEND:
					selected_ability = DEFEND_INDEX;
					waiting_ability = false;
					waiting_target = false;
					break;
				}
			}
			else if (m.type == ABILITY_ACTION)
			{

			}
			else if (m.type == TARGET_PLAYER)
			{
				target_player_bool = true;
				target_index = m.index;
				waiting_target = false;
			}
			else if (m.type == TARGET_MONSTER)
			{
				target_player_bool = false;
				target_index = m.index;
				waiting_target = false;
			}
		}
		private void CreateAbilityButtons()
		{

		}

		private void CleanUpPlayerTurn()
		{
			foreach (Button b in standard_actions)
			{
				b.Delete();
			}
			standard_actions.Clear();
			if (open_ability_menu)
			{
				CleanUpAbilityButtons();
			}
			waiting = false;
			turn_delay = DELAY_TIME;
			target_index = -1;
			waiting_target = true;
			waiting_ability = true;
		}

		private void CleanUpAbilityButtons()
		{
			foreach (Button b in abilities)
			{
				b.Delete();
			}
			abilities.Clear();
		}

		private void CleanUpBattle()
		{
			CleanUpPlayerTurn();
			foreach (Nametag n in player_tags)
			{
				if (n != null)
				{
					n.Delete();
				}
			}
			foreach (Nametag n in monster_tags)
			{
				if (n != null)
				{
					n.Delete();
				}
			}
			player_tags.Clear();
			monster_tags.Clear();
			background.Delete();
			background = null;
		}

		private Message GetMessage()
		{
			Message m = null;
			//Pull things off the message queue
			while (MessageHandler.HasMessages())
			{
				m = MessageHandler.GetMessage();
			}
			return m;
		}

		private void StartPlayerTurn()
		{
			float camheight = Camera.main.orthographicSize * 2.0f;
			float camwidth = Camera.main.aspect * camheight;
			float camx = Camera.main.transform.position.x - camwidth / 2;
			float camy = Camera.main.transform.position.y + camheight / 2;
			
			Button l = new Button();
			l.SetText("Attack");
			l.SetMessageDetails(STANDARD_ACTION, STANDARD_ATTACK);
			l.SetPosition(camx + (camwidth / 2) - 256.0f, camy - camheight + 64.0f * 3 + 16.0f);
			l.SetScale(8.0f, 1.5f);
			standard_actions.Add(l);
			
			l = new Button();
			l.SetText("Abilities");
			l.SetMessageDetails(STANDARD_ACTION, STANDARD_ABILITY);
			l.SetPosition(camx + (camwidth / 2), camy - camheight + 64.0f * 3 + 16.0f);
			l.SetScale(8.0f, 1.5f);
			standard_actions.Add(l);
			
			l = new Button();
			l.SetText("Defend");
			l.SetMessageDetails(STANDARD_ACTION, STANDARD_DEFEND);
			l.SetPosition(camx + (camwidth / 2) + 256.0f, camy - camheight + 64.0f * 3 + 16.0f);
			l.SetScale(8.0f, 1.5f);
			standard_actions.Add(l);
			
			waiting_ability = true;
			waiting_target = true;
			
			waiting = true;
			MessageHandler.ClearMessages();
		}

		public void StartBattle(MonsterGroup mg)
		{
			float camheight = Camera.main.orthographicSize * 2.0f;
			float camwidth = Camera.main.aspect * camheight;
			float camx = Camera.main.transform.position.x - camwidth / 2;
			float camy = Camera.main.transform.position.y + camheight / 2;

			float namewidth = 32.0f * 8.0f;
			float nameheight = 32.0f * 3.0f;

			background = new SpriteObject("WhiteBlock");
			background.SetPosition(camx, camy);
			background.SetScale(camwidth / 32, camheight / 32);
			background.SetColor(0f, 0f, 0f, .95f);

			
			player_tags.Clear();
			monster_tags.Clear();

			monster_group = mg;

			turn_delay = 0;

			for (int i = 0; i < Party.party_list.Count; i++)
			{
				PlayerCharacter pc = Party.party_list[i];
				player_tags.Add(new Nametag(pc));
				player_tags[i].SetMessageDetails("player", i);
				player_tags[i].SetText(pc.name);
				player_tags[i].SetPosition(camx + (camwidth / 2) + (i-2) * namewidth, camy - camheight + nameheight);
				player_tags[i].SetScale(8.0f, 3.0f);
			}

			for (int i = 0; i < mg.monsters.Count; i++)
			{
				Monster m = mg.monsters[i];
				monster_tags.Add(new Nametag(m));
				monster_tags[i].SetMessageDetails("monster", i);
				monster_tags[i].SetText(m.name);
				monster_tags[i].SetPosition(camx + (camwidth / 2) - (mg.monsters.Count * namewidth / 2) + i * namewidth, camy);
				monster_tags[i].SetScale(8.0f, 3.0f);
			}
		}

		public void EndBattle()
		{
			CleanUpBattle();
			MainControl.EndBattle(monster_group);
		}
	}
}

