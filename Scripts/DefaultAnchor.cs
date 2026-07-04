using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anchor.Scripts
{
    public partial class DefaultAnchor : AnchorBase
    {
        public const float PullForce = 0.5f;
        public const float Min = 100f;
        public const float Max = 300f;

        public override bool TryPullPlayer(MainCharacter chara, out Vector2 velocity)
        {
            if (!IsOnWall)
            {
                velocity = default;
                return false;
            }

            var direction = Position - chara.Position; // 不要归一化向量，保留大小信息
            if (direction.Length() < Min) direction = direction.Normalized() * Min;
            else direction = direction.LimitLength(Max);
            velocity = direction * PullForce; 
            return true;
        }
    }
}
