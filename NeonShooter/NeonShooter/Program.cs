using System;

namespace NeonShooter
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (XNA_Main game = new XNA_Main())
            {
                game.Run();
            }
        }
    }
#endif
}

