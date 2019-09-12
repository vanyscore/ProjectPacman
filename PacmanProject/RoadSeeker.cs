using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Pacman
{
    public class RoadSeeker
    {
        int[][] originalMap;
        int[][] map;
        int count;

        Vector2 sourcePoint;
        Vector2 destinationPoint;

        Stack<Vector2> roadPoints;
        

        public RoadSeeker(int[][] map)
        {
            this.originalMap = map;
            this.map = new int[originalMap.Length][];
            this.roadPoints = new Stack<Vector2>();

            CopyMap();
        }

        public Stack<Vector2> GetRoadPoints(Vector2 sourcePoint, Vector2 destiantionPoint)
        {
            this.sourcePoint = sourcePoint;
            this.destinationPoint = destiantionPoint;

            CreateWaves2DestinationPoint();
            FillRoadPoints();

            return roadPoints;
        }

        private void CreateWaves2DestinationPoint()
        {
            CopyMap();

            count = 1;

            int posX = (int)sourcePoint.X;
            int posY = (int)sourcePoint.Y;

            Stack<Vector2> points = new Stack<Vector2>();
            Stack<Vector2> savedPoints = new Stack<Vector2>();
            Stack<Vector2> neighoburs = new Stack<Vector2>();

            map[posY][posX] = 1;

            points.Push(sourcePoint);

            bool isFinded = false;

            while (!isFinded)
            {
                count++;

                while (points.Count > 0)
                {
                    Vector2 point = points.Pop();

                    neighoburs = GetNeighbours(point);

                    while (neighoburs.Count > 0)
                    {
                        Vector2 neighbour = neighoburs.Pop();

                        int x = (int)neighbour.X;
                        int y = (int)neighbour.Y;

                        if (neighbour.Equals(destinationPoint))
                        {
                            isFinded = true;
                            map[y][x] = count;
                            break;
                        }

                        if (map[y][x] == 0)
                        {
                            map[y][x] = count;

                            savedPoints.Push(neighbour);
                        }
                    }
                }

                while (savedPoints.Count > 0)
                    points.Push(savedPoints.Pop());
            }

            count = 0;
        }

        private void FillRoadPoints()
        {
            roadPoints = new Stack<Vector2>();

            Stack<Vector2> points = new Stack<Vector2>();

            points.Push(destinationPoint);

            bool isFinded = false;

            roadPoints.Push(destinationPoint);

            while (!isFinded)
            {
                if (points.Count == 0) return;

                Vector2 point = points.Pop();

                Stack<Vector2> neighbours = GetNeighbours(point);

                while (neighbours.Count > 0)
                {
                    Vector2 neighbour = neighbours.Pop();

                    int pX = (int)point.X;
                    int pY = (int)point.Y;

                    int nX = (int)neighbour.X;
                    int nY = (int)neighbour.Y;

                    if (map[pY][pX] - map[nY][nX] == 1)
                    {
                        points.Push(neighbour);
                        roadPoints.Push(neighbour);
                        neighbours.Clear();
                    }
                }

                if (roadPoints.Peek().Equals(sourcePoint))
                    isFinded = true;
            }
        }

        private Stack<Vector2> GetNeighbours(Vector2 point)
        {
            Stack<Vector2> neighbours = new Stack<Vector2>();

            int x = (int)point.X;
            int y = (int)point.Y;

            if (map[y][x + 1] >= 0)
                neighbours.Push(new Vector2(point.X + 1, point.Y));

            if (map[y][x - 1] >= 0)
                neighbours.Push(new Vector2(point.X - 1, point.Y));

            if (map[y - 1][x] >= 0)
                neighbours.Push(new Vector2(point.X, point.Y - 1));

            if (map[y + 1][x] >= 0)
                neighbours.Push(new Vector2(point.X, point.Y + 1));

            return neighbours;
        }

        private void CopyMap()
        {
            for (int i = 0; i < originalMap.Length; i++)
            {
                int[] array = new int[originalMap[i].Length];

                for (int k = 0; k < array.Length; k++)
                {
                    array[k] = originalMap[i][k];
                }

                map[i] = array;
            }
        }
    }
}
