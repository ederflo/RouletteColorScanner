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
        //private static ScreenLocation betStart = new ScreenLocation(3200, 730);
        //private static ScreenLocation waitForTurnEnd = new ScreenLocation(3200, 860);
        //private static ScreenLocation colorOfTurn = new ScreenLocation(3370, 860);
        //private static ScreenLocation betBlack = new ScreenLocation(2950, 955);
        //private static ScreenLocation betRed = new ScreenLocation(2830, 955);
        //private static ScreenLocation doubleValue = new ScreenLocation(3100, 1055);
        //private static int greenEndsToStartTurnDelay = 14000;

        //Home-PC, Right Screen, Fullscreen, Auto-Roulette
        private static ScreenLocation betStart = new ScreenLocation(3200, 560);
        private static ScreenLocation waitForTurnEnd = new ScreenLocation(3200, 560);
        private static ScreenLocation colorOfTurn = new ScreenLocation(3333, 142);
        private static ScreenLocation betBlack = new ScreenLocation(2600, 900);
        private static ScreenLocation betRed = new ScreenLocation(2500, 900);
        private static ScreenLocation doubleValue = new ScreenLocation(3100, 1055);
        private static int greenEndsToStartTurnDelay = 14000;

        //Laptop, Fullscreen, Auto-Roulette
        //private static ScreenLocation betStart = new ScreenLocation(1400, 560);
        //private static ScreenLocation waitForTurnEnd = new ScreenLocation(1400, 560);
        //private static ScreenLocation colorOfTurn = new ScreenLocation(1410, 133);
        //private static ScreenLocation betBlack = new ScreenLocation(700, 900);
        //private static ScreenLocation betRed = new ScreenLocation(550, 900);
        //private static int greenEndsToStartTurnDelay = 14000;

        private const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        private const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        private static bool betted = false;
        private static int mouseSpeed = 15;
        private static Random random = new Random();

        private static void Main(string[] args)
        {
            bool moveMouse = true;
            bool movedMouseToColor = false;
            bool mouseMoved = false;
            bool sound = false;
            int greenStreak = 0;
            int redStreak = 0;
            int blackStreak = 0;
            int countStreak = 0;
            int betStreak = 2;
            int defaultNumOfClicks = 1;
            int numOfClicks = defaultNumOfClicks;
            int maxStreak = 10;
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
                Console.WriteLine("bet starts");
                do
                {
                    c = GetColorAt(betStart);
                    Thread.Sleep(300);
                    if (moveMouse && !mouseMoved)
                    {
                        if (countStreak >= betStreak)
                        {
                            if (redStreak >= betStreak && countStreak <= maxStreak)
                            {
                                MoveMouse(betBlack.X, betBlack.Y, 0, 0);
                                Thread.Sleep(200);
                                movedMouseToColor = true;
                            }
                            else if (blackStreak >= betStreak && countStreak <= maxStreak)
                            {
                                MoveMouse(betRed.X, betRed.Y, 0, 0);
                                Thread.Sleep(200);
                                movedMouseToColor = true;
                            }

                            if (movedMouseToColor)
                            {
                                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                                MoveMouse(doubleValue.X, doubleValue.Y, 0, 0);
                                Thread.Sleep(80);
                                int sleepClickTime = random.Next(50, 120); 
                                for (int i = 0; i < countStreak - betStreak; i++)
                                {
                                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                                    Thread.Sleep(sleepClickTime);
                                }
                            }
                            numOfClicks *= 2;
                            mouseMoved = true;
                            movedMouseToColor = false;
                        }
                    }
                        

                } while (c.R < 50 && c.B < 50 && c.G > 150);
                mouseMoved = false;

                Console.WriteLine("green ends");
                Thread.Sleep(greenEndsToStartTurnDelay);
                Console.WriteLine("turn starts");
                do
                {
                    c = GetColorAt(waitForTurnEnd);
                    Thread.Sleep(300);
                } while ((c.R > 65 && c.R < 130) && (c.G > 65 && c.G < 130) && (c.B > 65 && c.B < 130));
                Console.WriteLine("turn ends");
                Thread.Sleep(1500);
                c = GetColorAt(colorOfTurn);
                if (c.R > 150 && c.G < 100 && c.B < 100)
                {
                    if (betted && redStreak == 0)
                    {
                        if (sound)
                            SystemSounds.Beep.Play();
                        betted = false;
                    }
                    redStreak++;
                    greenStreak = 0;
                    blackStreak = 0;
                }
                else if (c.G >= 100 && c.B < 90 && c.R < 90)
                {
                    //redStreak = 0;
                    //greenStreak++;
                    //blackStreak = 0;
                    if (redStreak > 0)
                        redStreak++;
                    else if (blackStreak > 0)
                        blackStreak++;
                } 
                else if ((c.R > 75 && c.R < 130) && (c.G > 75 && c.G < 130) && (c.B > 75 && c.B < 130))
                {
                    if (betted && blackStreak == 0)
                    {
                        if (sound)
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
                countStreak = redStreak > 0 ? redStreak : blackStreak > 0 ? blackStreak : greenStreak > 0 ? greenStreak : 15;
                if (countStreak >= betStreak)
                {
                    for (int i = 0; i < countStreak; i++)
                    {
                        if (sound)
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

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point p);

        public static Color GetColorAt(ScreenLocation location)
        {
            IntPtr desk = GetDesktopWindow();
            IntPtr dc = GetWindowDC(desk);
            int a = (int)GetPixel(dc, location.X, location.Y);
            ReleaseDC(desk, dc);
            return Color.FromArgb(255, (a >> 0) & 0xff, (a >> 8) & 0xff, (a >> 16) & 0xff);
        }


        static void MoveMouse(int x, int y, int rx, int ry)
        {
            Point c = new Point();
            GetCursorPos(out c);
            

            x += random.Next(rx);
            y += random.Next(ry);

            double randomSpeed = Math.Max((random.Next(mouseSpeed) / 2.0 + mouseSpeed) / 10.0, 0.1);

            WindMouse(c.X, c.Y, x, y, 9.0, 3.0, 10.0 / randomSpeed,
                15.0 / randomSpeed, 10.0 * randomSpeed, 10.0 * randomSpeed);
        }

        static void WindMouse(double xs, double ys, double xe, double ye,
            double gravity, double wind, double minWait, double maxWait,
            double maxStep, double targetArea)
        {

            double dist, windX = 0, windY = 0, veloX = 0, veloY = 0, randomDist, veloMag, step;
            int oldX, oldY, newX = (int)Math.Round(xs), newY = (int)Math.Round(ys);

            double waitDiff = maxWait - minWait;
            double sqrt2 = Math.Sqrt(2.0);
            double sqrt3 = Math.Sqrt(3.0);
            double sqrt5 = Math.Sqrt(5.0);

            dist = Hypot(xe - xs, ye - ys);

            while (dist > 1.0)
            {

                wind = Math.Min(wind, dist);

                if (dist >= targetArea)
                {
                    int w = random.Next((int)Math.Round(wind) * 2 + 1);
                    windX = windX / sqrt3 + (w - wind) / sqrt5;
                    windY = windY / sqrt3 + (w - wind) / sqrt5;
                }
                else
                {
                    windX = windX / sqrt2;
                    windY = windY / sqrt2;
                    if (maxStep < 3)
                        maxStep = random.Next(3) + 3.0;
                    else
                        maxStep = maxStep / sqrt5;
                }

                veloX += windX;
                veloY += windY;
                veloX = veloX + gravity * (xe - xs) / dist;
                veloY = veloY + gravity * (ye - ys) / dist;

                if (Hypot(veloX, veloY) > maxStep)
                {
                    randomDist = maxStep / 2.0 + random.Next((int)Math.Round(maxStep) / 2);
                    veloMag = Hypot(veloX, veloY);
                    veloX = (veloX / veloMag) * randomDist;
                    veloY = (veloY / veloMag) * randomDist;
                }

                oldX = (int)Math.Round(xs);
                oldY = (int)Math.Round(ys);
                xs += veloX;
                ys += veloY;
                dist = Hypot(xe - xs, ye - ys);
                newX = (int)Math.Round(xs);
                newY = (int)Math.Round(ys);

                if (oldX != newX || oldY != newY)
                    SetCursorPos(newX, newY);

                step = Hypot(xs - oldX, ys - oldY);
                int wait = (int)Math.Round(waitDiff * (step / maxStep) + minWait);
                Thread.Sleep(wait);
            }

            int endX = (int)Math.Round(xe);
            int endY = (int)Math.Round(ye);
            if (endX != newX || endY != newY)
                SetCursorPos(endX, endY);
        }

        static double Hypot(double dx, double dy)
        {
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
