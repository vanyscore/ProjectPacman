using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pacman
{
    public class Enemy : MapObject
    {

        private Texture2D[] originalTextures;
        private Texture2D[] policeTextures;
        private Texture2D[] ghostTextures;
        private Texture2D unprotectedTexture;
        private Stack<Vector2> roadPoints;
        private Map.Direction direction;
        private Vector2 destinationPoint;
        private bool isProtected;
        private bool ghost;

        public Stack<Vector2> RoadPoints
        {
            get
            {
                return roadPoints;
            }
            set
            {
                roadPoints = value;
                destinationPoint = roadPoints.Last();
            }
        }
        public Map.Direction Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
            }
        }

        public bool Protected
        {
            get
            {
                return isProtected;
            }
            set
            {
                isProtected = value;

                UpdateTexture();
            }
        }

        public bool Ghost
        {
            get
            {
                return ghost;
            }
            set
            {
                ghost = value;

                ChangeTextures();
                UpdateTexture();
            }
        }

        public Vector2 DestinationPoint
        {
            get
            {
                return destinationPoint;
            }
            set
            {
                destinationPoint = value;
            }
        }

        public Vector2 OriginalMapPosition
        {
            get
            {
                return originalMapPosition;
            }
        }

        public Enemy(Game game, Map map, Vector2 mapPosition) : base(game, map, mapPosition)
        {
            destinationRectangle = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width - 10, rectangle.Height - 10);
            roadPoints = new Stack<Vector2>();
            destinationPoint = map.FreeRandomPoint;
            isProtected = true;

            base.velocity = 2;
            base.originalVelocity = velocity;
        }

        protected override void LoadContent()
        {
            textures = new Texture2D[4];

            originalTextures = new Texture2D[]
            {
                Game.Content.Load<Texture2D>("enemies/enemy_1"),
                Game.Content.Load<Texture2D>("enemies/enemy_2"),
                Game.Content.Load<Texture2D>("enemies/enemy_3"),
                Game.Content.Load<Texture2D>("enemies/enemy_4")
            };

            policeTextures = new Texture2D[]
            {
                Game.Content.Load<Texture2D>("enemies/enemy_police_1"),
                Game.Content.Load<Texture2D>("enemies/enemy_police_2"),
                Game.Content.Load<Texture2D>("enemies/enemy_police_3"),
                Game.Content.Load<Texture2D>("enemies/enemy_police_4")
            };

            ghostTextures = new Texture2D[]
            {
                Game.Content.Load<Texture2D>("enemies/enemy_ghost_1"),
                Game.Content.Load<Texture2D>("enemies/enemy_ghost_2"),
                Game.Content.Load<Texture2D>("enemies/enemy_ghost_3"),
                Game.Content.Load<Texture2D>("enemies/enemy_ghost_4")
            };

            textures = originalTextures;

            texture = textures[0];

            unprotectedTexture = Game.Content.Load<Texture2D>("enemies/enemy_unprotected");

            base.LoadContent();
        }

        public void ChangeTextures()
        {
            if (!map.Pacman.Wanted)
            {
                textures = originalTextures;
            }
            else
            {
                textures = policeTextures;
            }

            if (ghost)
            {
                textures = ghostTextures;
            }
        }

        public void UpdateTexture()
        {
            if (!isProtected)
            {
                texture = unprotectedTexture;
                return;
            }

            switch(direction)
            {
                case Map.Direction.Right: texture = textures[0]; break;
                case Map.Direction.Down: texture = textures[1]; break;
                case Map.Direction.Left: texture = textures[2]; break;
                case Map.Direction.Up: texture = textures[3]; break;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, destinationRectangle, Color.White);
        }

        protected override void UpdateDestinationRectangle()
        {
            destinationRectangle.X = rectangle.X + 5;
            destinationRectangle.Y = rectangle.Y + 5;
        }
    }
}
