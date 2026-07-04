using Anchor.Scripts;
using Godot;
using System;

public partial class Car : CharacterBody2D
{
	public const float Speed = 30.0f;
	private float baseY;        // 原始高度
	private bool jumpUp = false;

	public AnchorBase Anchor { get; set; }

	public Sprite2D Sprite;
	public override void _Ready()
	{
		baseY = Position.Y;
		Sprite = GetNode<Sprite2D>("Sprite2D");
	}
	public void 第一幕(double delta)
	{
		// 永远只前进，不改Y速度
		Position += new Vector2(Speed * (float)delta, 0);

		// 随机触发“上抬”
		if (Random.Shared.Next(0, 100) < 1)
		{
			jumpUp = true;
		}

		// 上抬2像素
		if (jumpUp)
		{
			Position = new Vector2(Position.X, baseY - 1);
			jumpUp = false; // 只持续一帧
		}
		else
		{
			// 下一帧回到原位
			Position = new Vector2(Position.X, baseY);
		}
	}

	public void 开始第二幕()
	{
	}
	public void 第二幕(double delta)
	{
		Sprite.Rotation += 50f * (float)delta;
	}
}


public static class AnchorExtensions
{
	public static void ThrowToPosition(this AnchorBase anchor, CharacterBody2D chara, Vector2 position)
	{
		anchor.Freeze = false;
		anchor.GetParent()?.RemoveChild(anchor);
		chara.GetParent().AddChild(anchor);
		anchor.Position = position;
		var directionVector = new Vector2(-1, 1);
		anchor.Rotation = (float)(directionVector.Angle() - (Math.PI / 180) * 90);
	}
}
