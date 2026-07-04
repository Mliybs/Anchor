using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anchor.Scripts
{
    public abstract partial class AnchorBase : RigidBody2D
    {
        [Export]
        public CollisionShape2D CollisionShape { get; protected set; }

        [Export]
        public Texture2D Texture { get; protected set; }

        public Line2D Line { get; protected set; }

        public virtual float Speed => 75f;

        protected Vector2? _direction;

        protected bool _isThrown;

        public bool IsOnWall { get; private set; }

        public override void _Process(double delta)
        {
            if (!_isThrown) return;

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
            SetDeferred(PropertyName.Freeze, true);
            IsOnWall = true;
        }

        public void ThrowAtMousePosition(MainCharacter chara)
        {
            Freeze = false;
            chara.RemoveChild(this);
            chara.GetParent().AddChild(this);
            var position = chara.Position;
            Position = position;
            var directionVector = chara.GetLocalMousePosition().Normalized();
            _direction = directionVector;
            Rotation = (float)(directionVector.Angle() -  (Math.PI / 180) * 90);
            _isThrown = true;
        }

        public override void _PhysicsProcess(double delta)
        {
            if (_direction is Vector2 direction)
            {
                ApplyImpulse(direction * Speed);
                _direction = null;
            }
        }

        public virtual bool TryPull(MainCharacter chara, out Vector2 velocity)
        {
            velocity = default;
            return false;
        }
    }
}
