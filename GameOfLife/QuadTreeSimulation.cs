// Author: Rebecca Bushko

using System;
using System.Collections;
using UltimateQuadTree;

namespace RebeccaBushko.GameOfLife
{
    public class QuadTreeGameOfLife
    {
        private QuadTree<Cell> grid;
        private QuadTree<Cell> cellsToAdd;

        public QuadTreeGameOfLife(List<Tuple<long, long>> initial)
        {
            grid = new QuadTree<Cell>(ulong.MaxValue, ulong.MaxValue, new CellBounds());
            cellsToAdd = new QuadTree<Cell>(ulong.MaxValue, ulong.MaxValue, new CellBounds());

            foreach (Tuple<long, long> cell in initial)
            {
                bool success = grid.Insert(new Cell()
                {
                    X = (ulong) (cell.Item1 + long.MaxValue),
                    Y = (ulong) (cell.Item2 + long.MaxValue),
                    IsAlive = true,
                });
            }
        }

        public void Simulate()
        {
            foreach (Cell cell in grid.GetObjects())
            {
                IEnumerable<Cell> nearest = grid.GetNearestObjects(cell);

                SetState(cell);
                SetNeighbors(cell);
            }

            grid.InsertRange(cellsToAdd.GetObjects());
            cellsToAdd.Clear();
        }

        private bool AreNeighbors(Cell cell1, Cell cell2)
        {
            if (!cell2.IsAlive)
            {
                return false;
            }

            bool xMatch = cell1.X == cell2.X ||
                cell1.X != ulong.MaxValue && cell1.X + 1 == cell2.X ||
                cell1.X != ulong.MinValue && cell1.X - 1 == cell2.X;

            bool yMatch = cell1.Y == cell2.Y ||
                cell1.Y != ulong.MaxValue && cell1.Y + 1 == cell2.Y ||
                cell1.Y != ulong.MinValue && cell1.Y - 1 == cell2.Y;

            return xMatch && yMatch;
        }

        private void SetNeighbors(Cell cell)
        {
            Cell neighborTemp = new Cell();
            // top-left
            if (cell.X > ulong.MinValue && cell.Y > ulong.MinValue)
            {
                neighborTemp.X = cell.X - 1;
                neighborTemp.Y = cell.Y - 1;
                SetState(neighborTemp);
            }

            // top-center
            if (cell.Y > ulong.MinValue)
            {
                neighborTemp.X = cell.X;
                neighborTemp.Y = cell.Y - 1;
                SetState(neighborTemp);
            }

            // top-right
            if (cell.X < ulong.MaxValue && cell.Y > ulong.MinValue)
            {
                neighborTemp.X = cell.X + 1;
                neighborTemp.Y = cell.Y - 1;
                SetState(neighborTemp);
            }

            // left
            if (cell.X > ulong.MinValue)
            {
                neighborTemp.X = cell.X - 1;
                neighborTemp.Y = cell.Y;
                SetState(neighborTemp);
            }

            // right
            if (cell.X < ulong.MaxValue)
            {
                neighborTemp.X = cell.X + 1;
                neighborTemp.Y = cell.Y;
                SetState(neighborTemp);
            }

            // bottom-left
            if (cell.X > ulong.MinValue && cell.Y > ulong.MinValue)
            {
                neighborTemp.X = cell.X - 1;
                neighborTemp.Y = cell.Y + 1;
                SetState(neighborTemp);
            }

            // bottom-center
            if (cell.Y < ulong.MaxValue)
            {
                neighborTemp.X = cell.X;
                neighborTemp.Y = cell.Y + 1;
                SetState(neighborTemp);
            }

            // bottom-right
            if (cell.X < ulong.MaxValue && cell.Y < ulong.MaxValue)
            {
                neighborTemp.X = cell.X + 1;
                neighborTemp.Y = cell.Y + 1;
                SetState(neighborTemp);
            }
        }
        
        private void SetState(Cell cell)
        {
            int neighborCount = 0;
            bool cellExists = false;
            foreach (Cell possibleNeighbor in grid.GetNearestObjects(cell))
            {
                if (cell.X == possibleNeighbor.X && cell.Y == possibleNeighbor.Y)
                {
                    cellExists = true;
                    continue;
                }

                if (AreNeighbors(cell, possibleNeighbor))
                {
                    neighborCount++;
                }
            }

            if (cell.IsAlive)
            {
                if (neighborCount < 2 || neighborCount > 3)
                {
                    cell.IsAlive = false;
                }
                else
                {
                    cell.IsAlive = true;
                }
            }
            else if (neighborCount == 3)
            {
                cell.IsAlive = true;
            }

            if (cell.IsAlive && !cellExists)
            {
                cellsToAdd.Insert(cell);
            }
        }

        public IEnumerable<Cell> GetCells()
        {
            return grid.GetObjects().Where(cell => cell.IsAlive);
        }
        
        public void TogglePoint(ulong x, ulong y)
        {
            Cell cell = new Cell()
            {
                X = x,
                Y = y,
                IsAlive = true
            };

            if (!grid.GetObjects().Contains<Cell>(cell))
            {
                grid.Insert(cell);
            }
            else
            {
                grid.Remove(cell);
            }
        }
    }
}
