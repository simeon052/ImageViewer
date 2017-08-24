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
            storageFileList = new List<StorageFile>();
            storageFilePathList = new List<string>();
            index = 0;
        }

        public static ImageFiles GetInstance()
        {
            if (_instance == null)
                _instance = new ImageFiles();
            return _instance;
        }

        private List<StorageFile> storageFileList = null;
        private List<String> storageFilePathList = null;

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
            storageFileList.Add(f);
            storageFilePathList.Add(f.Path);
        }

        public int count { get { return storageFileList.Count; } }
        public int current { get { return index; } }

        public StorageFile GetNext()
        {
            if (index >= storageFileList.Count)
            {
                index = 0;
            }
            Debug.WriteLine(" Next : " + index.ToString());
            return storageFileList[index++];
        }
        public StorageFile GetPrevious()
        {
            Debug.WriteLine(" Previous : " + (index - 1).ToString());
            if (index <= 0)
            {
                return storageFileList[0];
            }
            else
            {
                return storageFileList[--index];
            }
        }

        public StorageFile GetSpecified(int page)
        {
            if ((0 < page) && (page < storageFileList.Count))
            {
                index = page;
            }
            Debug.WriteLine(" Specified " + index.ToString());
            if (storageFileList.Count == 0)
            {
                return null;
            }
            else
            {
                return storageFileList[index];
            }
        }

        public void Clear()
        {
            this.storageFilePathList.Clear();
            this.storageFileList.Clear();
            index = 0;
        }

        private const string defaultFileName = "filelist.dat";

        async public Task SaveAsync(string filename = defaultFileName)
        {
            StorageFile sessionFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            using (IRandomAccessStream sessionRandomAccess = await sessionFile.OpenAsync(FileAccessMode.ReadWrite))
            using (IOutputStream sessionOutputStream = sessionRandomAccess.GetOutputStreamAt(0))
            {
                //Using DataContractSerializer
                var sessionSerializer = new DataContractSerializer(typeof(List<String>));
                sessionSerializer.WriteObject(sessionOutputStream.AsStreamForWrite(), this.storageFilePathList);

                await sessionOutputStream.FlushAsync();
            }
        }

        async public Task RestoreAsync(string filename = defaultFileName)
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
                if (sessionInputStream.AsStreamForRead().Length != 0)
                {
                    //Using DataContractSerializer
                    var sessionSerializer = new DataContractSerializer(typeof(List<String>));
                    fileList = (List<String>)sessionSerializer.ReadObject(sessionInputStream.AsStreamForRead());
                }
            }
            if (fileList != null)
            {
                foreach (var f in fileList)
                {
                    StorageFile file = await StorageFile.GetFileFromPathAsync(f);
                    this.Add(file);
                }
            }
        }

    }
}
