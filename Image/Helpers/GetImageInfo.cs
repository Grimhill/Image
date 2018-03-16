using System.IO;

namespace Image
{
    //obtain some image information for process. 
    //Can extend with more new helping methods about image
    public static class GetImageInfo
    {
        public static string Imginfo(this Imageinfo info)
        {
            var file = Program.FILE_Path;
            string result = string.Empty;

            switch (info)
            {
                case Imageinfo.Extension:
                    result = Path.GetExtension(file).ToLower();
                    break;
                case Imageinfo.FileName:
                    result = Path.GetFileNameWithoutExtension(file);
                    break;
            }
            return result;
        }

        public static string MyPath(this string directory)
        {
            var savepath = Program.Save_FILE_Path;
            return (savepath + "\\" + directory + "\\").DirectoryExist();
        }

        public static string LutPath(this string lut)
        {
            string parent = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            return parent + lut;
        }
    }

    public enum Imageinfo
    {
        Extension,
        FileName
    }

    public enum PreparedLutPath
    {
        BwmorphHelper,
    }
}
