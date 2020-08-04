using Lib.helper;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lib.io
{
    public class WordHelper
    {
        public byte[] CreateWord(Dictionary<string, string> paragraphs)
        {
            if (paragraphs == null)
                throw new ArgumentNullException(nameof(paragraphs));

            var doc = new XWPFDocument();
            try
            {
                //创建段落对象
                foreach (var key in paragraphs.Keys)
                {
                    XWPFParagraph paragraph = doc.CreateParagraph();
                    XWPFRun run = paragraph.CreateRun();
                    run.IsBold = true;
                    run.SetText(ConvertHelper.GetString(key));
                    run.SetText(ConvertHelper.GetString(paragraphs[key]));
                }

                using (var stream = new MemoryStream())
                {
                    doc.Write(stream);
                    var bs = stream.ToArray();
                    return bs;
                }
            }
            finally
            {
                doc.Close();
            }
        }
    }
}
