using System;

namespace Stories.Pages.Chat
{
    public static class GenerateSet
    {
        public static ColorSet GenerateColorSet()
        {
            ColorSet ColorSet = new ColorSet();

            Random r = new Random();
            int red = r.Next(0, 255);
            int green = r.Next(0, 255);
            int blue = r.Next(0, 255);
            double transparency = ((double)r.Next(25, 100) / 100);

            ColorSet.BackgroundColor = "rgba(" + red + ", " + green + ", " + blue + ", " + transparency.ToString() + ")";

            int fontRed = 255 - red;
            int fontGreen = 255 - green;
            int fontBlue = 255 - blue;

            ColorSet.TextColor = "rgba(" + fontRed + "," + fontGreen + "," + fontBlue + ", 1.0)";

            if (((fontRed + fontGreen + fontBlue) / 3) > 123)
                ColorSet.NameColor = "rgba(20, 20, 20, 1)";
            else
                ColorSet.NameColor = "rgba(230, 230, 230, 1)";
            return ColorSet;
        }
    }
}