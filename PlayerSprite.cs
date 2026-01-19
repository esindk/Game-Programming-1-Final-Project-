using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Chicken_Duner
{
    public class PlayerSprite : Sprite
    {
        private const float GRAVITY = 1.2f;
        private const float JUMP_VELOCITY = -16.5f; // Jump Height Halved
        private const int GROUND_Y_POS_OFFSET = 10; // Alignment adjustment

        public float VerticalVelocity;
        public bool IsJumping;
        public bool IsDucking;
        public bool IsCrashed;

        private Dictionary<string, Texture2D> _textures;
        private int _floorY;
        private float _animationTimer;
        private int _currentFrame;

        // Hitbox tuning
        public Rectangle CollisionBox
        {
            get
            {
                Rectangle box = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
                // Adjust width for running/ducking if they are strips
                // Assuming simple textures for now or 2-frame strips
                if (IsRunning() || IsDucking) box.Width /= 2;
                
                box.Inflate(-15, -15); // Hitbox reduction
                return box;
            }
        }

        public PlayerSprite(Dictionary<string, Texture2D> textures, Vector2 position, int floorY) 
            : base(textures["idle"], position)
        {
            _textures = textures;
            _floorY = floorY;
            Position.Y = _floorY - Texture.Height; // Snap to ground
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Jumping Logic (AwsomeGame Style: Direct Calls)
            if ((Keyboard.GetState().IsKeyDown(Keys.Space) || Keyboard.GetState().IsKeyDown(Keys.Up)) && !IsJumping && !IsDucking)
            {
                VerticalVelocity = JUMP_VELOCITY;
                IsJumping = true;
                Texture = _textures["jump"];
            }

            // Ducking Logic
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && !IsJumping)
            {
                IsDucking = true;
                Texture = _textures["duck"];
            }
            else if (IsDucking && !Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                IsDucking = false;
                Texture = _textures["run"];
            }

            // Physics
            if (IsJumping)
            {
                Position.Y += VerticalVelocity;
                VerticalVelocity += GRAVITY;

                if (Position.Y >= _floorY - Texture.Height)
                {
                    Position.Y = _floorY - Texture.Height;
                    IsJumping = false;
                    VerticalVelocity = 0;
                    Texture = _textures["run"];
                }
            }
            else if (!IsDucking)
            {
                Texture = _textures["run"];
                Position.Y = _floorY - Texture.Height;
            }
            else // Ducking
            {
                Position.Y = _floorY - Texture.Height;
            }

            // Animation (Simple toggle for running/ducking)
            _animationTimer += deltaTime;
            if (_animationTimer > 100)
            {
                _animationTimer = 0;
                _currentFrame++;
            }
        }

        private bool IsRunning() => Texture == _textures["run"];

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Simple animation handling for 2-frame strips
            int width = Texture.Width;
            int height = Texture.Height;
            int sourceX = 0;

            if (IsRunning() || IsDucking)
            {
               width /= 2;
               sourceX = (_currentFrame % 2) * width;
            }

            Rectangle sourceRect = new Rectangle(sourceX, 0, width, height);
            spriteBatch.Draw(Texture, Position, sourceRect, Color);
        }
    }
}
