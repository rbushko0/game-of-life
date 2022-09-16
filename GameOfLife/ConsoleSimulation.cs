
using RebeccaBushko.GameOfLife;

namespace GameOfLife
{
    public class ConsoleSimulation
    {
        private const int NUM_STEPS = 10;

        public ConsoleSimulation()
        {

        }

        public void Run()
        {
            Console.WriteLine();
            Console.WriteLine("Enter coordinates in line-by-line in (X,Y) format. Press Enter twice to finish.");
            List<Tuple<long, long>> liveCells = new List<Tuple<long, long>>();

            bool readingInput = true;
            while (readingInput)
            {
                string coordinate = Console.ReadLine();
                coordinate = coordinate.Replace(" ", "");

                if (string.IsNullOrEmpty(coordinate) || coordinate.StartsWith('d'))
                {
                    readingInput = false;
                    continue;
                }

                bool bothMatch = true;
                if (!long.TryParse(coordinate.Substring(1, coordinate.IndexOf(',') - 1), out long first))
                {
                    bothMatch = false;
                }
                coordinate = coordinate.Substring(coordinate.IndexOf(',') + 1);
                if (!long.TryParse(coordinate.Substring(0, coordinate.Length - 1), out long second))
                {
                    bothMatch = false;
                }

                if (bothMatch)
                {
                    liveCells.Add(new Tuple<long, long>(first, second));
                }
            }

            QuadTreeGameOfLife gol = new QuadTreeGameOfLife(liveCells);
            for (int i = 0; i < NUM_STEPS; i++)
            {
                gol.Simulate();
            }

            Console.WriteLine();
            Console.WriteLine("#Life 1.06");
            foreach (Cell cell in gol.GetCells())
            {
                Console.WriteLine($"{cell.X - long.MaxValue} {cell.Y - long.MaxValue}");
            }
        }
    }
}
