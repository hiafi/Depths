using System.Collections.Generic;
using UnityEngine;
using SpriteObj;


public class Effect
{
	public string name;
	public bool positive;
	public int duration;
	public string effect;
	public int amount;
	public string type;
}

public class Ability
{
	public string name;
	public string damage_type = "slash";
	public bool uses_weapon = false;
	public int num_attacks = 1;
	public int base_damage = 10;
	public string scaling_attribute = "mind";
	public float scaling_value = 1.0f;
	public float hit_chance = 0.0f;
	public float crit_chance = 0.0f; //This is added to your base crit
	public float crit_power = 1.5f;
	
	public List<Effect> effects;
	
	public void AddEffect(Effect e)
	{
		if (effects == null)
		{
			effects = new List<Effect>();
		}
		effects.Add(e);
	}
	
	public void HandleAbility(Actor source, Actor target)
	{
		target.TakeDamage(GetDamageValue(source), damage_type);
	}
	
	public void HandleMassAbility(Actor source, List<Actor> targets)
	{
		foreach (Actor a in targets)
		{
			HandleAbility(source, a);
		}
	}
	
	public void HandleMassAbility(Actor source, List<Actor> targets, Actor optional_target)
	{
		
	}
	
	public int GetDamageValue(Actor caster)
	{
		return base_damage + (int)(caster.attributes[scaling_attribute] * scaling_value);
	}
}