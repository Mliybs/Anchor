using Godot;
using System.Linq;

public partial class Openning : Node2D
{
	[Export]
	public Parallax2D[] Backgrounds;

	[Export]
	public Car Car;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Backgrounds = [.. GetChildren().OfType<Parallax2D>()];
		Car = GetChildren().OfType<Car>().FirstOrDefault();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_accept"))
		{
			GD.Print(delta.ToString());
		}
		第一幕(delta);
	}

	public void 第一幕(double delta)
	{
		foreach (var background in Backgrounds)
		{
			background.ScrollOffset -= (float)delta * 100f * background.ScrollScale;
		}
		Car.Process(delta);
	}

}
