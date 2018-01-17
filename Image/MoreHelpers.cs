using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image
{
    class MoreHelpers
    {
        public static List<string> AvailableFormats = new List<string>() { ".jpg", ".jpeg", ".bmp", ".png", ".tif", ".gif" };
        public static bool CheckForInputFormat(string ImgExtension)
        {
            if (!AvailableFormats.Contains(ImgExtension))
            {
                Console.WriteLine("Unsupport image format (extension. Support: jpg, jpeg, bmp, png, tif, gif (only first picture for animation)");
                return false;
            }
            else if (ImgExtension == ".gif")
            {
                Console.WriteLine("Worning! For gif animation take only first picture");
                return true;
            }
            return true;
        }

        public static void DirectoryExistance(string path)
        {
            bool exists = System.IO.Directory.Exists(path);

            if (!exists)
                System.IO.Directory.CreateDirectory(path);
        }
    }
}
