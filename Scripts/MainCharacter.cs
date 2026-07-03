using Anchor.Scripts;
using Godot;
using System;

#nullable enable
public partial class MainCharacter : CharacterBody2D
{
	public const float Speed = 100.0f;
	public const float JumpVelocity = -250.0f;
	public AnchorBase? Anchor { get; private set; }

    public override void _Ready()
    {
		// 阻塞式加载，先这样
		PickAnchor(ResourceLoader.Load<PackedScene>("res://Scenes/DefaultAnchor.tscn").Instantiate<AnchorBase>());
    }

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

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
            if (Input.IsActionJustReleased(MouseLeftJustReleased))
            {
				anchor.ThrowAt(this, GetViewport().GetMousePosition());
                LeaveAnchor();
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
		return anchor;
    }
}
