using Anchor.Scripts;
using Godot;
using System;

public partial class MainCharacter : CharacterBody2D
{
	public const float Speed = 100.0f;
	public const float JumpVelocity = -250.0f;

	[Export]
	public Line2D Line { get; set; }
#nullable enable
    public AnchorBase? Anchor { get; private set; }

	private bool _hasThrown;

    public override void _Ready()
    {
		// 阻塞式加载，先这样
		PickAnchor(ResourceLoader.Load<PackedScene>("res://Scenes/DefaultAnchor.tscn").Instantiate<AnchorBase>());
    }

    public override void _Process(double delta)
    {
		if (_hasThrown)
			Line.SetPointPosition(0, Position);
    }

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;

			velocity.Y = float.Clamp(float.Abs(velocity.Y), 0, 500) * float.Sign(velocity.Y);
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetAxis("ui_left", "ui_right") * Vector2.Right;

		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		if (Anchor is AnchorBase anchor)
		{
			if (_hasThrown)
			{
				if (Input.IsMouseButtonPressed(MouseButton.Left) && anchor.TryPull(this, out var _velocity))
					velocity += _velocity;
			}

            else if (Input.IsActionJustReleased(MouseLeftJustReleased))
            {
				anchor.ThrowAtMousePosition(this);
				_hasThrown = true;
            }
        }

		Velocity = velocity;
		MoveAndSlide();
	}

	public void PickAnchor(AnchorBase anchor)
	{
		Anchor = anchor;
		anchor.Position = new(8, 4);
		CallDeferred(Node.MethodName.AddChild, anchor);
		
    }

    public AnchorBase LeaveAnchor()
    {
        var anchor = Anchor ?? throw new Exception();
		Anchor = null;
		return anchor;
    }
}
