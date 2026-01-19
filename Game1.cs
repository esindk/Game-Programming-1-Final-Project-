using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace Chicken_Duner
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Assets
        private Texture2D _groundTexture;
        private Texture2D _monsterTexture;
        private Texture2D _cactusTexture;
        private Texture2D _bonesTexture;
        private Texture2D _backgroundTexture; // Added Background
        private SpriteFont _font;
        private Dictionary<string, Texture2D> _playerTextures;

        // Entities
        private PlayerSprite _player;
        private MonsterSprite _monster;
        private List<ObstacleSprite> _obstacles;

        // Game State
        private float _speed = 6f;
        private const float MAX_SPEED = 13f;
        private float _score;
        private float _highScore;
        private bool _gameOver;
        private float _groundOffset;
        private Random _random;
        private float _spawnTimer;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 800; 
            _graphics.PreferredBackBufferHeight = 450;
        }

        protected override void Initialize()
        {
            _random = new Random();
            _obstacles = new List<ObstacleSprite>();
            LoadHighScore();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load Textures
            _groundTexture = Content.Load<Texture2D>("Sprites/Environment/ground");
            _monsterTexture = Content.Load<Texture2D>("Sprites/monster");
            _cactusTexture = Content.Load<Texture2D>("Sprites/Obstacles/cactus");
            _bonesTexture = Content.Load<Texture2D>("Sprites/Obstacles/bones");
            _backgroundTexture = Content.Load<Texture2D>("Sprites/Environment/background"); // Load Background
            _font = Content.Load<SpriteFont>("Fonts/ScoreFont");

            _playerTextures = new Dictionary<string, Texture2D>
            {
                { "idle", Content.Load<Texture2D>("Sprites/Player/player_idle") },
                { "run", Content.Load<Texture2D>("Sprites/Player/player_run") },
                { "duck", Content.Load<Texture2D>("Sprites/Player/player_duck") },
                { "jump", Content.Load<Texture2D>("Sprites/Player/player_jump") },
                { "crash", Content.Load<Texture2D>("Sprites/Player/player_crash") }
            };

            ResetGame();
        }

        private void ResetGame()
        {
            int floorY = GraphicsDevice.Viewport.Height - 50;
            _player = new PlayerSprite(_playerTextures, new Vector2(250, floorY), floorY); 
            _monster = new MonsterSprite(_monsterTexture, new Vector2(0, floorY - _monsterTexture.Height + 20)); 
            _obstacles.Clear();
            _score = 0;
            _speed = 6f;
            _gameOver = false;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_gameOver)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space) || Keyboard.GetState().IsKeyDown(Keys.Enter))
                    ResetGame();
                return;
            }

            // Inputs / Player Update
            _player.Update(gameTime);
            _monster.Update(gameTime);

            // Ground Scrolling
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            _groundOffset -= _speed * (deltaTime / 16.6f);
            if (_groundOffset <= -_groundTexture.Width) _groundOffset = 0;

            // Speed Acceleration
            if (_speed < MAX_SPEED)
                _speed += 0.001f * deltaTime;

            // Score
            _score += (_speed * deltaTime) * 0.01f;
            if (_score > _highScore) _highScore = _score;

            // Obstacles
            ManageObstacles(gameTime);

            base.Update(gameTime);
        }

        private void ManageObstacles(GameTime gameTime)
        {
             // 1. Yeni Engel Ekleme
             float deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
             _spawnTimer -= deltaTime;

             if (_spawnTimer <= 0)
             {
                 SpawnObstacle();
                 _spawnTimer = _random.Next(1000, 2000) / (_speed / 6f); 
             }

             // 2. Engelleri Güncelle ve Çarpışma Kontrolü
             foreach (var obs in _obstacles)
             {
                 obs.SetSpeed(_speed);
                 obs.Update(gameTime);

                 if (obs.CollisionBox.Intersects(_player.CollisionBox))
                 {
                     GameOver();
                 }
             }

             // 3. Ekrandan Çıkanları Temizle (Modern Yöntem)
             _obstacles.RemoveAll(obs => obs.Position.X < -obs.Texture.Width);
        }

        private void SpawnObstacle()
        {
            Texture2D tex = _random.NextDouble() > 0.5 ? _cactusTexture : _bonesTexture;
            int floorY = GraphicsDevice.Viewport.Height - 50;
            Vector2 pos = new Vector2(GraphicsDevice.Viewport.Width + 50, floorY - tex.Height); 
            
            _obstacles.Add(new ObstacleSprite(tex, pos, _speed));
        }

        private void GameOver()
        {
            _gameOver = true;
            _player.Texture = _playerTextures["crash"]; 
            SaveHighScore();
        }

        private void LoadHighScore()
        {
            try
            {
                if (File.Exists("highscore.txt"))
                    _highScore = float.Parse(File.ReadAllText("highscore.txt"));
            }
            catch { }
        }

        private void SaveHighScore()
        {
            try
            {
                File.WriteAllText("highscore.txt", ((int)_score).ToString());
            }
            catch { }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue); 

            _spriteBatch.Begin();

            // 1. Draw Background (First, behind everything)
            if (_backgroundTexture != null)
            {
                _spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            }

            // 2. Draw Ground (Extend to 3 segments to prevent gaps)
            int floorY = GraphicsDevice.Viewport.Height - 50;
            Vector2 groundPos = new Vector2(_groundOffset, floorY);
            _spriteBatch.Draw(_groundTexture, groundPos, Color.White);
            _spriteBatch.Draw(_groundTexture, groundPos + new Vector2(_groundTexture.Width, 0), Color.White);
            _spriteBatch.Draw(_groundTexture, groundPos + new Vector2(_groundTexture.Width * 2, 0), Color.White); 
            _spriteBatch.Draw(_groundTexture, groundPos + new Vector2(_groundTexture.Width * 3, 0), Color.White); 

            // 3. Draw Entities (Obstacles, Player)
            foreach (var obs in _obstacles)
            {
                obs.Draw(_spriteBatch);
            }

            _player.Draw(_spriteBatch);

            // 4. Draw Monster (Last = Foreground)
            _monster.Draw(_spriteBatch); 

            // UI
            string scoreText = $"HI {(int)_highScore:D5}  {(int)_score:D5}";
            Vector2 scorePos = new Vector2(GraphicsDevice.Viewport.Width - 300, 10);
            _spriteBatch.DrawString(_font, scoreText, scorePos, Color.Gray);

            if (_gameOver)
            {
                string msg = "GAME OVER";
                Vector2 size = _font.MeasureString(msg);
                _spriteBatch.DrawString(_font, msg, new Vector2(GraphicsDevice.Viewport.Width/2 - size.X/2, GraphicsDevice.Viewport.Height/2), Color.Gray);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
