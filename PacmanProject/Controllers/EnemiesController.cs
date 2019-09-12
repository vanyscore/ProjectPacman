using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Pacman.Controllers
{
    public class EnemiesController : GameComponent
    {
        private Map map;
        private List<Enemy> enemies;
        private RoadSeeker roadSeeker;

        private double totalSeconds;
        private double policeTime;
        private double unprotectedSeconds;

        private bool isProtected;

        public delegate void Void();

        public event Void GhostEated;
        public event Void BonusEated;
        public event Void EnemyRestored;
        public event Void PacmanWanted;
        public event Void PacmanDie;

        public EnemiesController(Game game, Map map) : base(game)
        {
            Game.Components.Add(this);

            this.map = map;
            this.enemies = map.Enemies;
            this.roadSeeker = new RoadSeeker(map.Blocks);
            this.totalSeconds = 0;
            this.policeTime = 15;
            this.unprotectedSeconds = 0;
            this.isProtected = true;

            foreach (Enemy enemy in enemies)
            {
                enemy.DestinationPoint = map.FreeRandomPoint;
                enemy.RoadPoints = roadSeeker.GetRoadPoints(enemy.MapPosition, enemy.DestinationPoint);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (map.Pacman.Killed == true) return;

            totalSeconds += gameTime.ElapsedGameTime.TotalSeconds;

            if (totalSeconds >= policeTime)
            {
                map.Pacman.Wanted = !map.Pacman.Wanted;

                if (map.Pacman.Wanted)
                {
                    PacmanWanted();

                    policeTime = 10;
                }
                else
                {
                    foreach (Enemy enemy in enemies)
                    {
                        if (enemy.Ghost) continue;

                        enemy.DestinationPoint = map.FreeRandomPoint;
                        enemy.RoadPoints = roadSeeker.GetRoadPoints(enemy.MapPosition, enemy.DestinationPoint);
                    }

                    policeTime = 15;
                }

                totalSeconds = 0;

                foreach (Enemy enemy in enemies)
                {
                    enemy.ChangeTextures();
                }
            }

            if (map.Pacman.IsIntersectsWithBonus())
            {
                BonusEated();

                foreach (Enemy enemy in enemies)
                {
                    enemy.Protected = false;
                }

                isProtected = false;
                unprotectedSeconds = 0;
            }

            if (!isProtected)
            {
                unprotectedSeconds += gameTime.ElapsedGameTime.TotalSeconds;

                if (unprotectedSeconds >= 10)
                {
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.Protected = true;
                    }

                    unprotectedSeconds = 0;
                    isProtected = true;
                }
            }

            if (map.Pacman.Wanted)
            {
                foreach (Enemy enemy in enemies)
                {
                    if (enemy.Ghost) continue;

                    enemy.DestinationPoint = map.Pacman.MapPosition;
                    enemy.RoadPoints = roadSeeker.GetRoadPoints(enemy.MapPosition, enemy.DestinationPoint);
                }
            }

            UpdateEnemiesPositions();

            base.Update(gameTime);
        }

        private void UpdateEnemiesPositions()
        {
            foreach (Enemy enemy in enemies)
            {
                Stack<Vector2> roadPoints = enemy.RoadPoints;
                Vector2 mapPosition = enemy.MapPosition;
                Map.Direction direction = enemy.Direction;

                if (mapPosition.Equals(roadPoints.Peek()))
                {
                    Vector2 point = roadPoints.Pop();

                    if (roadPoints.Count == 0)
                    {
                        enemy.DestinationPoint = map.FreeRandomPoint;
                        roadPoints = roadSeeker.GetRoadPoints(enemy.MapPosition, enemy.DestinationPoint);

                        point = roadPoints.Pop();

                        enemy.RoadPoints = roadPoints;

                        if (enemy.Ghost)
                        {
                            EnemyRestored();

                            enemy.Velocity = enemy.OriginalVelocity;
                            enemy.Ghost = false;
                            enemy.Protected = isProtected;
                        }
                    }

                    Vector2 prePoint = roadPoints.Pop();
                    Vector2 difference = prePoint - point;

                    enemy.Direction = GetDirection(difference);

                    enemy.UpdateTexture();

                    roadPoints.Push(prePoint);
                }

                if (enemy.IsIntersectsWithWalls())
                {
                    Vector2 basePosition = enemy.Position;

                    basePosition.X = mapPosition.X * enemy.Rectangle.Width;
                    basePosition.Y = mapPosition.Y * enemy.Rectangle.Height;

                    enemy.Position = basePosition;
                }

                Vector2 position = SetPosition(direction, enemy.Velocity);

                enemy.UpdatePosition(position);

                if (enemy.Rectangle.Intersects(map.Pacman.Rectangle))
                {
                    if (!enemy.Ghost)
                    {
                        if (!isProtected)
                        {
                            GhostEated();

                            map.Pacman.Score += 200;

                            enemy.RoadPoints = roadSeeker.GetRoadPoints(enemy.MapPosition, enemy.OriginalMapPosition);
                            enemy.Velocity = 6;
                            enemy.Ghost = true;
                            enemy.Protected = true;

                            enemy.Position = new Vector2(
                                enemy.MapPosition.X * enemy.Rectangle.Width,
                                enemy.MapPosition.Y * enemy.Rectangle.Height);
                        }
                        else
                        {
                            map.Pacman.Killed = true;

                            PacmanDie();

                            foreach (Enemy en in enemies)
                            {
                                en.RoadPoints = roadSeeker.GetRoadPoints(en.OriginalMapPosition, map.FreeRandomPoint);
                            }
                        }
                    }
                }
            }
        }

        private Map.Direction GetDirection(Vector2 difference)
        {
            Map.Direction direction = Map.Direction.Right;

            switch (difference.X)
            {
                case -1: direction = Map.Direction.Left; break;
                case 1: direction = Map.Direction.Right; break;
            }

            switch (difference.Y)
            {
                case -1: direction = Map.Direction.Up; break;
                case 1: direction = Map.Direction.Down; break;
            }

            return direction;
        }

        private Vector2 SetPosition(Map.Direction direction, int velocity)
        {
            Vector2 position = Vector2.Zero;

            switch (direction)
            {
                case Map.Direction.Right: position.X += velocity; break;
                case Map.Direction.Down: position.Y += velocity; break;
                case Map.Direction.Left: position.X -= velocity; break;
                case Map.Direction.Up: position.Y -= velocity; break;
            }

            return position;
        }
    }
}
