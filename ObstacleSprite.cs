using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Chicken_Duner
{
    public class ObstacleSprite : Sprite
    {
        private float _speed;
        
        // Hitbox tuning
        public Rectangle CollisionBox
        {
            get
            {
                Rectangle box = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
                box.Inflate(-10, -10); // Hitbox reduction
                return box;
            }
        }

        public ObstacleSprite(Texture2D texture, Vector2 position, float speed) : base(texture, position)
        {
            _speed = speed;
        }

        public void SetSpeed(float speed)
        {
            _speed = speed;
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            // Speed is typically pixels/frame or pixels/ms. 
            // Previous logic used (speed * deltaTime) or similar. 
            // If speed is ~6-13, and existing logic was: position.X -= speed * (dt/ something).
            // Let's assume passed speed is pixels/frame at 60fps, so adjust by deltaTime.
            
            // Standardizing: Speed is pixels per frame (approx).
            // DeltaTime factor: (deltaTime / 16.6f)
            
            Position.X -= _speed * (deltaTime / 16.6f);
        }
    }
}
