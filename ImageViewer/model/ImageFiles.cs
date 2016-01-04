using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ImageViewer.model
{
    public class ImageFiles
    {
        private static ImageFiles _instance = null;

        private ImageFiles() {
            files = new List<StorageFile>();
            filelist = new List<string>();
            index = 0;
        }

        public static ImageFiles GetInstance()
        {
            if (_instance == null)
                _instance = new ImageFiles();
            return _instance;
        }

        private List<StorageFile> files = null;
        private List<string> filelist = null;

        private int index = 0;
        public void SetStorage(IReadOnlyList<StorageFile> l)
        {
            foreach (var f in l)
            {
                this.Add(f);
            }
        }

        public void Add(StorageFile f)
        {
            files.Add(f);
            filelist.Add(f.Path);
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
            Debug.WriteLine(" Previous : " + (index - 1).ToString());
            if (index <= 0)
            {
                return files[0];
            }
            else
            {
                return files[--index];
            }
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

        public void Clear()
        {
            this.filelist.Clear();
            this.files.Clear();
        }

        private const string filename = "test.dat";

        async public Task SaveAsync()
        {
            StorageFile sessionFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            using (IRandomAccessStream sessionRandomAccess = await sessionFile.OpenAsync(FileAccessMode.ReadWrite))
            using (IOutputStream sessionOutputStream = sessionRandomAccess.GetOutputStreamAt(0))
            {
                //Using DataContractSerializer , look at the cat-class
                var sessionSerializer = new DataContractSerializer(typeof(List<String>));
                sessionSerializer.WriteObject(sessionOutputStream.AsStreamForWrite(), this.filelist);

                await sessionOutputStream.FlushAsync();
            }
        }

        async public Task RestoreAsync()
        {
            StorageFile sessionFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
            if (sessionFile == null)
            {
                return;
            }
            this.Clear();
            List<String> fileList = null;
            using (IInputStream sessionInputStream = await sessionFile.OpenReadAsync())
            {
                //Using DataContractSerializer
                var sessionSerializer = new DataContractSerializer(typeof(List<String>));
                fileList = (List<String>)sessionSerializer.ReadObject(sessionInputStream.AsStreamForRead());
            }

            foreach (var f in fileList)
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(f);
                this.Add(file);
            }
        }

    }
}
