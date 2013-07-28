using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace TheFactorM.Federation.Persistence
{
    internal partial class IsolatedStorageUtil<TDataType>
    {
        private JsonSerializer _serializer;
        private IsolatedStorageFile _isolatedStorageFile;

        partial void InternalIsolatedStorageUtil()
        {
            _serializer = new JsonSerializer();
        }

        private IsolatedStorageFile IsolatedStorageFile
        {
            get { return _isolatedStorageFile ?? (_isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()); }
        }



        partial void internalFileExisits(string fileName, string directoryName, ref bool returnValue)
        {
            string targetFileName = directoryName == null ? fileName : String.Format("{0}/{1}", directoryName, fileName);
            returnValue = IsolatedStorageFile.FileExists(targetFileName);
        }

        partial void InternalDirectoryExists(string directoryName, ref bool returnValue)
        {
            returnValue = IsolatedStorageFile.DirectoryExists(directoryName);
        }

        //Get all filenames in a given directory
        partial void InternalGetFileNames(string directoryName, string fileType, ref string[] fileNames)
        {
            fileNames = new string[0];
            if (DirectoryExists(directoryName))
            {
                fileNames = IsolatedStorageFile.GetFileNames(directoryName + "/*." + fileType);
            }

        }

        //Save data to the isolated storage, the data must be serializable
        partial void InternalSaveData(TDataType sourceData, String fileName, string directoryName)
        {
            string targetFileName = directoryName == null ? fileName : String.Format("{0}/{1}", directoryName, fileName);

            if (directoryName != null && !IsolatedStorageFile.DirectoryExists(directoryName))
                //Create directory if needed
                IsolatedStorageFile.CreateDirectory(directoryName);
            try
            {
				using (var w = new StreamWriter(IsolatedStorageFile.CreateFile(targetFileName)))
                {
					using (var j = new JsonTextWriter(w))
					{
						_serializer.Serialize(j, sourceData);
					}
                }
            }
            catch (Exception)
            {
                //If exception just delete file
                IsolatedStorageFile.DeleteFile(targetFileName);
            }
        }

        //Load data from storage, retrieved the deserialized object
        partial void InternalLoadData(string fileName, string directoryName, ref TDataType returnValue)
        {
            string targetFileName = directoryName == null ? fileName : String.Format("{0}/{1}", directoryName, fileName);

            if (IsolatedStorageFile.FileExists(targetFileName))
			{
				using (var r = new StreamReader(IsolatedStorageFile.OpenFile(targetFileName, FileMode.Open)))
                {
					using (var j = new JsonTextReader(r))
					{
	                    //Deserialize file data
						returnValue = _serializer.Deserialize<TDataType>(j);
	                }
				}
			}
		}

        //Delete a given file
        partial void InternalDeleteFile(string fileName, string directoryName)
        {
            string targetFileName = directoryName == null ? fileName : String.Format("{0}/{1}", directoryName, fileName);
            if (IsolatedStorageFile.FileExists(targetFileName))
            {
                IsolatedStorageFile.DeleteFile(targetFileName);
            }
        }
    }
}
