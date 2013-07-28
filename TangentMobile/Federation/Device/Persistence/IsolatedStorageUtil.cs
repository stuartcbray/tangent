using System;
using System.IO;
using System.Runtime.Serialization;

namespace TheFactorM.Federation.Persistence
{
    internal partial class IsolatedStorageUtil<TDataType> : IDisposable
    {
        public IsolatedStorageUtil()
        {
            InternalIsolatedStorageUtil();
        }

        partial void InternalIsolatedStorageUtil();
        

        //Check if file exists
        public bool FileExists(string fileName, string directoryName)
        {
            bool returnValue = false;
             internalFileExisits(fileName, directoryName, ref returnValue);
             return returnValue;
        }

        partial void internalFileExisits(string fileName, string directoryName, ref bool returnValue);


        //Check if directory exists
        public bool DirectoryExists(string directoryName)
        {
            bool returnValue = false;
             InternalDirectoryExists(directoryName, ref returnValue);
             return returnValue;
        }

        partial void InternalDirectoryExists(string directoryName, ref bool returnValue);

        //Get all filenames in a given directory
        public string[] GetFileNames(string directoryName, string fileType)
        {
            string[] returnValue = new string[0];

            InternalGetFileNames(directoryName, fileType, ref returnValue);

            return returnValue;
        }

        partial void InternalGetFileNames(string directoryName, string fileType, ref string[] fileNames);
   

        //Save data to the isolated storage, the data must be serializable
        public void SaveData(TDataType sourceData, String fileName, string directoryName)
        {
            InternalSaveData(sourceData, fileName, directoryName);
        }

        partial void InternalSaveData(TDataType sourceData, String fileName, string directoryName);

        //Load data from storage, retrieved the deserialized object
        public TDataType LoadData(string fileName, string directoryName)
        {
            TDataType returnValue = default(TDataType);
            InternalLoadData(fileName, directoryName, ref returnValue);
            return returnValue;
        }

        partial void InternalLoadData(string fileName, string directoryName, ref TDataType returnValue);

        //Delete a given file
        public void DeleteFile(string fileName, string directoryName)
        {
            InternalDeleteFile(fileName, directoryName);
        }

        partial void InternalDeleteFile(string fileName, string directoryName);

        public void Dispose()
        {
            InternalDispose();
        }

        partial void InternalDispose();
    }
 }

