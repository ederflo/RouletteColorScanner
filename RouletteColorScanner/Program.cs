using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;

namespace RouletteColorScanner
{
    /*
     * 3200, 730  -->  Betting started (green)
     * 3200, 730  -->  Betting nearly ended (yellow)
     * 3200, 860  -->  Betting closed (red)
     * 3370, 860  -->  Color of turn (red or gray)
     */

    class Program
    {
        //Home-PC, Right Screen, Fullscreen, Live Roulette
        private static ScreenLocation betStart = new ScreenLocation(3200, 730);
        private static ScreenLocation waitForTurnEnd = new ScreenLocation(3200, 860);
        private static ScreenLocation colorOfTurn = new ScreenLocation(3370, 860);
        private static int greenEndsToStartTurnDelay = 14000;

        //Home-PC, Right Screen, Fullscreen, Auto-Roulette
        //private static ScreenLocation betStart = new ScreenLocation(3200, 560);
        //private static ScreenLocation waitForTurnEnd = new ScreenLocation(3200, 560);
        //private static ScreenLocation colorOfTurn = new ScreenLocation(3333, 142);
        //private static int greenEndsToStartTurnDelay = 14000;

        private static bool betted = false;

        private static void Main(string[] args)
        {
            int greenStreak = 0;
            int redStreak = 0;
            int blackStreak = 0;
            Color c;
            Console.ReadKey();
            Console.WriteLine("Program started");
            for (int i = 0; i< 5; i++)
            {
                c = GetColorAt(betStart);
                Thread.Sleep(10);
            } 
            while (true)
            {
                //Console.WriteLine("bet starts");
                do
                {
                    c = GetColorAt(betStart);
                    Thread.Sleep(300);
                } while (c.R < 25 && c.B < 25 && c.G > 150);

                Console.WriteLine("green ends");
                Thread.Sleep(greenEndsToStartTurnDelay);
                Console.WriteLine("turn starts");
                do
                {
                    c = GetColorAt(waitForTurnEnd);
                    Thread.Sleep(300);
                } while ((c.R > 65 && c.R < 95) && (c.G > 65 && c.G < 95) && (c.B > 65 && c.B < 95));
                Console.WriteLine("turn ends");
                Thread.Sleep(1500);
                c = GetColorAt(colorOfTurn);
                if (c.R > 150 && c.G < 100 && c.B < 100)
                {
                    if (betted && redStreak == 0)
                    {
                        SystemSounds.Beep.Play();
                        betted = false;
                    }
                    redStreak++;
                    greenStreak = 0;
                    blackStreak = 0;
                }
                else if (c.G >= 100 && c.B < 90 && c.R < 90)
                {
                    redStreak = 0;
                    greenStreak++;
                    blackStreak = 0;
                } 
                else if ((c.R > 75 && c.R < 130) && (c.G > 75 && c.G < 130) && (c.B > 75 && c.B < 130))
                {
                    if (betted && blackStreak == 0)
                    {
                        SystemSounds.Beep.Play();
                        betted = false;
                    }
                    redStreak = 0;
                    greenStreak = 0;
                    blackStreak++;
                }
                Console.Clear();
                Console.WriteLine(c.ToString());
                Console.WriteLine("Red: " + redStreak);
                Console.WriteLine("Black: " + blackStreak);
                Console.WriteLine("Green: " + greenStreak);
                int count = redStreak > 0 ? redStreak : blackStreak > 0 ? blackStreak : greenStreak > 0 ? greenStreak : 15;
                if (count >= 3)
                {
                    for (int i = 0; i < count; i++)
                    {
                        SystemSounds.Beep.Play();
                        Thread.Sleep(300);
                    }
                    
                    betted = true;
                }
            }     
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowDC(IntPtr window);
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern uint GetPixel(IntPtr dc, int x, int y);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int ReleaseDC(IntPtr window, IntPtr dc);

        public static Color GetColorAt(ScreenLocation location)
        {
            IntPtr desk = GetDesktopWindow();
            IntPtr dc = GetWindowDC(desk);
            int a = (int)GetPixel(dc, location.X, location.Y);
            ReleaseDC(desk, dc);
            return Color.FromArgb(255, (a >> 0) & 0xff, (a >> 8) & 0xff, (a >> 16) & 0xff);
        }
    }
}
