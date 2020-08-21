using Lib.extension;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace WCloud.Framework.IO.Image_
{
    public class VerificationCodeHelper
    {
        public static readonly VerificationCodeHelper Instance = new VerificationCodeHelper();

        private readonly List<char> chars;

        public VerificationCodeHelper()
        {
            var chars = new List<char>();
            Action<char, char> FindChar = (start, end) =>
            {
                for (var i = start; i <= end; ++i)
                {
                    chars.Add((char)i);
                }
            };
            FindChar('a', 'z');
            FindChar('A', 'Z');
            FindChar('0', '9');

            chars.RemoveWhere_(x => new char[] { '0', 'o', 'O', '1', 'l', '9', 'q', 'z', 'Z', '2' }.Contains(x));

            this.chars = chars;
        }

        public List<Color> Colors => new List<Color>() { Color.Purple, Color.Black, Color.DarkBlue, Color.DarkRed };

        public List<string> Fonts => new List<string>() { "Times New Roman", "MS Mincho", "Gungsuh", "PMingLiU", "Impact" };

        public List<char> Chars => this.chars;
    }
}
