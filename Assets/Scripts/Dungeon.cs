using System.Collections.Generic;
using UnityEngine;

namespace DungeonNamespace
{
	public class RoomRef
	{
		public DungeonRoom room;
		public int x;
		public int y;
		public List<RoomRef> neighbors;

		public List<MonsterGroup> monsters;

		public RoomRef(DungeonRoom _room, int _x, int _y)
		{
			this.room = _room;
			x = _x;
			y = _y;
			neighbors = new List<RoomRef>();
		}

		public int GetCenterX()
		{
			return x + (int)this.room.GetCenter().x;
		}

		public int GetCenterY()
		{
			return y + (int)this.room.GetCenter().y;
		}
	}

	public class Tile
	{
		public int x;
		public int y;
		public bool wall = true;
		public char mask = 'X';
		public int cost = 1;

		public bool revealed = true;
		public bool hidden = false;

		public Tile(int _x, int _y)
		{
			x = _x;
			y = _y;
		}

		public void SetToMovable()
		{
			wall = false;
			mask = ' ';
		}
	}

	public class MonsterGroupPos
	{
		public MonsterGroup mg;
		public int x;
		public int y;

		public MonsterGroupPos(MonsterGroup g, int _x, int _y)
		{
			mg = g;
			x = _x;
			y = _y;
		}
	}

	public class Dungeon
	{
		private Tile[,] grid;

		public int width = 80;
		public int height = 80;

		public List<RoomRef> rooms;

		//Config
		private int min_rooms = 10;
		private int room_space = 7;
		private int room_border = 5;
		private int interconnect_distance = 7;

		public List<MonsterGroupPos> monsters;

		public Dungeon ()
		{
			this.grid = new Tile[height, width];
			monsters = new List<MonsterGroupPos>();
			CreateMaze();
		}

		public void CreateMaze()
		{
			int attempts = 1;
			ClearDungeon();
			BuildMaze ();
			while (rooms.Count < min_rooms && attempts < 5)
			{
				ClearDungeon();
				BuildMaze();
				attempts++;
				
			}
		}

		public Tile GetTile(int x, int y)
		{
			try
			{
				Tile t = this.grid[y, x];
			}
			catch
			{
				Debug.Log (x);
				Debug.Log (y);
			}
			return this.grid[y, x];
		}

		private void ClearDungeon()
		{
			for (int y = 0; y < this.height; y++)
			{
				for (int x = 0; x < this.width; x++)
				{
					this.grid[y, x] = new Tile(x, y);
					this.grid[y, x].mask = 'X';
					this.grid[y, x].cost = 1;
				}
			}
		}

		public bool ValidEmptyTile(int x, int y)
		{
			if (x < 0 || x >= width || y < 0 || y >= height)
			{
				return false;
			}
			Tile tile = GetTile (x, y);
			if (tile.mask == 'X' || tile.mask == 'I' || tile.mask == 'T' || tile.mask == 'M')
			{
				return false;
			}
			return true;
		}

		public MonsterGroup GetMonster(int x, int y)
		{
			foreach (MonsterGroupPos mg in monsters)
			{
				if (x == mg.x && y == mg.y)
				{
					return mg.mg;
				}
			}
			return null;
		}

		public MonsterGroup GetTreasure(int x, int y)
		{
			return null;
			foreach (MonsterGroupPos mg in monsters)
			{
				if (x == mg.x && y == mg.y)
				{
					return mg.mg;
				}
			}
			return null;
		}

		private bool TestRoomPlacement(DungeonRoom room, int _xstart, int _ystart, int border=1)
		{
			int xstart = _xstart - border;
			int ystart = _ystart - border;
			int xend = xstart + room.width + border * 2;
			int yend = ystart + room.height + border * 2;
			if (xstart < 1 || xend > width - 2 || ystart < 1 || yend > height - 2)
			{
				return false;
			}

			for (int y = ystart; y < yend; y++)
			{
				for (int x = xstart; x < xend; x++)
				{
					if (GetTile(x, y).mask == ' ')
					{
						return false;
					}
				}
			}
			return true;
		}

		private void AddRoom(DungeonRoom room, int xstart, int ystart)
		{
			for (int y = 0; y < room.height; y++)
			{
				for (int x = 0; x < room.width; x++)
				{
					char t = room.GetTile(x, y);
					GetTile(x + xstart, y + ystart).mask = t;
					if (t == 'I')
					{
						GetTile(x + xstart, y + ystart).cost = -1;
					}
				}
			}
		}

		private void BuildOutRoom(RoomRef room)
		{
			Utility.Utility.ShuffleList(room.room.possible_exits);
			int neighbor_count = Random.Range (room.room.min_neighbors, room.room.max_neighbors + 1);
			foreach (char direction in room.room.possible_exits)
			{

				if (room.neighbors.Count < neighbor_count)
				{
					int x = 0; 
					int y = 0;

					string r = room.room.GetRandomRoom();
					int cx = room.GetCenterX();
					int cy = room.GetCenterY();

					int major_offset = Random.Range(room_border + 1, room_border + 1 + room_space);
					int minor_offset = Random.Range(-2, 2);
					switch (direction)
					{
						case 'E':
							x = room.x + room.room.width + major_offset;
							y = cy - (RoomStorage.GetRoom(r).height / 2) + minor_offset;
							break;
						case 'W':
							x = room.x - RoomStorage.GetRoom(r).width - major_offset;
							y = cy - (RoomStorage.GetRoom(r).height / 2) + minor_offset;
							break;
						case 'N':
							x = cx - (RoomStorage.GetRoom(r).width / 2) + minor_offset;
							y = room.y + RoomStorage.GetRoom(r).height + major_offset;
							break;
						case 'S':
							x = cx - (RoomStorage.GetRoom(r).width / 2) + minor_offset;
							y = room.y - room.room.height - major_offset;
							break;
					}
					if (TestRoomPlacement(RoomStorage.GetRoom(r), x, y, room_border))
					{
						AddRoom(RoomStorage.GetRoom(r), x, y);
						room.neighbors.Add(new RoomRef(RoomStorage.GetRoom(r), x, y));
					}
				}
			}
		}

		private void CreateMonster(RoomRef room)
		{
			int x = -1;
			int y = -1;

			while (!ValidEmptyTile(x, y))
			{
				x = Random.Range(room.x, room.x + room.room.width);
				y = Random.Range(room.y, room.y + room.room.height);
			}
			MonsterGroup mg = new MonsterGroup();
			Monster m = new Monster();
			m.name = "Monster 1";
			mg.monsters.Add(m);
			m = new Monster();
			m.name = "Monster 2";
			mg.monsters.Add(m);
			m = new Monster();
			m.name = "Monster 3";
			mg.monsters.Add(m);
			monsters.Add(new MonsterGroupPos(mg, x, y));
		}

		private void CreateTreasure(RoomRef room)
		{

		}

		private void BuildFeatures(RoomRef room)
		{
			float chance = room.room.monster_chance;
			while (Random.value < chance)
			{
				CreateMonster(room);
				chance *= room.room.monster_dr;
			}

			chance = room.room.treasure_chance;
			while (Random.value < chance)
			{
				CreateTreasure(room);
				chance *= room.room.treasure_dr;
			}
		}

		private void BuildMaze()
		{
			int x = Random.Range(0, width);
			int y = Random.Range(0, height);

			rooms = new List<RoomRef>();
			RoomRef start_room = new RoomRef(RoomStorage.GetRoom("Square5x5"), x, y);
			while (!TestRoomPlacement(start_room.room, x, y, room_border))
			{
				x = Random.Range(0, width);
				y = Random.Range(0, height);
				start_room = new RoomRef(RoomStorage.GetRoom("Square5x5"), x, y);
			}
			AddRoom(start_room.room, x, y);

			Queue<RoomRef> room_queue = new Queue<RoomRef>();
			room_queue.Enqueue(start_room);

			//Build out the rooms
			while (room_queue.Count > 0)
			{
				RoomRef r = room_queue.Dequeue();
				rooms.Add(r);
				BuildOutRoom(r);
				foreach (RoomRef neighbor in r.neighbors)
				{
					room_queue.Enqueue(neighbor);
				}
			}

			//Build the halls
			foreach (RoomRef room in rooms)
			{
				foreach (RoomRef n in room.neighbors)
				{
					int sx = room.GetCenterX();
					int sy = room.GetCenterY();
					int ex = n.GetCenterX();
					int ey = n.GetCenterY();
					List<Tile> path = PathFind(sx, sy, ex, ey);
					if (path != null)
					{
						foreach (Tile t in path)
						{
							t.SetToMovable();
						}
					}
				}

				BuildFeatures(room);
			}

			//Turn all the placeholder walls into real items
			for (y = 0; y < this.height; y++)
			{
				for (x = 0; x < this.width; x++)
				{
					if (GetTile (x, y).mask == 'I')
					{
						GetTile (x, y).mask = 'X';
					}
				}
			}

		}

		private float EstimatePathScore(int startx, int starty, int endx, int endy)
		{
			return Mathf.Abs(startx - endx) + Mathf.Abs(starty - endy);
		}

		public List<Tile> PathFind(int startx, int starty, int endx, int endy)
		{
			List<Tile> explored = new List<Tile>();
			List<Tile> pending = new List<Tile>();
			Dictionary<Tile, Tile> path = new Dictionary<Tile, Tile>();
			Dictionary<Tile, float> best_score = new Dictionary<Tile, float>();
			Dictionary<Tile, float> est_score = new Dictionary<Tile, float>();

			path.Add(GetTile(startx, starty), null);
			best_score.Add(GetTile(startx, starty), 0.0f);
			est_score.Add(GetTile(startx, starty), EstimatePathScore(startx, starty, endx, endy));
			pending.Add(GetTile(startx, starty));

			while (pending.Count > 0)
			{
				//Get the best node
				Tile current = pending[0];
				float current_best = est_score[current];
				foreach (Tile node in pending)
				{
					if (est_score[node] < current_best)
					{
						current = node;
						current_best = est_score[node];
					}
				}
				//
				if (current == GetTile(endx, endy))
				{
					List<Tile> final_path = new List<Tile>();
					while (path[current] != null)
					{
						final_path.Add(current);
						current = path[current];
					}
					final_path.Reverse();
					return final_path;
				}

				pending.Remove(current);
				explored.Add(current);

				List<Tile> neighbors = new List<Tile>();
				if (current.x < width - 2) { neighbors.Add(GetTile(current.x + 1, current.y)); }
				if (current.x > 1) { neighbors.Add(GetTile(current.x - 1, current.y)); }
				if (current.y < height - 2) { neighbors.Add(GetTile(current.x, current.y + 1)); }
				if (current.y > 1) { neighbors.Add(GetTile(current.x, current.y - 1)); }

				foreach (Tile n in neighbors)
				{
					if (!explored.Contains(n) && n.cost > 0)
					{
						float score = best_score[current] + n.cost;
						if (!pending.Contains(n) || score < best_score[n])
						{
							path[n] = current;
							best_score[n] = score;
							est_score[n] = score + EstimatePathScore(n.x, n.y, endx, endy);
							if (!pending.Contains(n))
							{
								pending.Add(n);
							}
						}
					}
				}
			}
			return null;
		}
	}
}

