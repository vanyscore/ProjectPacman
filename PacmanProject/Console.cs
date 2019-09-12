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
    public class Console : DrawableGameComponent
    {
        private enum Positions { StringScore, IntScore, StringLives, IntLives, GameOver };

        private Texture2D texture;
        private Rectangle destinationRectangle;
        private Rectangle[] rectangles;
        private Rectangle block;
        private Dictionary<char, Rectangle> symbolsPairs;
        private Dictionary<Positions, Vector2> posPairs;

        public Console(Game game, Rectangle block) : base(game)
        {
            Game.Components.Add(this);

            this.block = block;

            this.block.Width /= 2;
            this.block.Height /= 2;

            symbolsPairs = new Dictionary<char, Rectangle>();
            posPairs = new Dictionary<Positions, Vector2>();

            destinationRectangle = new Rectangle(0, 0, this.block.Width, this.block.Height);

            posPairs.Add(Positions.StringScore, new Vector2(1 * block.Width, this.block.Height / 2));
            posPairs.Add(Positions.IntScore, new Vector2(1 * block.Width + 5 * block.Width, this.block.Height / 2));
            posPairs.Add(Positions.StringLives, new Vector2(15 * block.Width, this.block.Height / 2));
            posPairs.Add(Positions.IntLives, new Vector2(19 * block.Width, this.block.Height / 2));
            posPairs.Add(Positions.GameOver, new Vector2(1 * block.Width, block.Height * 14 + this.block.Height / 2));
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("symbols");

            InitializeSymbols();

            base.LoadContent();
        }

        private void InitializeSymbols()
        {
            symbolsPairs = new Dictionary<char, Rectangle>();

            char[] symbols = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I',
            'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', ' ' };

            rectangles = new Rectangle[36];

            int width = texture.Width / 36;

            Rectangle rectangle = new Rectangle(0, 0, width, texture.Height);

            rectangles[0] = rectangle;

            symbolsPairs.Add(symbols[0], rectangle);

            for (int i = 1; i < rectangles.Length; i++)
            {
                rectangle.X = i * width;

                rectangles[i] = rectangle;

                symbolsPairs.Add(symbols[i], rectangle);
            }

            symbolsPairs.Add(symbols.Last(), new Rectangle(-250, -250, 0, 0));
        }

        public void PrintScore(Pacman pacMan, SpriteBatch spriteBatch)
        {
            PrintText("SCORE", posPairs[Positions.StringScore], spriteBatch, Color.WhiteSmoke);
            PrintText(pacMan.Score.ToString(), posPairs[Positions.IntScore], spriteBatch, Color.Yellow);
            PrintText("LIVES", posPairs[Positions.StringLives], spriteBatch, Color.WhiteSmoke);
            PrintText(string.Format("{0}", pacMan.Lives), posPairs[Positions.IntLives], spriteBatch, Color.Yellow);
        }

        public void PrintGameOver(int lives, SpriteBatch spriteBatch)
        {
            Color color;

            string message = "";

            if (lives > 0)
            {
                color = Color.Green;
                message = "YOU WIN";
            }
            else
            {
                color = Color.Red;
                message = "YOU LOSE";
            }

            PrintText(message, posPairs[Positions.GameOver], spriteBatch, color);
        }

        public void PrintPositions(Pacman pacMan, List<Enemy> enemies, SpriteBatch spriteBatch)
        {
            PrintText(string.Format("{0}{1}", pacMan.MapPosition.X, pacMan.MapPosition.Y), pacMan.Position, spriteBatch, Color.Red);

            foreach (Enemy enemy in enemies)
            {
                PrintText(string.Format("{0}{1}", enemy.MapPosition.X, enemy.MapPosition.Y), enemy.Position, spriteBatch, Color.Green);
            }
        }

        public void PrintWay(List<Enemy> enemies, SpriteBatch spriteBatch)
        {
            //int[][] mapArray = enemy.MapArray;

            //for (int i = 0; i < mapArray.Length; i++)
            //{
            //    for (int k = 0; k < mapArray[i].Length; k++)
            //    {
            //        int value = mapArray[i][k];

            //        if (value > 0)
            //        {
            //            Vector2 position = new Vector2(k * block.Width * 2, i * block.Height * 2);

            //            position.X += block.Width / 2;
            //            position.Y += block.Height / 2;

            //            PrintText(string.Format("{0}", value), position, spriteBatch, Color.Red);
            //        }
            //    }
            //}

            foreach (Enemy enemy in enemies)
            {
                Vector2 pos = new Vector2(enemy.DestinationPoint.X * block.Width * 2, enemy.DestinationPoint.Y * block.Height * 2);

                Rectangle rectangle = new Rectangle((int)pos.X, (int)pos.Y, block.Width * 2, block.Height * 2);

                spriteBatch.Draw(texture, rectangle, Color.Green);
            }
        }

        public void PrintText(string word, Vector2 position, SpriteBatch spriteBatch, Color color)
        {
            char[] symbol = word.ToCharArray();

            Rectangle[] rectangles = new Rectangle[symbol.Length];

            int index = 0;

            foreach (char letter in symbol)
                rectangles[index++] = symbolsPairs[letter];

            destinationRectangle.X = (int)position.X;
            destinationRectangle.Y = (int)position.Y;

            foreach (Rectangle rectangle in rectangles)
            {
                spriteBatch.Draw(texture, destinationRectangle, rectangle, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);

                destinationRectangle.X += destinationRectangle.Width + 10;
            }
        }
    }
}
