using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.TextServer;

namespace Anchor.Scripts
{
	public abstract partial class AnchorBase : RigidBody2D
	{
		public enum AnchorState { Idle, Thrown, Attached, Returning }

		[Export]
		public CollisionShape2D CollisionShape { get; protected set; }

		[Export]
		public Texture2D Texture { get; protected set; }

		[Export]
		public Sprite2D Sprite { get; set; }

		public Line2D Line { get; protected set; }

		public virtual float Speed => 175f;

		protected Vector2? _direction;

		protected AnchorState State = AnchorState.Idle;

		public bool IsOnWall { get; private set; }
#nullable enable
		public MainCharacter? _owner;

		public override void _Process(double delta)
		{
			if (State is AnchorState.Idle) return;

			if (Line.GetPointCount() > 1)
				Line.SetPointPosition(1, Position);

			else
				Line.AddPoint(Position);
		}

		public override void _Ready()
		{
			BodyEntered += OnCollision;
			Line = GetNode<Line2D>("/root/Root/Line");
		}

		protected virtual void OnCollision(Node body)
		{
			// 撞到物体后固定在墙上
			SetDeferred(PropertyName.Freeze, true);
			State = AnchorState.Attached;
			IsOnWall = true;
		}

		public void ThrowAtPositionLocalToParam(MainCharacter chara, Vector2 localPosition)
		{
			Freeze = false;
			var position = chara.Position;
			Position = position;
			GetParent()?.RemoveChild(this);
			chara.GetParent().AddChild(this);
			var directionVector = localPosition.Normalized();
			_direction = directionVector;
			Rotation = (float)(directionVector.Angle() -  (Math.PI / 180) * 90);
			State = AnchorState.Thrown;

			_owner = chara;

			ApplyImpulse(directionVector * Speed);
		}

		public override void _PhysicsProcess(double delta)
		{
			if (_owner is not null && (State == AnchorState.Thrown || State == AnchorState.Returning))
			{
				ApplyForce((_owner.Position - Position).Normalized() * 100);
			}
		}

		public virtual bool TryPullAnchor()
		{
			if (_owner is not null)
			{
				// 让锚放开并进入返回状态
				Freeze = false;
				State = AnchorState.Returning;
				return true;
			}

			else return false;
		}

		public virtual bool TryPullPlayer(MainCharacter chara, out Vector2 velocity)
		{
			// 默认不对玩家施加拉力，子类可重写
			velocity = default;
			return false;
		}

		public void Recover()
		{
			_owner = null;
			SetDeferred(PropertyName.Freeze, true);
			Rotation = 0;
			State = AnchorState.Idle;
			IsOnWall = false;
			if (Line.GetPointCount() > 1)
				Line.RemovePoint(1);
		}
	}
}
