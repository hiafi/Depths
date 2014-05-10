using System.Collections.Generic;
using System;


public class Effect
{
	public string name;
	public bool positive;
	public int duration;
	public string effect;
	public int amount;
	public string type;
}

public class Actor
{
	public string name;
	
	public int hp;
	public int max_hp;
	public int mp;
	public int max_mp;

	public Dictionary<string, float> attributes;

	public bool can_act = true;
	public bool can_attack = true;

	protected List<Effect> effects;


	public Actor ()
	{
		attributes = new Dictionary<string, float>();
		effects = new List<Effect>();
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
			int dmg = (int)(amount * (1 - GetResist(type)));
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
}

public class PlayerCharacter : Actor
{
	public string cls;

	public PlayerCharacter() : base()
	{

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
