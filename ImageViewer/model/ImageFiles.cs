using System.Collections.Generic;
using Windows.Storage;

namespace ImageViewer.model
{
    public class ImageFiles
    {
        List<StorageFile> files = null;
        private int index = 0;
        public ImageFiles(IReadOnlyList<StorageFile> l)
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

        public int Count()
        {
            return files.Count;
        }

        public StorageFile GetNext()
        {
            if (index >= files.Count)
            {
                index = 0;
            }
            return files[index++];
        }

    }
}
