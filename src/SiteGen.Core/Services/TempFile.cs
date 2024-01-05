namespace SiteGen.Core.Services
{
    public class TempFile : IDisposable
    {
        private readonly FileInfo fileInfo;

        public TempFile(string path)
        {
            fileInfo = new FileInfo(path);
        }

        public string FullName {  get =>  fileInfo.FullName; }

        public bool Exists { get => fileInfo.Exists; }

        public string NameWithoutExtension { get => Path.GetFileNameWithoutExtension(fileInfo.Name); }

        public void Dispose()
        {
            if (!fileInfo.Exists) return;
            fileInfo.Delete();
        }
    }
}
