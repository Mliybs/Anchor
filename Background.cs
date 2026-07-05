using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class Background : TextureRect
{
	[Export]
	public Parallax2D[] Backgrounds;

	[Export]
	public MainCharacter Ako;

	public Vector2 BasePosition;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Backgrounds = [.. GetAllChildren(this).OfType<Parallax2D>()];
		//var parent = GetParent();
		//Ako = GetAllChildren(parent).OfType<MainCharacter>().FirstOrDefault();
		//BasePosition = Ako.Position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//var offset = Ako.Position - BasePosition;
		//offset.Y = -offset.Y;
		//foreach (Parallax2D bg in Backgrounds)
		//{
		//	var target = -offset * bg.ScrollScale;
		//	bg.ScrollOffset = bg.ScrollOffset.Lerp(target, 5f * (float)delta);
		//}
	}

	private static IEnumerable<Node> GetAllChildren(Node node)
	{
		foreach (Node child in node.GetChildren())
		{
			yield return child;
			foreach (Node sub in GetAllChildren(child))
				yield return sub;
		}
	}
}
