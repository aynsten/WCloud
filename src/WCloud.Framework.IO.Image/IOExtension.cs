using System.Drawing;
using System.Drawing.Imaging;

namespace WCloud.Framework.IO.Image_
{
    public static class IOExtension
    {
        /// <summary>
        /// bitmap转byte数组
        /// </summary>
        public static byte[] ToBytes(this Bitmap bm, ImageFormat formart) =>
            ImageHelper.BitmapToBytes(bm, formart);
    }
}
