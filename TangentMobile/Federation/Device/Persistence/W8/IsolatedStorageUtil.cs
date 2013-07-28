using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.Storage.Streams;
namespace TheFactorM.Federation.Persistence
{
    internal partial class IsolatedStorageUtil<TDataType>
    {
        private JsonSerializer _serializer;
        private StorageFolder _StorageFolder;

        partial void InternalIsolatedStorageUtil()
        {
            _serializer = new JsonSerializer();
        }

        private StorageFolder IsolatedStorageFolder
        {
            get
            {
                if (_StorageFolder == null)
                {
                    var applicationData = Windows.Storage.ApplicationData.Current;
                    _StorageFolder = applicationData.LocalFolder;

                }
                return _StorageFolder;
            }
        }



        partial void internalFileExisits(string fileName, string directoryName, ref bool returnValue)
        {
            string targetFileName = directoryName == null ? fileName : String.Format("{0}/{1}", directoryName, fileName);
            // can't use await here, because that leaks the signature to the partial, that will not work with older version of C#
            var exists = internalFileExisitsAsync(fileName, directoryName);
            exists.Wait();

            returnValue = exists.Result;
        }

        async private Task<bool> internalFileExisitsAsync(string fileName, string folderName)
        {
            try 
            { 
                StorageFolder folder;
                if(folderName != null)
                {
                 folder = await IsolatedStorageFolder.GetFolderAsync(folderName);
                }
                else
                {
                    folder = IsolatedStorageFolder;
                }

                var query  = folder.CreateFileQuery(new CommonFileQuery());
                var files =  await query.GetFilesAsync();
                return (files.Where(file => file.Name == fileName).Count() >0);
            } 
            catch (FileNotFoundException) 
            { 
                return false;
            } 
        }

        partial void InternalDirectoryExists(string directoryName, ref bool returnValue)
        {
            var exists = FolderExisits(directoryName);
            exists.Wait();

            returnValue = exists.Result;
        }

        async private Task<bool> FolderExisits(string folderName)
        {
            try{
                if (folderName != null)
                {

                    var folderQuery = IsolatedStorageFolder.CreateFolderQuery();
                    var folders = await folderQuery.GetFoldersAsync();

                    return (folders.Where(folder => folder.Name == folderName).Count() > 0);

                }
                else
                    return false;
            }
            catch(FileNotFoundException)
            {
                return false;
            }
             
        }

        //Get all filenames in a given directory
        partial void InternalGetFileNames(string directoryName, string fileType, ref string[] fileNames)
        {
            fileNames = new string[0];
            var files = GetFiles(directoryName, fileType);
            files.Wait();
            fileNames = files.Result;
        }

        private async Task<string[]> GetFiles(string directoryName, string fileType)
        {
			var folderexists = await FolderExisits(directoryName);
            if (folderexists)
            {
                var storageFiles = await IsolatedStorageFolder.GetFilesAsync();
                return storageFiles.Select(f => f.Name).ToArray();
            }
            else
            {
                return null;
            }
        }

        //Save data to the isolated storage, the data must be serializable
        partial void InternalSaveData(TDataType sourceData, String fileName, string directoryName)
        {
            string targetFileName = directoryName == null ? fileName : String.Format("{0}/{1}", directoryName, fileName);

            InternalSaveDataAsync(sourceData, directoryName, targetFileName);
        }

        private async void InternalSaveDataAsync(TDataType sourceData, string directoryName, string targetFileName)
        {
            if (directoryName != null && !await FolderExisits(directoryName))
            {

                await IsolatedStorageFolder.CreateFolderAsync(directoryName);
            }
            //Create directory if needed

            try
            {
                var targetFile = await IsolatedStorageFolder.CreateFileAsync(targetFileName, CreationCollisionOption.ReplaceExisting);
                //Serialize file data

                IRandomAccessStream writeStream = await targetFile.OpenAsync(FileAccessMode.ReadWrite);
                using (IOutputStream outputStream = writeStream.GetOutputStreamAt(0))
                {
                    using (var w = new StreamWriter(outputStream.AsStreamForWrite()))
                    {
                        using (var j = new JsonTextWriter(w))
                        {
                            _serializer.Serialize(j, sourceData);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //If exception just delete file
                var result = DeleteFile(targetFileName);
                result.Wait();
            }
        }

        private async Task DeleteFile(string targetFileName)
        {
            var file = await IsolatedStorageFolder.GetFileAsync(targetFileName);
            await file.DeleteAsync();
        }

        //Load data from storage, retrieved the deserialized object
        partial void InternalLoadData(string fileName, string directoryName, ref TDataType returnValue)
        {

            string targetFileName = directoryName == null ? fileName : String.Format("{0}/{1}", directoryName, fileName);
            var result = InternalLoadDataAsync(targetFileName);
            result.Wait();

            returnValue = result.Result;
        }

        private async Task<TDataType> InternalLoadDataAsync(string targetFileName)
        {
            TDataType returnValue = default(TDataType);
            if (await internalFileExisitsAsync(targetFileName, null))
                try
                {
                    var targetFile = await IsolatedStorageFolder.CreateFileAsync(targetFileName, CreationCollisionOption.ReplaceExisting);
                    //Serialize file data

                    IRandomAccessStream readStream = await targetFile.OpenAsync(FileAccessMode.ReadWrite);

                    using (var reader = new JsonTextReader(new StreamReader(readStream.AsStreamForRead())))
                    {
                        returnValue = (TDataType)_serializer.Deserialize(reader);
                    }
                }
                catch (Exception)
                {
                    //If exception just delete file
                    var result = DeleteFile(targetFileName);
                    result.Wait();
                }
            return returnValue;
        }


        //Delete a given file
        partial void InternalDeleteFile(string fileName, string directoryName)
        {
            InternalDeleteFileAsync(fileName, directoryName);
        }

        async private void InternalDeleteFileAsync(string fileName, string directoryName)
        {
            string targetFileName = directoryName == null ? fileName : String.Format("{0}/{1}", directoryName, fileName);
            if (await internalFileExisitsAsync(fileName, directoryName))
            {
                var file = await IsolatedStorageFolder.GetFileAsync(targetFileName);
                await file.DeleteAsync();
            }
        }



    }
}
