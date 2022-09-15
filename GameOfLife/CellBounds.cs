using System.Drawing;
using UltimateQuadTree;

namespace RebeccaBushko.GameOfLife
{
    public class CellBounds : IQuadTreeObjectBounds<Cell>
    {
        public ulong GetLeft(Cell obj)
        {
            return obj.X - 1;
        }

        public ulong GetRight(Cell obj)
        {
            return obj.X + 1;
        }

        public ulong GetTop(Cell obj)
        {
            return obj.Y + 1;
        }

        public ulong GetBottom(Cell obj)
        {
            return obj.Y - 1;
        }
    }
}
