using System.Collections.Generic;
using System.Diagnostics;
using Windows.Storage;

namespace ImageViewer.model
{
    public class ImageFiles
    {
        private static ImageFiles _instance = null;

        private ImageFiles() { }

        public static ImageFiles GetInstance()
        {
            if (_instance == null)
                _instance = new ImageFiles();
            return _instance;
        }

        List<StorageFile> files = null;
        private int index = 0;
        public void SetStorage(IReadOnlyList<StorageFile> l)
        {
            files = new List<StorageFile>();
            index = 0;

            foreach (var f in l)
            {
                this.Add(f);
            }
        }

        public void Add(StorageFile f)
        {
            files.Add(f);
        }

        public int count { get { return files.Count; } }

        public StorageFile GetNext()
        {
            if (index >= files.Count)
            {
                index = 0;
            }
            Debug.WriteLine(" Next : " + index.ToString());
            return files[index++];
        }
        public StorageFile GetPrevious()
        {
            if (index - 1 < 0)
            {
                index = 0;
            }
            Debug.WriteLine(" Previous : " + (index - 1).ToString());
            return files[--index];
        }

        public StorageFile GetSpecified(int page)
        {
            if ((0 < page) && (page < files.Count))
            {
                index = page;
            }
            Debug.WriteLine(" Specified " + index.ToString());
            return files[index];
        }
    }
}
