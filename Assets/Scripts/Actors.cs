using System.Collections.Generic;
using System;

using UnityEngine;






public class Actor
{
	public string name;
	
	public int hp = 10;
	public int max_hp = 10;
	public int mp = 10;
	public int max_mp = 10;

	public Dictionary<string, float> attributes;

	public bool can_act = true;
	public bool can_attack = true;

	protected List<Effect> effects;

	public List<Ability> abilities;
	protected Ability attack_ability;

	public Actor ()
	{
		attributes = new Dictionary<string, float>();
		effects = new List<Effect>();
		abilities = new List<Ability>();

		attributes["strength"] = 0;
		attributes["dexterity"] = 0;
		attributes["constitution"] = 0;
		attributes["mind"] = 0;
		attributes["intuition"] = 0;
		attributes["composure"] = 0;

		attack_ability = new Ability();
		attack_ability.name = "Attack";
		attack_ability.scaling_attribute = "strength";
	}

	public float GetResist(string name)
	{
		return 0.0f;
	}

	public void TakeDamage(int amount, string type)
	{
		if (GetResist(type) > 1.0f)
		{
			HealDamage((int)(amount * (GetResist(type) - 1.0f)));
		}
		else
		{
			int dmg = (int)(amount * (1.0 - GetResist(type)));
			foreach (Effect e in effects)
			{
				if (e.effect == "shield")
				{
					if (e.amount < dmg)
					{
						e.amount = 0;
						dmg -= e.amount;
					}
					else
					{
						e.amount -= dmg;
					}
				}
			}
			hp -= dmg;
		}
	}

	public void HealDamage(int amount)
	{
		hp = Math.Min(max_hp, hp + amount);
	}

	public void HandleEffects()
	{
		can_act = true;
		can_attack = true;
		foreach (Effect e in effects)
		{

			switch (e.effect)
			{
			case "damage":
				TakeDamage(e.amount, e.type);
				break;
			case "heal":
				HealDamage(e.amount);
				break;
			}

			e.duration -= 1;
			if (e.duration <= 0)
			{
				effects.Remove(e);	
			}
		}
	}

	public void AddEffect(Effect e)
	{
		effects.Add(e);
	}

	public Ability GetAttack()
	{
		return attack_ability;
	}

	public int GetShield()
	{
		int s = 0;
		foreach (Effect e in effects)
		{
			if (e.effect == "shield")
			{
				s += e.amount;
			}
		}
		return s;
	}
}

public class PlayerCharacter : Actor
{
	public string cls;

	public PlayerCharacter() : base()
	{
		attack_ability.uses_weapon = true;
	}
}

public class Monster : Actor
{

}

public class MonsterGroup
{
	public List<Monster> monsters;

	public MonsterGroup()
	{
		monsters = new List<Monster>();
	}
}
