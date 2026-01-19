using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Chicken_Duner
{
    public abstract class Sprite
    {
        public Vector2 Position;
        public Texture2D Texture;
        public Color Color = Color.White;
        public Rectangle Rectangle => new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

        public Sprite(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color);
        }
    }
}
