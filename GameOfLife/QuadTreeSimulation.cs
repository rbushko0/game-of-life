// Author: Rebecca Bushko

using System;
using System.Collections;
using UltimateQuadTree;

namespace RebeccaBushko.GameOfLife
{
    public class QuadTreeGameOfLife
    {
        private QuadTree<Cell> grid;
        private HashSet<Cell> cellsToAdd;

        public QuadTreeGameOfLife(List<Tuple<long, long>> initial)
        {
            grid = new QuadTree<Cell>(ulong.MaxValue, ulong.MaxValue, new CellBounds());
            cellsToAdd = new HashSet<Cell>();

            foreach (Tuple<long, long> cell in initial)
            {
                bool success = grid.Insert(new Cell()
                {
                    X = (ulong) (cell.Item1 + long.MaxValue),
                    Y = (ulong) (cell.Item2 + long.MaxValue),
                    State = CellState.ALIVE,
                });
            }
        }

        public void Simulate()
        {
            foreach (Cell cell in grid.GetObjects())
            {
                SetState(cell);
                Console.WriteLine();
                foreach (Cell cell2 in grid.GetNearestObjects(cell))
                {
                    Console.Write($"({cell2.X},{cell2.Y},{cell2.State})");
                }
                SetNeighbors(cell);
            }

            Console.WriteLine();
            List<Cell> toRemove = grid.GetObjects().Where(cell => cell.State != CellState.ALIVE).ToList();
            grid.RemoveRange(toRemove);

            foreach (Cell cell in cellsToAdd)
            {
                cell.State = CellState.ALIVE;
                grid.Insert(cell);
            }
            cellsToAdd.Clear();
        }

        private bool AreNeighbors(Cell cell1, Cell cell2)
        {
            if (cell2.State == CellState.DEAD)
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
            // top-left
            if (cell.X > ulong.MinValue && cell.Y > ulong.MinValue)
            {
                Cell neighborTemp = new Cell()
                {
                    X = cell.X - 1,
                    Y = cell.Y - 1,
                };
                SetState(neighborTemp, true);
            }

            // top-center
            if (cell.Y > ulong.MinValue)
            {
                Cell neighborTemp = new Cell()
                {
                    X = cell.X,
                    Y = cell.Y - 1,
                };
                SetState(neighborTemp, true);
            }

            // top-right
            if (cell.X < ulong.MaxValue && cell.Y > ulong.MinValue)
            {
                Cell neighborTemp = new Cell()
                {
                    X = cell.X + 1,
                    Y = cell.Y - 1,
                };
                SetState(neighborTemp, true);
            }

            // left
            if (cell.X > ulong.MinValue)
            {
                Cell neighborTemp = new Cell()
                {
                    X = cell.X - 1,
                    Y = cell.Y,
                };
                SetState(neighborTemp, true);
            }

            // right
            if (cell.X < ulong.MaxValue)
            {
                Cell neighborTemp = new Cell()
                {
                    X = cell.X + 1,
                    Y = cell.Y,
                };
                SetState(neighborTemp, true);
            }

            // bottom-left
            if (cell.X > ulong.MinValue && cell.Y > ulong.MinValue)
            {
                Cell neighborTemp = new Cell()
                {
                    X = cell.X - 1,
                    Y = cell.Y + 1,
                };
                SetState(neighborTemp, true);
            }

            // bottom-center
            if (cell.Y < ulong.MaxValue)
            {
                Cell neighborTemp = new Cell()
                {
                    X = cell.X,
                    Y = cell.Y + 1,
                };
                SetState(neighborTemp, true);
            }

            // bottom-right
            if (cell.X < ulong.MaxValue && cell.Y < ulong.MaxValue)
            {
                Cell neighborTemp = new Cell()
                {
                    X = cell.X + 1,
                    Y = cell.Y + 1,
                };
                SetState(neighborTemp, true);
            }
        }
        
        private void SetState(Cell cell, bool neighbor = false)
        {
            int neighborCount = 0;
            bool cellExists = false;
            Cell cellInGrid = cell;
            foreach (Cell possibleNeighbor in grid.GetNearestObjects(cell))
            {
                if (cell.X == possibleNeighbor.X && cell.Y == possibleNeighbor.Y)
                {
                    cellInGrid = possibleNeighbor;
                    cellExists = true;
                    continue;
                }

                if (AreNeighbors(cellInGrid, possibleNeighbor))
                {
                    neighborCount++;
                }
            }

            // Only check dead neighbors to see if they need to be set alive
            if (cellExists && neighbor)
            {
                return;
            }

            if (cellInGrid.State == CellState.ALIVE && neighborCount < 2 || neighborCount > 3)
            {
                cellInGrid.State = CellState.DYING;
            }
            else if (cell.State == CellState.DEAD && neighborCount == 3)
            {
                cellInGrid.State = CellState.ALIVE;
                if (!cellExists)
                {
                    cellsToAdd.Add(cellInGrid);
                }
            }
        }

        public IEnumerable<Cell> GetCells()
        {
            return grid.GetObjects().Where(cell => cell.State == CellState.ALIVE);
        }
        
        public void TogglePoint(ulong x, ulong y)
        {
            Cell cell = new Cell()
            {
                X = x,
                Y = y,
            };

            if (!grid.GetObjects().Contains<Cell>(cell))
            {
                cell.State = CellState.ALIVE;
                grid.Insert(cell);
            }
            else
            {
                cell.State = CellState.DEAD;
                grid.Remove(cell);
            }
        }
    }
}
