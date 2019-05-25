using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace ayeonbbsbackend.Utils
{
    public class ImageHelper
    {
        public static void Base64ToImg(string imgStr, string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imgStr))
                    return;
                var bytes = Convert.FromBase64String(imgStr);
                File.WriteAllBytes(filePath, bytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }
    }
}
