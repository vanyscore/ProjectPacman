using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pacman.Controllers
{
    public class PacmanController : GameComponent
    {
        private Pacman pacman;
        private Vector2 position;
        private Map.Direction mainDirection;
        private Map.Direction secondDirection;
        private Map.Direction lastDirection;
        private KeyboardState lastKeyboardState;
        private float rotation;

        public delegate void Void();

        public event Void GameIsWon;
        public event Void BallEated;

        public PacmanController(Game game, Pacman pacMan) : base(game)
        {
            Game.Components.Add(this);

            this.pacman = pacMan;
        }

        public override void Update(GameTime gameTime)
        {
            if (pacman.Killed) return;

            KeyboardState keyboardState = Keyboard.GetState();

            mainDirection = lastDirection;

            Keys[] keys = lastKeyboardState.GetPressedKeys();

            if (keys.Length > 0)
            {
                Keys key = keys[0];

                if (lastKeyboardState.IsKeyDown(key))
                {
                    if (keyboardState.IsKeyUp(key))
                    {
                        switch(key)
                        {
                            case Keys.Right:
                                mainDirection = Map.Direction.Right;
                                break;
                            case Keys.Down:
                                mainDirection = Map.Direction.Down;
                                break;
                            case Keys.Left:
                                mainDirection = Map.Direction.Left;
                                break;
                            case Keys.Up:
                                mainDirection = Map.Direction.Up;
                                break;
                        }

                        secondDirection = mainDirection;
                    }
                }
            }

            if (mainDirection.Equals(secondDirection))
            {
                UpdatePacmanPosition(mainDirection);
            }
            else
            {
                UpdatePacmanPosition(mainDirection);
                UpdatePacmanPosition(secondDirection);
            }

            lastKeyboardState = keyboardState;

            base.Update(gameTime);
        }

        private void UpdatePacmanPosition(Map.Direction direction)
        {
            position = Vector2.Zero;

            switch (direction)
            {
                case Map.Direction.Right: position.X += pacman.Velocity; rotation = 0f; break;
                case Map.Direction.Down: position.Y += pacman.Velocity; rotation = MathHelper.ToRadians(90f); break;
                case Map.Direction.Left: position.X -= pacman.Velocity; rotation = MathHelper.ToRadians(180f); break;
                case Map.Direction.Up: position.Y -= pacman.Velocity; rotation = MathHelper.ToRadians(270f); break;
            }

            pacman.UpdatePosition(position);

            if (pacman.IsIntersectsWithWalls())
            {
                pacman.UpdatePosition(-position);
            }
            else
            {
                if (pacman.IsIntersectsWithFood())
                {
                    pacman.Score += 10;

                    BallEated();

                    if (pacman.Map.FoodBlocks.Count == 0)
                    {
                        
                    }
                }

                lastDirection = direction;
                pacman.Rotation = rotation;
            }
        }
    }
}
