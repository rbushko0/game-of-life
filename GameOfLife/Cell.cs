
namespace RebeccaBushko.GameOfLife
{
    public class Cell
    {
        public ulong X { get; set; }
        public ulong Y { get; set; }
        public CellState State { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Cell))
            {
                return false;
            }

            return this.X == ((Cell)obj).X && this.Y == ((Cell)obj).Y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }
}
