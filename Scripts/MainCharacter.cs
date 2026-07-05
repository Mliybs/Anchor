using Anchor.Scripts;
using Godot;
using System;
using System.Threading.Tasks;

public partial class MainCharacter : CharacterBody2D
{
	public const float Speed = 100.0f;
	public const float JumpVelocity = -250.0f;

	[Export]
	public Line2D Line { get; set; }
	[Export]
	public Area2D Area { get; set; }
	[Export]
	public AnimatedSprite2D Sprite { get; set; }
#nullable enable
	public AnchorBase? Anchor { get; private set; }

	private enum PlayerAnchorState { None, Attached, Thrown, Pulling }
	private PlayerAnchorState _anchorState = PlayerAnchorState.None;
	private bool _shouldSuppressThrow;

	public override void _Ready()
	{
		// 阻塞式加载，先这样
		PickAnchor(ResourceLoader.Load<PackedScene>("res://Scenes/DefaultAnchor.tscn").Instantiate<AnchorBase>());
		Sprite.Play("idle");
	}

	public override void _Process(double delta)
	{
		if (_anchorState is PlayerAnchorState.Thrown or PlayerAnchorState.Pulling)
			Line.SetPointPosition(0, Position + new Vector2(Sprite.FlipH ? 10 : -10, -7));
	}

	public override async void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			var signA = float.Sign(velocity.Y);
			velocity += GetGravity() * (float)delta;

			velocity.Y = float.Clamp(float.Abs(velocity.Y), 0, 500) * float.Sign(velocity.Y);
			var signB = float.Sign(velocity.Y);

			if (signA != signB && signA + signB == 0)
				Sprite.Play("fall");
		}

		else if (_anchorState is not PlayerAnchorState.Pulling && Sprite.Animation == "fall")
		{
			Sprite.Play("grounded");
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			Sprite.Play("jump");
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetAxis("ui_left", "ui_right") * Vector2.Right;

		var isPulling = false;

		if (Anchor is AnchorBase anchor)
		{
			switch (_anchorState)
			{
				case PlayerAnchorState.Thrown:
					if (Input.IsActionJustReleased(MouseRightJustReleased) && anchor.IsOnWall)
					{
						anchor.TryPullAnchor();
						_anchorState = PlayerAnchorState.Pulling;
					}

					if (Input.IsMouseButtonPressed(MouseButton.Left) && (isPulling = anchor.TryPullPlayer(this, out var _velocity)))
					{
						velocity += _velocity;
						_anchorState = PlayerAnchorState.Pulling;
					}

					break;

				case PlayerAnchorState.Attached:
					if (Input.IsActionJustReleased(MouseLeftJustReleased))
					{
						if (_shouldSuppressThrow) _shouldSuppressThrow = false;
						else
						{
							anchor.ThrowAtPositionLocalToParam(this, GetLocalMousePosition());
							_anchorState = PlayerAnchorState.Thrown;

							await Task.Delay(100);
							Area.Monitoring = true;
						}
					}

					break;

				case PlayerAnchorState.Pulling:
					// 保持拉拽时的行为：允许 Anchor.TryPullPlayer 在上面应用速度
					if (Input.IsMouseButtonPressed(MouseButton.Left) && (isPulling = anchor.TryPullPlayer(this, out var _v2)))
					{
						velocity += _v2;
					}
					else
					{
						// 当停止按下左键时，回到 Thrown 状态以允许再次操作
						if (!Input.IsMouseButtonPressed(MouseButton.Left))
							_anchorState = PlayerAnchorState.Thrown;
					}

					break;

				default:
					break;
			}
		}

		if (!isPulling)
		{
			if (direction != Vector2.Zero)
			{
				velocity.X = direction.X * Speed;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			}
		}

		if (velocity.X != 0)
		{
			Sprite.FlipH = velocity.X < 0;
			if (_anchorState is PlayerAnchorState.Attached)
				Anchor?.Position = Anchor.Position with { X = float.Abs(Anchor.Position.X) * (velocity.X < 0 ? 1 : -1 ) };
		}

		if (!(Sprite.IsPlaying() && Sprite.Animation == "grounded"))
		{
			if (Velocity is (0, 0))
			{
				Sprite.Play("idle");
			}

			else if (Velocity is (not 0, 0))
			{
				Sprite.Play("walk");
			}
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public void PickAnchor(AnchorBase anchor)
	{
		Anchor = anchor;
		anchor.SetDeferred(Node2D.PropertyName.Position, new Vector2(-13, -1));
		CallDeferred(MethodName.AddChild, anchor);
		Area.Monitoring = false;
		_anchorState = PlayerAnchorState.Attached;
	}

	public AnchorBase LeaveAnchor()
	{
		var anchor = Anchor ?? throw new Exception();
		Anchor = null;
		_anchorState = PlayerAnchorState.None;
		return anchor;
	}

	public void OnAnchorRecovery(Node2D node)
	{
		if (node is AnchorBase anchor && (anchor.IsOnWall || (anchor.LinearVelocity.Dot(Position - anchor.Position) > 0) && anchor == Anchor))
		{
			Area.SetDeferred(Area2D.PropertyName.Monitoring, false);
			_shouldSuppressThrow = _anchorState is PlayerAnchorState.Pulling;
			anchor.Recover();
			anchor.SetDeferred(AnchorBase.PropertyName.Position, new Vector2(-13, -1));
			anchor.CallDeferred(MethodName.Reparent, this, false);
			_anchorState = PlayerAnchorState.Attached;
		}
	}
}
