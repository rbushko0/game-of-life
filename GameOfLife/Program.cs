// Author: Rebecca Bushko

using GameOfLife;
using RebeccaBushko.GameOfLife;

public class Program
{
    public static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("1 - Console Mode");
            Console.WriteLine("2 - GUI Mode");

            char selection = Console.ReadKey().KeyChar;
            switch (selection)
            {
                case '1':
                    RunConsoleSimulation();
                    break;
                case '2':
                    RunGuiSimulation();
                    break;
            }

            Console.WriteLine();
        }
    }

    private static void RunConsoleSimulation()
    {
        ConsoleSimulation consoleSim = new ConsoleSimulation();
        consoleSim.Run();
    }

    private static void RunGuiSimulation()
    {
        GuiSimulation guiSim = new GuiSimulation();
        while (guiSim.IsRunning())
        {
            guiSim.Update();
        }

        guiSim.Destroy();
    }
}