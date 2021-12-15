using System;

namespace fReEFLEX_clicker
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new ClickerGame())
                game.Run();
        }
    }
}
