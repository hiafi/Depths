using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleJSON;

namespace DungeonNamespace
{
	public static class RoomStorage
	{
		private static Dictionary<string, DungeonRoom> rooms;

		public static DungeonRoom GetRoom(string room)
		{
			if (RoomStorage.rooms == null)
			{
				Init ();
			}
			return RoomStorage.rooms[room];
		}

		public static void Init()
		{
			RoomStorage.rooms = new Dictionary<string, DungeonRoom>();
			string data_path = "/Resources/Data/";
			string fname = "Rooms.json";
			StreamReader reader = new StreamReader(Application.dataPath + data_path + fname);
			string contents = reader.ReadToEnd();
			reader.Close();

			JSONClass json = JSON.Parse(contents).AsObject;
			foreach (KeyValuePair<string, JSONNode> node in json)
			{
				rooms.Add(node.Key, new DungeonRoom(node.Value.AsObject));
			}
		}
	}

	public class DungeonRoom
	{
		public int width = 0;
		public int height = 0;
		public int centerx = -1;
		public int centery = -1;
		public char[,] room_mask;
		public List<string> possible_neighbors;
		public List<char> possible_exits;

		//Config
		public int max_neighbors = 4;
		public int min_neighbors = 2;
		public float treasure_chance = 0.2f;
		public float treasure_dr = 0.2f;
		public float monster_chance = 0.6f;
		public float monster_dr = 0.2f;

		public DungeonRoom (JSONClass json)
		{
			//Load from JSON
			width = json["width"].AsInt;
			height = json["height"].AsInt;
			possible_neighbors = new List<string>();


			room_mask = new char[height, width];
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					room_mask[y, x] = (char)json["room"][y].Value[x];
				}
			}
			foreach (JSONNode node in json["possible_neighbors"].AsArray)
			{
				possible_neighbors.Add(node.Value);
			}

			if (json["possible_exits"] != null) 
			{
				JSONArray ext = json["possible_exits"].AsArray;
				possible_exits = new List<char>();
				for (int i = 0; i < ext.Count; i++)
				{
					possible_exits.Add((char)ext[i].Value[0]);
				}
			}
			else 
			{
				possible_exits = new List<char> {'N', 'S', 'E', 'W'};
			}
			if (json["centerx"] != null && json["centery"] != null)
			{
				centerx = json["centerx"].AsInt;
				centery = json["centery"].AsInt;
			}
			if (json["max_neighbors"] != null) { max_neighbors = json["max_neighbors"].AsInt; }
			if (json["min_neighbors"] != null) { min_neighbors = json["min_neighbors"].AsInt; }
			if (json["treasure_chance"] != null) { treasure_chance = json["treasure_chance"].AsFloat; }
			if (json["treasure_dr"] != null) { treasure_dr = json["treasure_dr"].AsFloat; }
			if (json["monster_chance"] != null) { monster_chance = json["monster_chance"].AsFloat; }
			if (json["monster_dr"] != null) { monster_dr = json["monster_dr"].AsFloat; }
		}

		public char GetTile(int x, int y)
		{
			return room_mask[y, x];
		}

		public string GetRandomRoom()
		{
			return possible_neighbors[Random.Range(0, possible_neighbors.Count)];
		}

		public Vector2 GetCenter()
		{
			if (centerx >= 0)
			{
				return new Vector2(centerx, centery);
			}
			return new Vector2((int)width / 2, (int)height / 2);
		}
	}
}

