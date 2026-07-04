using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anchor.Scripts
{
	public abstract partial class AnchorBase : CharacterBody2D
	{
		[Export]
		public CollisionShape2D CollisionShape { get; protected set; }

		[Export]
		public Texture2D Texture { get; protected set; }

		public virtual float Speed => 250f;

		protected Vector2? _direction;

		public void ThrowAt(MainCharacter chara, Vector2 mousePosition)
		{
			CollisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
			chara.RemoveChild(this);
			chara.GetParent().AddChild(this);
			var position = chara.GlobalPosition;
			Position = position;
			var directionVector = (mousePosition - position).Normalized();
			_direction = directionVector;
			Rotation = (float)(directionVector.Angle() -  (Math.PI / 180) * 90);
		}

		public override void _PhysicsProcess(double delta)
		{
			if (_direction is Vector2 direction)
			{
				if (MoveAndCollide(direction * (float)(Speed * delta)) is { } collision)
				{
					_direction = null;
				}
			}
		}
	}
}
