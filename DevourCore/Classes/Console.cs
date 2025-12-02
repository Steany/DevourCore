using System;
using System.Text.RegularExpressions;
using SysConsole = System.Console;

namespace DevourCore
{
    public static class Console
    {
        public static void Show()
        {
            int width = SysConsole.WindowWidth;

            string StripAnsiCodes(string input)
            {
                return Regex.Replace(input, @"\x1B\[[0-9;]*m", "");
            }

            string RightShiftedCenter(string text, int shift = 0)
            {
                int visibleLength = StripAnsiCodes(text).Length;
                int padding = ((width - visibleLength) / 2) + shift;
                if (padding < 0) padding = 0;
                return new string(' ', padding) + text;
            }

            string pink = "\u001b[38;2;255;0;255m";
            string lightBlue = "\u001b[38;2;0;255;255m";
            string yellow = "\u001b[38;2;255;255;0m";
            string reset = "\u001b[0m";

            SysConsole.WriteLine("\n\n\n");

            string[] banner0 = new string[]
            {
                "  ██████╗ ███████╗██╗   ██╗ ██████╗ ██╗   ██╗██████╗  ██████╗ ██████╗ ██████╗ ███████╗",
                "  ██╔══██╗██╔════╝██║   ██║██╔═══██╗██║   ██║██╔══██╗██╔════╝██╔═══██╗██╔══██╗██╔════╝",
                "  ██║  ██║█████╗  ██║   ██║██║   ██║██║   ██║██████╔╝██║     ██║   ██║██████╔╝█████╗  ",
                "  ██║  ██║██╔══╝  ╚██╗ ██╔╝██║   ██║██║   ██║██╔══██╗██║     ██║   ██║██╔══██╗██╔══╝  ",
                "  ██████╔╝███████╗ ╚████╔╝ ╚██████╔╝╚██████╔╝██║  ██║╚██████╗╚██████╔╝██║  ██║███████╗",
                "  ╚═════╝ ╚══════╝  ╚═══╝   ╚═════╝  ╚═════╝ ╚═╝  ╚═╝ ╚═════╝ ╚═════╝ ╚═╝  ╚═╝╚══════╝"
            };

            foreach (var line in banner0)
                SysConsole.WriteLine(RightShiftedCenter(pink + line + reset));
            SysConsole.WriteLine("\n");

            string[] bannerBy = new string[]
            {
                "       ██████╗ ██╗   ██╗",
                "       ██╔══██╗╚██╗ ██╔╝",
                "       ██████╔╝ ╚████╔╝ ",
                "       ██╔══██╗  ╚██╔╝  ",
                "       ██████╔╝   ██║   ",
                "       ╚═════╝    ╚═╝   "
            };

            string[] bannerSteany = new string[]
            {
                "    ███████╗████████╗███████╗ █████╗ ███╗   ██╗██╗   ██╗",
                "    ██╔════╝╚══██╔══╝██╔════╝██╔══██╗████╗  ██║╚██╗ ██╔╝",
                "    ███████╗   ██║   █████╗  ███████║██╔██╗ ██║ ╚████╔╝ ",
                "    ╚════██║   ██║   ██╔══╝  ██╔══██║██║╚██╗██║  ╚██╔╝  ",
                "    ███████║   ██║   ███████╗██║  ██║██║ ╚████║   ██║   ",
                "    ╚══════╝   ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝  ╚═══╝   ╚═╝   "
            };

            for (int i = 0; i < bannerBy.Length; i++)
            {
                string coloredLine = yellow + bannerBy[i] + lightBlue + bannerSteany[i] + reset;
                SysConsole.WriteLine(RightShiftedCenter(coloredLine, -2));
            }

            SysConsole.WriteLine("\n");

            string[] bannerAnd = new string[]
            {
                "       █████╗ ███╗   ██╗██████╗ ",
                "      ██╔══██╗████╗  ██║██╔══██╗",
                "      ███████║██╔██╗ ██║██║  ██║",
                "      ██╔══██║██║╚██╗██║██║  ██║",
                "      ██║  ██║██║ ╚████║██████╔╝",
                "      ╚═╝  ╚═╝╚═╝  ╚═══╝╚═════╝ "
            };

            string[] bannerMikasa = new string[]
            {
                "    ███╗   ███╗██╗██╗  ██╗ █████╗ ███████╗ █████╗ ",
                "    ████╗ ████║██║██║ ██╔╝██╔══██╗██╔════╝██╔══██╗",
                "    ██╔████╔██║██║█████╔╝ ███████║███████╗███████║",
                "    ██║╚██╔╝██║██║██╔═██╗ ██╔══██║╚════██║██╔══██║",
                "    ██║ ╚═╝ ██║██║██║  ██╗██║  ██║███████║██║  ██║",
                "    ╚═╝     ╚═╝╚═╝╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝"
            };

            for (int i = 0; i < bannerAnd.Length; i++)
            {
                string coloredLine = yellow + bannerAnd[i] + lightBlue + bannerMikasa[i] + reset;
                SysConsole.WriteLine(RightShiftedCenter(coloredLine, -2));
            }

            SysConsole.WriteLine("\n\n");
        }
    }
}