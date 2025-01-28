namespace ygBlog
{
    public class FileDir
    {
        public static string Data => GetPath(Root, "data");
        public static string PostRoot => GetPath(Data, "posts");
        public static string PostImageRoot => GetPath(Data, "post_images");
        public static string CustomImageRoot => GetPath(WWWRoot, "custom");
        public static string CacheAvatarRoot => GetPath(Data, "cached_avatars");
        public static string PageResource => GetPath(Root, "PageResource");
        public static string WWWRoot => GetPath(Root, "wwwroot");
        static string GetPath(params string[] paths)
        {
            string ret = Path.Combine(paths);
            if (!Directory.Exists(ret)) Directory.CreateDirectory(ret);
            return ret;
        }
        public static string Root => Environment.CurrentDirectory;
    }
}
