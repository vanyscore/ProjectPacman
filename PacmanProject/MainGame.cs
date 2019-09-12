using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Pacman.Controllers;

namespace Pacman
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private PacmanController pacManController;
        private EnemiesController enemiesController;
        private MusicController musicController;
        private Map map;
        private Pacman pacman;
        private Console console;
        private List<Enemy> enemies;
        private int[][] level;

        private bool gameEnd;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1260;
            graphics.PreferredBackBufferHeight = 990;

            graphics.IsFullScreen = false;
        }

        protected override void Initialize()
        {
            level = Levels.LevelOne;

            InititalizeGameObjects();

            console = new Console(this, map.Block);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (gameEnd)
            {
                KeyboardState keyboardState = Keyboard.GetState();

                if (keyboardState.IsKeyDown(Keys.Enter))
                {
                    RemoveMusicEvents();

                    level = Levels.LevelTwo;

                    InititalizeGameObjects();
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront);

            map.Draw(gameTime, spriteBatch);

            //console.PrintPositions(pacman, enemies, spriteBatch);
            //console.PrintWay(enemies, spriteBatch);
            console.PrintScore(pacman, spriteBatch);

            if (gameEnd)
            {
                console.PrintGameOver(pacman.Lives, spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void OnGameEnd()
        {
            Components.Clear();
            musicController.Stop();
            gameEnd = true;
        }

        private void InititalizeGameObjects()
        {
            map = new Map(this, level);

            enemies = map.Enemies;
            gameEnd = false;

            pacman = map.Pacman;
            pacManController = new PacmanController(this, pacman);
            enemiesController = new EnemiesController(this, map);
            musicController = new MusicController(this);

            enemiesController.PacmanWanted += musicController.OnPacmanWanted;
            enemiesController.BonusEated += musicController.OnPacmanPowerUp;
            enemiesController.GhostEated += musicController.OnGhostEat;
            enemiesController.EnemyRestored += musicController.OnEnemyRestored;
            enemiesController.PacmanDie += musicController.OnPacmanDie;

            pacManController.BallEated += musicController.OnBallEated;

            pacman.GameEnd += OnGameEnd;
        }

        private void RemoveMusicEvents()
        {
            enemiesController.PacmanWanted -= musicController.OnPacmanWanted;
            enemiesController.BonusEated -= musicController.OnPacmanPowerUp;
            enemiesController.GhostEated -= musicController.OnGhostEat;
            enemiesController.EnemyRestored -= musicController.OnEnemyRestored;
            enemiesController.PacmanDie -= musicController.OnPacmanDie;

            pacManController.BallEated -= musicController.OnBallEated;
        }
    }
}
