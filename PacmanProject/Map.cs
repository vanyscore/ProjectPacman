using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pacman
{
    public class Map : DrawableGameComponent
    {
        protected Texture2D wallTexture;
        protected Texture2D foodTexture;
        protected Texture2D bonusTexture;
        protected List<Rectangle> foodBlocks;
        protected List<Rectangle> wallBlocks;
        protected List<Rectangle> bonusBlocks;
        protected List<Vector2> freePoints;
        protected Rectangle block;
        protected Color color;
        protected int[][] blocks;
        protected List<Enemy> enemies;
        protected Pacman pacMan;
        protected List<MapObject> mapObjects;
        protected Random random;
        protected Vector2 pacManPosition;
        protected Vector2[] enemiesPositions;
        public enum Direction { None, Right, Down, Left, Up };

        public Rectangle Block
        {
            get
            {
                return block;
            }
        }

        public int[][] Blocks
        {
            get
            {
                return blocks;
            }
        }

        public List<Rectangle> FoodBlocks
        {
            get
            {
                return foodBlocks;
            }
            set
            {
                foodBlocks = value;
            }
        }

        public List<Vector2> FreePoints
        {
            get
            {
                return freePoints;
            }
        }

        public List<Rectangle> BonusBlocks
        {
            get
            {
                return bonusBlocks;
            }
        }

        public Vector2 FreeRandomPoint
        {
            get
            {
                return freePoints[random.Next(freePoints.Count)];
            }
        }

        public List<Enemy> Enemies
        {
            get
            {
                return enemies;
            }
        }

        public Pacman Pacman
        {
            get
            {
                return pacMan;
            }
        }

        public Map(Game game, int[][] level) : base(game)
        {
            Game.Components.Add(this);

            random = new Random();
            enemies = new List<Enemy>();

            blocks = level;

            int width = GraphicsDevice.Viewport.Bounds.Width / blocks[0].Length;
            int height = GraphicsDevice.Viewport.Bounds.Height / blocks.Length;

            block = new Rectangle(0, 0, width, height);

            InitializeBlocks();
            InitializeMapObjects();
        }

        protected override void LoadContent()
        {
            wallTexture = Game.Content.Load<Texture2D>("wall");
            foodTexture = Game.Content.Load<Texture2D>("food");
            bonusTexture = Game.Content.Load<Texture2D>("bonus");

            base.LoadContent();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawWallBlocks(spriteBatch);
            DrawFoodBlocks(spriteBatch);
            DrawBonusBlocks(spriteBatch);

            pacMan.Draw(gameTime, spriteBatch);

            foreach (Enemy enemy in enemies)
                enemy.Draw(gameTime, spriteBatch);

            base.Draw(gameTime);
        }

        private void DrawFoodBlocks(SpriteBatch spriteBatch)
        {
            foreach (Rectangle foodBlock in foodBlocks)
            {
                spriteBatch.Draw(foodTexture, foodBlock, null, new Color(203, 193, 187), 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
            }
        }

        private void DrawWallBlocks(SpriteBatch spriteBatch)
        {
            foreach (Rectangle wall in wallBlocks)
            {
                spriteBatch.Draw(wallTexture, wall, null, new Color(40, 40, 93), 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
            }
        }

        private void DrawBonusBlocks(SpriteBatch spriteBatch)
        {
            foreach (Rectangle block in bonusBlocks)
            {
                spriteBatch.Draw(bonusTexture, block, Color.Pink);
            }
        }

        public Vector2 GetMapPosition(Vector2 mapPosition)
        {
            return new Vector2(mapPosition.X * block.Width, mapPosition.Y * block.Height);
        }

        private void InitializeBlocks()
        {
            wallBlocks = new List<Rectangle>();
            foodBlocks = new List<Rectangle>();
            bonusBlocks = new List<Rectangle>();
            freePoints = new List<Vector2>();

            for (int i = 0; i < blocks.Length; i++)
            {
                for (int k = 0; k < blocks[i].Length; k++)
                {
                    Rectangle rectangle = new Rectangle(k * block.Width, i * block.Height,
                        block.Width, block.Height);

                    switch (blocks[i][k])
                    {
                        case 0:
                            foodBlocks.Add(rectangle);
                            freePoints.Add(new Vector2(k, i));
                            break;
                        case -1:
                            wallBlocks.Add(rectangle);
                            break;
                        case -4:
                            bonusBlocks.Add(rectangle);
                            blocks[i][k] = 0;
                            break;
                    }
                }
            }
        }

        private void InitializeMapObjects()
        {
            mapObjects = new List<MapObject>();
            enemiesPositions = new Vector2[3];
            int index = 0;

            for (int i = 0; i < blocks.Length; i++)
            {
                for (int k = 0; k < blocks[i].Length; k++)
                {
                    switch (blocks[i][k])
                    {
                        case -2:
                            pacMan = new Pacman(Game, this, new Vector2(k, i));
                            blocks[i][k] = 0;
                            foodBlocks.Add(new Rectangle(k * block.Width, i * block.Height, block.Width, block.Height));
                            pacManPosition = pacMan.MapPosition;
                            break;
                        case -3:
                            enemies.Add(new Enemy(Game, this, new Vector2(k, i)));
                            blocks[i][k] = 0;
                            foodBlocks.Add(new Rectangle(k * block.Width, i * block.Height, block.Width, block.Height));
                            enemiesPositions[index++] = enemies.Last().MapPosition;
                            break;
                    }
                }
            }

            mapObjects.Add(pacMan);
            mapObjects.AddRange(enemies);
        }
    }
}
