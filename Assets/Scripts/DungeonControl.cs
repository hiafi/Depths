using UnityEngine;
using System.Collections.Generic;
using SpriteObj;

namespace DungeonNamespace
{
	public class ActorTile {

		public int x;
		public int y;
		SpriteObject obj;

		public ActorTile(int x, int y)
		{
			this.x = x;
			this.y = y;
			this.obj = new SpriteObject(Resources.Load<Sprite>("Sprites/Person"));
			SetPosition(x, y);
			this.obj.SetLayer(-1);
			this.obj.SetScale(DungeonControl.tile_scale, DungeonControl.tile_scale);
		}

		public void SetPosition(int x, int y)
		{
			this.x = x;
			this.y = y;
			this.obj.SetPosition(x * (32 * DungeonControl.tile_scale), y * (32 * DungeonControl.tile_scale) + (32 * DungeonControl.tile_scale));
		}

		public void Move(int dx, int dy)
		{
			SetPosition(this.x + dx, this.y + dy);
		}
	}

	public class DungeonControl {

		public Dungeon dungeon;

		public static float tile_scale = 1f; //Normal size 32

		//Camera Stuff
		public bool cam_lock = false;
		public float camDragSpeed = 1000;
		private Vector3 camDragOrigin;

		private List<SpriteObject> tiles;
		private ActorTile player;
		private List<ActorTile> monster_tiles;


		public DungeonControl()
		{
			monster_tiles = new List<ActorTile>();

			dungeon = new Dungeon();

			create_tiles();

			player = new ActorTile(dungeon.rooms[0].GetCenterX(), dungeon.rooms[0].GetCenterY());

			SetCamXY(player.x, player.y);

			foreach (MonsterGroupPos mg in dungeon.monsters)
			{
				monster_tiles.Add(new ActorTile(mg.x, mg.y));
			}
		}
		
		// Update is called once per frame
		public void Update () {
		
			if (GetMovement ())
			{
				TakeTurn ();
			}

			if (!cam_lock)
			{
				if (Input.GetMouseButtonDown(0))
					
				{
					camDragOrigin = Input.mousePosition;
					return;
				}
				if (!Input.GetMouseButton(0)) return;
				Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - camDragOrigin);
				camDragOrigin = Input.mousePosition;
				Vector3 move = new Vector3(pos.x * 1.5f * -camDragSpeed, pos.y * -camDragSpeed, 0);
				Camera.main.transform.Translate(move, Space.World);  
			}
		}

		public void SetCamXY(int x, int y)
		{
			Camera.main.transform.position = new Vector3(x * 32.0f, y * 32.0f, Camera.main.transform.position.z);
		}

		public bool CheckCollision(int x, int y)
		{
			if (x > 0 && x < dungeon.width - 1 && y > 0 && y < dungeon.height - 1)
			{
				if (dungeon.ValidEmptyTile(x, y))
				{
					return true;
				}
			}
			return false;
		}

		public bool GetMovement()
		{
			if (Input.GetButtonDown("Left"))
			{

				if (CheckCollision(player.x - 1, player.y))
				{
					player.Move(-1, 0);
					return true;
				}
			}
			if (Input.GetButtonDown("Right"))
			{
				if (CheckCollision(player.x + 1, player.y))
				{
					player.Move(1, 0);
					return true;
				}
			}
			if (Input.GetButtonDown("Down"))
			{
				if (CheckCollision(player.x, player.y - 1))
				{
					player.Move(0, -1);
					return true;
				}
			}
			if (Input.GetButtonDown("Up"))
			{
				if (CheckCollision(player.x, player.y + 1))
				{
					player.Move(0, 1);
					return true;
				}
			}
			return false;
		}

		public void TakeTurn()
		{
			SetCamXY(player.x, player.y);
			MonsterGroup mg = dungeon.GetMonster(player.x + 1, player.y);
			if (mg == null)
			{
				mg = dungeon.GetMonster(player.x - 1, player.y);
				if (mg == null)
				{
					mg = dungeon.GetMonster(player.x, player.y + 1);
					if (mg == null)
					{
						mg = dungeon.GetMonster(player.x, player.y - 1);
					}
				}
			}
			if (mg != null)
			{
				MainControl.StartBattle(mg);
			}
		}

		//Drawing Stuff

		void create_tiles()
		{
			tiles = new List<SpriteObject>();
			for (int y = 0; y < this.dungeon.height; y++)
			{
				for (int x = 0; x < this.dungeon.width; x++)
				{
					SpriteObject t = new SpriteObject();
					t.SetPosition(x * (32 * tile_scale), y * (32 * tile_scale) + (32 * tile_scale));
					t.SetScale(tile_scale, tile_scale);
					tiles.Add(t);
				}
			}
			set_tiles();
		}
		
		void set_tile_sprite(int x, int y, Sprite sprite)
		{
			tiles[y * dungeon.width + x].SetSprite(sprite);
		}
		
		void set_tiles()
		{
			Sprite wall = Resources.Load<Sprite>("Sprites/Wall");
			Sprite floor = Resources.Load<Sprite>("Sprites/Floor");
			for (int y = 0; y < this.dungeon.height; y++)
			{
				for (int x = 0; x < this.dungeon.width; x++)
				{
					switch (dungeon.GetTile(x, y).mask)
					{
					case ' ': 
						set_tile_sprite (x, y, floor);
						break;
					case 'X':
						set_tile_sprite (x, y, wall);
						break;
					default:
						break;
					}
				}
			}
		}


	}
}