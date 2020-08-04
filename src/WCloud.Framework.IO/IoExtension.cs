using System.Drawing;
using System.Drawing.Imaging;

namespace Lib.io
{
    public static class IoExtension
    {
        /// <summary>
        /// bitmap转byte数组
        /// </summary>
        public static byte[] ToBytes(this Bitmap bm, ImageFormat formart) =>
            ImageHelper.BitmapToBytes(bm, formart);
    }
}
