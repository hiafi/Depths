using UnityEngine;
using DungeonNamespace;
using System;

namespace SpriteObj
{
	public class AbstractDraw
	{
		public GameObject obj;

		public void SetPosition(float x, float y)
		{
			obj.transform.position = new Vector3(x, y, obj.transform.position.z);
		}

		public Vector3 GetPosition()
		{
			return obj.transform.position;
		}
		
		public void SetLayer(float z)
		{
			obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, z);
		}
		
		public void SetScale(float x_scale, float y_scale)
		{
			obj.transform.localScale = new Vector3(x_scale, y_scale, 1);
		}

		public void Delete()
		{
			GameObject.Destroy(obj);
		}

		public void SetTimedLife(float time)
		{
			GameObject.Destroy(obj, time);
		}
	}

	public class AbstractSpriteObject : AbstractDraw
	{
		protected SpriteRenderer sr;

		public void SetSprite(Sprite spr)
		{
			sr.sprite = spr;
		}

		public void SetColor(float r, float g, float b)
		{
			sr.color = new Color(r, g, b, sr.color.a);
		}

		public void SetColor(float r, float g, float b, float a)
		{
			sr.color = new Color(r, g, b, a);
		}

		public void SetAlpha(float alpha)
		{
			sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
		}
	}

	public class FloatingText : AbstractDraw
	{
		protected TextMesh tm;
		public FloatingText()
		{
			GameObject loaded = Resources.Load<GameObject>("Prefabs/FloatingText");
			this.obj = (GameObject)GameObject.Instantiate(loaded);
			this.tm = ((TextMesh)obj.GetComponent("TextMesh"));
			SetMovePosition(obj.transform.position.x, obj.transform.position.y, 1.0f);
		}

		public void SetTextColor(Color color)
		{
			tm.color = color;
		}

		public void SetText(string text)
		{
			tm.text = text;
		}

		public void SetMovePosition(float x, float y, float time)
		{
			FloatingTextBehavior scr = this.obj.GetComponent<FloatingTextBehavior>();
			scr.end_pos = new Vector3(x, y, obj.transform.position.z);
			scr.move_time = time;
		}

	}

	public class SpriteObject : AbstractSpriteObject
	{
		public SpriteObject ()
		{
			GameObject loaded = Resources.Load<GameObject>("Prefabs/SprObj");
			this.obj = (GameObject)GameObject.Instantiate(loaded);
			this.sr = ((SpriteRenderer)obj.GetComponent("SpriteRenderer"));
		}
		
		public SpriteObject(string path) : this()
		{
			SetSprite(Resources.Load<Sprite>(String.Format("Sprites/{0}", path)));
		}
		
		public SpriteObject(Sprite spr) : this()
		{
			SetSprite(spr);
		}
	}

	public class AbstractButton : AbstractSpriteObject
	{

		public void SetMessageDetails(string type, int index)
		{
			InteractableSprite scr = this.obj.GetComponent<InteractableSprite>();
			scr.type = type;
			scr.index = index;
		}
		
		public void SetScale(float xscale, float yscale)
		{
			obj.transform.localScale = new Vector3(xscale, yscale, 1);
			GameObject t = obj.transform.Find("Text").gameObject;
			t.transform.localScale = new Vector3(.5f, 0.5f * xscale / yscale, 1);
		}
		
		public void SetText(string text)
		{
			GameObject t = obj.transform.Find("Text").gameObject;
			t.GetComponent<TextMesh>().text = text;
		}
	}

	public class Button : AbstractButton
	{
		public Button()
		{
			GameObject loaded = Resources.Load<GameObject>("Prefabs/Button");
			this.obj = (GameObject)GameObject.Instantiate(loaded);
			SetMessageDetails("", 0);
			SetLayer(-5);
		}
	}

	public class Nametag : AbstractButton
	{
		public float health = 1.0f;
		public float energy = 1.0f;
		public float shield = 0.0f;
		public Actor actor;

		public Nametag(Actor actor)
		{
			GameObject loaded = Resources.Load<GameObject>("Prefabs/Nametag");
			this.obj = (GameObject)GameObject.Instantiate(loaded);
			SetMessageDetails("", 0);
			SetLayer(-5);
			this.actor = actor;
		}

		public void Update()
		{
			health = Mathf.Lerp(health, ((float)actor.hp / (float)actor.max_hp), 0.05f);
			energy = Mathf.Lerp(energy, ((float)actor.hp / (float)actor.max_hp), 0.05f);
			shield = Mathf.Lerp(shield, Math.Min(1.0f, ((float)actor.GetShield() / (float)actor.max_hp)), 0.05f);

			SetHealth();
			SetEnergy();
			SetShield();
		}

		private void SetHealth()
		{
			GameObject t = obj.transform.Find("HealthBar").gameObject;
			t.transform.localScale = new Vector3(health, t.transform.localScale.y, t.transform.localScale.z);
		}

		private void SetEnergy()
		{
			GameObject t = obj.transform.Find("EnergyBar").gameObject;
			t.transform.localScale = new Vector3(energy, t.transform.localScale.y, t.transform.localScale.z);
		}

		private void SetShield()
		{
			GameObject t = obj.transform.Find("ShieldBar").gameObject;
			t.transform.localScale = new Vector3(shield, t.transform.localScale.y, t.transform.localScale.z);
		}
	}
}