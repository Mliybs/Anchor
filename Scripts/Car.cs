using Godot;
using System;

public partial class Car : CharacterBody2D
{
	public const float Speed = 30.0f;
	private float baseY;        // 原始高度
	private bool jumpUp = false;
	public override void _Ready()
	{
		baseY = Position.Y;
	}
	public void Process(double delta)
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
}
