using System.Collections.Generic;

namespace BattleNamespace
{
	public class Party
	{
		public static List<PlayerCharacter> party_list;

		public Party ()
		{

		}

		public static void Init()
		{
			if (party_list == null)
			{
				party_list = new List<PlayerCharacter>();

				for (int i = 0; i < 4; i++)
				{
					PlayerCharacter p = new PlayerCharacter();
					p.name = string.Format("Character {0}", i);
					party_list.Add(p);
				}
			}
		}
	}
}

