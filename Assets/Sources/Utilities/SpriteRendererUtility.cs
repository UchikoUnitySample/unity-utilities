using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRendererUtility : MonoBehaviourUtility
{
	SpriteRenderer _renderer = null;

	public SpriteRenderer Renderer {
		get {
			return _renderer ?? (_renderer = gameObject.GetComponent<SpriteRenderer> ());
		}
	}

	public bool Visible {
		get { return Renderer.enabled; }
		set { Renderer.enabled = value; }
	}

	public string SortingLayer {
		get { return Renderer.sortingLayerName; }
		set { Renderer.sortingLayerName = value; }
	}

	public int SortingOrder {
		get { return Renderer.sortingOrder; }
		set { Renderer.sortingOrder = value; }
	}

	public void SetSprite(Sprite sprite)
	{
		Renderer.sprite = sprite;
	}

	public void SetColor(float r, float g, float b)
	{
		var c = Renderer.color;
		c.r = r;
		c.g = g;
		c.b = b;
		Renderer.color = c;
	}

	public void SetColor(Color c)
	{
		Renderer.color = c;
	}

	public void SetAlpha(float a)
	{
		var c = Renderer.color;
		c.a = a;
		Renderer.color = c;
	}

	public float Alpha {
		get { return Renderer.color.a; }
		set { SetAlpha (value); }
	}

	public float Width {
		get { return Renderer.bounds.size.x; }
	}

	public float Height {
		get { return Renderer.bounds.size.y; }
	}
}
