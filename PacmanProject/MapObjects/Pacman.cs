using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pacman.Controllers;

namespace Pacman
{
    public class Pacman : MapObject
    {
        private Texture2D[] dieTextures;
        private Texture2D[] liveTextures;
        private double totalSeconds;
        private double frequency;
        private int index;
        private int score;
        private int lives;
        private bool killed;
        private bool wanted;

        public event EnemiesController.Void GameEnd;

        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                score = value;
            }
        }

        public int Lives
        {
            get
            {
                return lives;
            }
            set
            {
                lives = value;
            }
        }

        public bool Killed
        {
            get
            {
                return killed;
            }
            set
            {
                killed = value;

                index = 0;

                if (killed)
                    textures = dieTextures;
                else
                    textures = liveTextures;
            }
        }

        public bool Wanted
        {
            get
            {
                return wanted;
            }
            set
            {
                wanted = value;
            }
        }

        public Pacman(Game game, Map map, Vector2 mapPosition) : base(game, map, mapPosition)
        {
            score = 0;
            index = 0;
            totalSeconds = 0;
            frequency = .15f;
            lives = 3;

            destinationRectangle = new Rectangle(rectangle.X, rectangle.Y, map.Block.Width - 10, map.Block.Height - 10);
        }

        protected override void LoadContent()
        {
            liveTextures = new Texture2D[3];

            liveTextures[0] = Game.Content.Load<Texture2D>("pacman/pacman_1");
            liveTextures[1] = Game.Content.Load<Texture2D>("pacman/pacman_2");
            liveTextures[2] = Game.Content.Load<Texture2D>("pacman/pacman_3");

            textures = liveTextures;

            origin = new Vector2(textures[0].Width / 2, textures[0].Height / 2);

            LoadDieTextures();

            base.LoadContent();
        }

        private void LoadDieTextures()
        {
            dieTextures = new Texture2D[13];

            int index = 0;

            for (int i = 1; i <= dieTextures.Length; i++)
                dieTextures[index++] = Game.Content.
                    Load<Texture2D>(string.Format("pacman/pacman_die_{0}", i));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            totalSeconds += gameTime.ElapsedGameTime.TotalSeconds;

            if (totalSeconds >= frequency)
            {
                totalSeconds = 0;
                index++;
            }

            if (index == textures.Length)
            {
                index = 0;

                if (killed)
                {
                    lives--;

                    Killed = false;

                    SetPosition(originalMapPosition);

                    foreach (Enemy enemy in map.Enemies)
                    {
                        enemy.SetPosition(enemy.OriginalMapPosition);
                    }

                    if (lives == 0)
                    {
                        GameEnd();

                        return;
                    }
                }
            }

            spriteBatch.Draw(textures[index], destinationRectangle, null, Color.White, rotation, origin, SpriteEffects.None, 0);
            //spriteBatch.Draw(textures[index], rectangle, Color.Green);
        }

        protected override void UpdateDestinationRectangle()
        {
            destinationRectangle.X = rectangle.X + rectangle.Width / 2;
            destinationRectangle.Y = rectangle.Y + rectangle.Height / 2;
        }

        public bool IsIntersectsWithFood()
        {
            Rectangle foodBlock = new Rectangle((int)mapPosition.X * rectangle.Width,
                (int)mapPosition.Y * rectangle.Height,
                rectangle.Width, rectangle.Height);

            if (map.FoodBlocks.Contains(foodBlock) && rectangle.Intersects(foodBlock))
            {
                map.FoodBlocks.Remove(foodBlock);

                if (map.FoodBlocks.Count == 0)
                    GameEnd();

                return true;
            }

            return false;
        }

        public bool IsIntersectsWithBonus()
        {
            Rectangle bonusBlock = new Rectangle((int)mapPosition.X * rectangle.Width,
                (int)mapPosition.Y * rectangle.Height,
                rectangle.Width, rectangle.Height);

            if (map.BonusBlocks.Contains(bonusBlock) && rectangle.Intersects(bonusBlock))
            {
                map.BonusBlocks.Remove(bonusBlock);
                return true;
            }

            return false;
        }

        //public bool IsIntersectsWithBonus()
        //{
        //    Rectangle bonusBlock = new Rectangle()
        //}
    }
}
