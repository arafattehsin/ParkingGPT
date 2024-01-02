using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingGPT.Helpers
{
    public static class UtilityHelper
    {
        //public static byte[] GetImageStreamAsBytes(Stream input)
        //{
        //    var buffer = new byte[input.Length];
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        int read;
        //        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
        //        {
        //            ms.Write(buffer, 0, read);
        //        }
        //        return ms.ToArray();
        //    }
        //}

        public static byte[] GetImageStreamAsBytes(Stream stream)
        {
            byte[] bytes;

            using (var binaryReader = new BinaryReader(stream))
            {
                stream.Position = 0;
                bytes = binaryReader.ReadBytes(Convert.ToInt32(stream.Length));
            }

            return bytes;
        }

        public static string GetBase64Image(string path)
        {
            var fileInfo = new FileInfo(path);
            using Stream sourceStream = fileInfo.OpenRead();
            var byteArrayImage = UtilityHelper.GetImageStreamAsBytes(sourceStream);
            return Convert.ToBase64String(byteArrayImage);
        }

    }
}
