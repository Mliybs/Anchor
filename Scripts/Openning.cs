using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class Openning : Node2D
{
	[Export]
	public Parallax2D[] Backgrounds;

	[Export]
	public Car Car;

	[Export]
	public MainCharacter Ako;

	[Export]
	public Sprite2D Title;

	DateTime? FirstTime = null;

	Func<double, int, int>[] 台本;

	Camera2D camera;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Backgrounds = [.. GetChildren().OfType<Parallax2D>()];
		Car = GetChildren().OfType<Car>().FirstOrDefault();
		FirstTime ??= DateTime.Now;
		台本 = [标题第一幕, 标题第二幕, 标题第三幕, 标题第四幕, 第一幕, 开始第二幕, 第二幕, 第三幕, 第四幕, 第五幕, 第六幕, 第七幕];
		camera = GetChildren().OfType<Camera2D>().FirstOrDefault();
		Title = GetNode<Sprite2D>("Title");
		Color color = Title.Modulate;
		color.A = 0;
		Title.Modulate = color;
	}

	int 幕 = 0;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (幕 < 台本.Length)
		{
			if (Input.IsActionJustPressed("ui_accept"))
			{
				第七幕(delta, 幕);
				return;
			}
			幕 = 台本[幕](delta, 幕);
		}
	}
	float carOriginY;
	public int 标题第一幕(double delta, int 幕)
	{
		float offset = 100;
		carOriginY = Car.Position.Y;
		for (int i = 0; i < Backgrounds.Length; i++)
		{
			Parallax2D background = Backgrounds[i];
			background.Position += new Vector2(0, 100 - offset);
			offset = offset > 30 ? offset - 30 : 0;
		}
		Car.Position = new Vector2(Car.Position.X, carOriginY + 100);
		return 幕 + 1;
	}
#nullable enable
	Label? staff;
	float staffAlpha = 0f;
	public int 标题第二幕(double delta, int 幕)
	{
		if (staff == null)
		{
			staff = new Label();

			// 1. 准备数据
			string programer = "程序员：jirigalang\n";
			string programer2 = "程序员：Mliybs翠鸟\n";
			string art = "美术：文丶丶叔\n";
			string art2 = "美术：foo1\n";

			List<string> list = new List<string> { programer, programer2, art, art2 };

			// 2. 随机打乱顺序（正确方法）
			Random rng = new Random();
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = rng.Next(n + 1);
				(list[n], list[k]) = (list[k], list[n]);
			}

			// 3. 拼接字符串
			StringBuilder sb = new StringBuilder();
			foreach (string item in list)
			{
				sb.Append(item);
			}
			staff.Text = sb.ToString();


			AddChild(staff);
		}

		// 5. 更新位置（必须在添加到树后）
		ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		float x = (GetViewportRect().Size.X - staff.GetRect().Size.X) / 2;
		staff.Position = new Vector2(x, 10);

		// 4. 设置透明度（正确的格式）
		staff.Modulate = new Color(0, 0x30, 0x30, staffAlpha);

		if (staffAlpha < 1f)
		{
			staffAlpha += (float)delta * 2f;
			return 幕;
		}
		FirstTime = DateTime.Now;
		return 幕 + 1;
	}

	public int 标题第三幕(double delta, int 幕)
	{
		if (DateTime.Now - FirstTime < TimeSpan.FromSeconds(2)) return 幕;
		staff!.Modulate = new Color(0, 0x30, 0x30, staffAlpha);

		if (staffAlpha > 0)
		{
			staffAlpha -= (float)delta * 2f;
		}
		float duration = 1.0f; // 全部用2秒完成

		for (int i = 0; i < Backgrounds.Length; i++)
		{
			Parallax2D background = Backgrounds[i];

			float distance = background.Position.Y;

			if (distance <= 0.1f)
				continue;

			float speed = distance / duration;

			float newY = background.Position.Y - speed * (float)delta;


			if (newY < 1)
				newY = 0;

			background.Position = new Vector2(background.Position.X, newY);
		}
		Car.Position = new Vector2(Car.Position.X, carOriginY + Backgrounds[4].Position.Y);
		if (Backgrounds.All(b => b.Position.Y <= 0.1f) && staffAlpha <= 0)
		{
			RemoveChild(staff);
			staff = new()
			{
				Text = "按下空格键开始游戏",
				Modulate = new Color(0x30, 0, 0x30, staffAlpha),
			};
			AddChild(staff);
			return 幕 + 1;
		}
		else
		{
			return 幕;
		}
	}
	public int 标题第四幕(double delta, int 幕)
	{
		if (staff!.Modulate.A < 1f)
		{
			staffAlpha += (float)delta * 10f;
			staff.Modulate = new Color(0, 0x30, 0x30, staffAlpha);
			Color color = Title.Modulate;
			color.A = staffAlpha;
			Title.Modulate = color;
		}
		else
		{
			foreach (var background in Backgrounds)
			{
				background.ScrollOffset -= (float)delta * 100f * background.ScrollScale;
			}
		}
		Car.颠簸(delta);
		staff.Position = new Vector2((GetViewportRect().Size.X - staff.GetRect().Size.X) / 2 + 80, (GetViewportRect().Size.Y - staff.GetRect().Size.Y) / 2 + 30);
		if (Input.IsActionJustPressed("ui_accept") || Input.IsActionJustPressed(MouseLeftJustReleased))
		{
			FirstTime = DateTime.Now;
			RemoveChild(staff);
			RemoveChild(Title);
			return 幕 + 1;
		}
		else
		{
			return 幕;
		}
	}

	public int 第一幕(double delta, int 幕)
	{
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
			FirstTime = DateTime.Now;
			return 幕 + 1;
		}
		return 幕;
	}
	public int 第四幕(double delta, int 幕)
	{
		if (DateTime.Now - FirstTime < TimeSpan.FromSeconds(2)) return 幕;

		var characterScene = ResourceLoader.Load<PackedScene>("res://Scenes/MainChara.tscn");
		Ako = characterScene.Instantiate<MainCharacter>();

		Ako.Position = Car.Position - new Vector2(0, 10);
		AddChild(Ako);
		return 幕 + 1;
	}

	public int 第五幕(double delta, int 幕)
	{
		Ako.Velocity = new Vector2(80, 0);
		Ako.MoveAndSlide();
		if (Ako.Position.X - Car.Position.X < 40)
		{
			return 幕;
		}
		Ako.Velocity = new Vector2(-1, 0);
		return 幕 + 1;
	}

	public int 第六幕(double delta, int 幕)
	{
		if (DateTime.Now - FirstTime < TimeSpan.FromSeconds(4)) return 幕;
		Ako.Velocity = new Vector2(80, 0);
		Ako.MoveAndSlide();
		if (Ako.Position.X > GetViewportRect().Size.X)
		{
			return 幕 + 1;
		}
		else
		{
			return 幕;
		}
	}

	public int 第七幕(double delta, int 幕)
	{
		GetTree().ChangeSceneToFile(@"res://Scenes/stage1.tscn");
		return 幕 + 1;
	}
}
