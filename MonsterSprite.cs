using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Chicken_Duner
{
    public class MonsterSprite : Sprite
    {
        private float _startY;
        private float _bobTimer;
        private float _flipTimer;
        private bool _flipped;

        public MonsterSprite(Texture2D texture, Vector2 position) : base(texture, position)
        {
            _startY = position.Y;
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Bobbing
            _bobTimer += deltaTime * 5f;
            Position.Y = _startY + (float)Math.Sin(_bobTimer) * 10f;

            // Glitchy Flip
            _flipTimer += deltaTime;
            if (_flipTimer > 0.2f)
            {
                _flipTimer = 0;
                _flipped = !_flipped;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            SpriteEffects effect = _flipped ? SpriteEffects.FlipVertically : SpriteEffects.None;
            spriteBatch.Draw(Texture, Position, null, Color, 0f, Vector2.Zero, 1f, effect, 0f);
        }
    }
}
