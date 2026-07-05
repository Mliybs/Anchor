using Godot;
using System.Threading.Tasks;

namespace Anchor.Scripts
{
	[GlobalClass]
	public partial class Door : Sprite2D
	{
		[Export]
		public string TargetScenePath { get; set; }

		[Export]
		public Area2D Area2D { get; set; }

		[Export]
		public CollisionShape2D CollisionShape { get; set; }
		private bool _isTransitioning = false;

		public override void _Ready()
		{
			Area2D.Monitoring = true;
			Area2D.Monitorable = true;
			// 连接 body_entered 信号
			Area2D.BodyEntered += OnBodyEntered;
			base._Ready();
			
		}

		private void OnBodyEntered(Node2D body)
		{
			// 判断进入的是否是玩家（通过类型或组）
			if (body is MainCharacter)
			{
				if (!_isTransitioning)
				{
					SwitchScene();
				}
			}
		}

		private void SwitchScene()
		{
			if (_isTransitioning) return;
			_isTransitioning = true;

			// 检查目标路径是否有效
			if (string.IsNullOrEmpty(TargetScenePath) || !ResourceLoader.Exists(TargetScenePath))
			{
				GD.PrintErr($"错误: 目标场景路径无效或不存在: {TargetScenePath}");
				_isTransitioning = false;
				return;
			}

			// 切换场景（Godot 4.2+ 推荐方式）
			GetTree().ChangeSceneToFile(TargetScenePath);
		}
	}
}
