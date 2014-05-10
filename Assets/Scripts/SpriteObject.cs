using UnityEngine;
using DungeonNamespace;
using System;

namespace SpriteObj
{
	public class SpriteObject
	{
		public GameObject obj;
		private SpriteRenderer sr;

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

		public void SetSprite(Sprite spr)
		{
			sr.sprite = spr;
		}

		public void SetPosition(float x, float y)
		{
			obj.transform.position = new Vector3(x, y, obj.transform.position.z);
		}

		public void SetLayer(float z)
		{
			obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, z);
		}

		public void SetScale(float x_scale, float y_scale)
		{
			obj.transform.localScale = new Vector3(x_scale, y_scale, 1);
		}

		public void SetAlpha(float alpha)
		{
			sr.color = new Color(1f, 1f, 1f, alpha);
		}

		public void Delete()
		{

		}
	}

	public class Nametag : SpriteObject
	{
		public Nametag()
		{
			GameObject loaded = Resources.Load<GameObject>("Prefabs/Nametag");
			this.obj = (GameObject)GameObject.Instantiate(loaded);
			SetMessageDetails("", 0);
			SetLayer(-5);
		}

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
			t.transform.localScale = new Vector3(1, xscale / yscale, 1);
		}

		public void SetText(string text)
		{
			GameObject t = obj.transform.Find("Text").gameObject;
			t.GetComponent<TextMesh>().text = text;
		}
	}
}

