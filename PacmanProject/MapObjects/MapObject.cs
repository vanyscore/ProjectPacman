using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pacman
{
    public class MapObject : DrawableGameComponent
    {
        protected Texture2D texture;
        protected Texture2D[] textures;
        protected Vector2 position;
        protected Vector2 origin;
        protected float rotation;
        protected int velocity;
        protected int originalVelocity;
        protected Rectangle rectangle;
        protected Rectangle destinationRectangle;
        protected Vector2 mapPosition;
        protected Vector2 originalMapPosition;
        protected Map map;

        public Texture2D Texture
        {
            get
            {
                return texture;
            }
            set
            {
                texture = value;
            }
        }
        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                return rectangle;
            }
            set
            {
                rectangle = value;
            }
        }

        public Rectangle DestinationRectangle
        {
            get
            {
                return destinationRectangle;
            }
        }

        public Vector2 MapPosition
        {
            get
            {
                return mapPosition;
            }
            set
            {
                mapPosition = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public int Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                velocity = value;
            }
        }

        public int OriginalVelocity
        {
            get
            {
                return originalVelocity;
            }
        }

        public Map Map
        {
            get
            {
                return map;
            }
        }

        public MapObject(Game game, Map map, Vector2 mapPosition) : base(game)
        {
            Game.Components.Add(this);

            this.map = map;
            this.mapPosition = mapPosition;
            this.originalMapPosition = mapPosition;

            rectangle = new Rectangle(0, 0, map.Block.Width, map.Block.Height);
            rotation = 0f;
            velocity = 2;

            SetPosition(mapPosition);
        }

        public void SetPosition(Vector2 mapPosition)
        {
            this.mapPosition = mapPosition;

            position.X = mapPosition.X * rectangle.Width;
            position.Y = mapPosition.Y * rectangle.Height;

            rectangle.X = (int)position.X;
            rectangle.Y = (int)position.Y;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) {}

        public override void Update(GameTime gameTime)
        {
            UpdateDestinationRectangle();

            base.Update(gameTime);
        }

        public void UpdatePosition(Vector2 position)
        {
            this.position += position;

            rectangle.X = (int)(this.position.X);
            rectangle.Y = (int)(this.position.Y);

            if (rectangle.X % map.Block.Width == 0
                && rectangle.Y % map.Block.Height == 0)
            {
                mapPosition.X = rectangle.X / rectangle.Width;
                mapPosition.Y = rectangle.Y / rectangle.Height;
            }
        }

        protected virtual void UpdateDestinationRectangle()
        {
            destinationRectangle.X = rectangle.X;
            destinationRectangle.Y = rectangle.Y;
        }

        public bool IsIntersectsWithWalls()
        {
            List<Rectangle> walls = new List<Rectangle>(6);
            List<int[]> nearBlocks = new List<int[]>(6);

            int y = (int)mapPosition.Y, x = (int)mapPosition.X;

            nearBlocks.Add(new int[] { y, x + 1 });
            nearBlocks.Add(new int[] { y, x - 1 });
            nearBlocks.Add(new int[] { y - 1, x + 1 });
            nearBlocks.Add(new int[] { y - 1, x });
            nearBlocks.Add(new int[] { y - 1, x - 1 });
            nearBlocks.Add(new int[] { y + 1, x + 1 });
            nearBlocks.Add(new int[] { y + 1, x });
            nearBlocks.Add(new int[] { y + 1, x - 1 });

            foreach (int[] block in nearBlocks)
                if (map.Blocks[block[0]][block[1]] < 0)
                    walls.Add(new Rectangle(block[1] * map.Block.Width, block[0] * map.Block.Height,
                        map.Block.Width, map.Block.Height));

            foreach (Rectangle wall in walls)
                if (rectangle.Intersects(wall))
                    return true;

            return false;
        }
    }
}
