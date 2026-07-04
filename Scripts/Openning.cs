using Godot;
using System;
using System.Linq;

public partial class Openning : Node2D
{
	[Export]
	public Parallax2D[] Backgrounds;

	[Export]
	public Car Car;

	[Export]
	public MainCharacter Ako;

	DateTime? FirstTime = null;

	Func<double, int, int>[] 台本;

	Camera2D camera;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Backgrounds = [.. GetChildren().OfType<Parallax2D>()];
		Car = GetChildren().OfType<Car>().FirstOrDefault();
		FirstTime ??= DateTime.Now;
		台本 = [第一幕, 开始第二幕, 第二幕, 第三幕, 第四幕];
		camera = GetChildren().OfType<Camera2D>().FirstOrDefault();
	}

	int 幕 = 0;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_accept"))
		{
			GD.Print(DateTime.Now - FirstTime);
		}
		if (幕 < 台本.Length)
		{
			幕 = 台本[幕](delta, 幕);
		}
	}

	public int 第一幕(double delta, int 幕)
	{
		GD.Print($"Car 节点名称: {Car.Name}");
		GD.Print($"Car 是否在场景树中: {Car.IsInsideTree()}");
		GD.Print($"Car 本地位置: {Car.Position}");
		GD.Print($"Car 全局位置: {Car.GlobalPosition}");
		GD.Print($"Car Sprite本地位置: {Car.Sprite.Position}");
		GD.Print($"Car Sprite全局位置: {Car.Sprite.GlobalPosition}");
		GD.Print($"Car 父节点: {Car.GetParent()?.Name}");
		//汽车向前开，背景滚动
		foreach (var background in Backgrounds)
		{
			background.ScrollOffset -= (float)delta * 100f * background.ScrollScale;
		}
		Car.第一幕(delta);
		if (DateTime.Now - FirstTime > TimeSpan.FromSeconds(2))
		{
			return 幕 + 1;
		}
		return 幕;
	}

	public int 开始第二幕(double delta, int 幕)
	{
		Car.开始第二幕();
		return 幕 + 1;
	}
	public int 第二幕(double delta, int 幕)
	{
		//某种神必力量让车停在了原地，背景停止滚动
		Car.第二幕(delta);
		if (Car.Sprite.Rotation > 3.14)
		{
			FirstTime = DateTime.Now;
			return 幕 + 1;
		}
		return 幕;
	}
	private float shakeTime = 2f;
	private float shakePower = 5f;
	public int 第三幕(double delta, int 幕)
	{
		if (shakeTime > 0)
		{
			shakeTime -= (float)delta;

			camera.Offset = new Vector2(
				(float)GD.RandRange(-shakePower, shakePower),
				(float)GD.RandRange(-shakePower, shakePower)
			);
			shakePower *= 0.9f; // 减小震动幅度
		}
		else
		{
			camera.Offset = Vector2.Zero;
		}
		if (shakePower < 0.1f)
		{
			return 幕 + 1;
		}
		return 幕;
	}
	public int 第四幕(double delta, int 幕)
	{
		var characterScene = ResourceLoader.Load<PackedScene>("res://Scenes/MainChara.tscn");
		Ako = characterScene.Instantiate<MainCharacter>();

		Ako.Position = Car.Position - new Vector2(0, 5);
		AddChild(Ako);
		return 幕 + 1;
	}
}
