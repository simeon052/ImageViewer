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
    [KnownType(typeof(StorageFile))]
    [CollectionDataContract]
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

        [DataMember]
        private List<StorageFile> files = null;

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

        private const string filename = "test.dat";

        async public Task SaveAsync<T>()
        {
                StorageFile sessionFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                IRandomAccessStream sessionRandomAccess = await sessionFile.OpenAsync(FileAccessMode.ReadWrite);
                IOutputStream sessionOutputStream = sessionRandomAccess.GetOutputStreamAt(0);

                //Using DataContractSerializer , look at the cat-class
                var sessionSerializer = new DataContractSerializer(typeof(List<object>), new Type[] { typeof(T) });
                sessionSerializer.WriteObject(sessionOutputStream.AsStreamForWrite(), this);

                //Using XmlSerializer , look at the Dog-class
                //var serializer = new XmlSerializer(typeof (List<object>), new Type[] {typeof (T)});
                //serializer.Serialize(sessionOutputStream.AsStreamForWrite(), this);
                //sessionRandomAccess.Dispose();
                await sessionOutputStream.FlushAsync();
                sessionOutputStream.Dispose();
        }

        async public Task RestoreAsync<T>()
        {
            StorageFile sessionFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
            if (sessionFile == null)
            {
                return;
            }
            IInputStream sessionInputStream = await sessionFile.OpenReadAsync();

            //Using DataContractSerializer , look at the cat-class
            // var sessionSerializer = new DataContractSerializer(typeof(List<object>), new Type[] { typeof(T) });
            //_data = (List<object>)sessionSerializer.ReadObject(sessionInputStream.AsStreamForRead());

            //Using XmlSerializer , look at the Dog-class
            var serializer = new XmlSerializer(typeof(List<object>), new Type[] { typeof(T) });
            files.Clear();
            files = (List<StorageFile>) serializer.Deserialize(sessionInputStream.AsStreamForRead());
            sessionInputStream.Dispose();
        }

    }
}
