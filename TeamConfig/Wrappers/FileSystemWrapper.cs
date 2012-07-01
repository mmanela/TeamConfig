using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TeamConfig.Wrappers
{
    public interface IFileSystemWrapper
    {
        bool FileExists(string path);
        Stream Open(string path);
        Stream Open(string path, FileMode mode, FileAccess access);
        void Save(string path, Stream stream);
        void DeleteFile(string path);
        byte[] GetContent(Stream stream);
        void MoveFile(string sourceFilename, string destFilename);
        void CopyFile(string sourceFilename, string destFilename, bool overwrite);
        void MoveDirectory(string sourceDirectory, string destDirectory);
        DateTime GetCreationTime(string path);
        bool FolderExists(string path);
        void DeleteDirectory(string path, bool recursive);
        string GetDirectoryName(string path);
        void CreateDirectory(string path);
        string GetFileName(string path);
        IEnumerable<string> GetDirectories(string directory);
        string[] GetFiles(string path);
        DateTime GetLastAccessTime(string path);
        string GetFullPath(string path);
        string ReadAllText(string path);
        void WriteAllText(string path, string text);
    }

    public class FileSystemWrapper : IFileSystemWrapper
    {
        public void MoveFile(string sourceFilename, string destFilename)
        {
            File.Move(sourceFilename, destFilename);
        }

        public virtual void CopyFile(string sourceFilename, string destFilename, bool overwrite)
        {
            File.Copy(sourceFilename, destFilename, overwrite);
        }

        public virtual void MoveDirectory(string sourceDirectory, string destDirectory)
        {
            if (!sourceDirectory.Equals(destDirectory, StringComparison.OrdinalIgnoreCase))
            {
                Directory.Move(sourceDirectory, destDirectory);
            }
        }

        public DateTime GetCreationTime(string path)
        {
            return File.GetCreationTime(path);
        }        
        
        public DateTime GetLastAccessTime(string path)
        {
            return File.GetLastAccessTime(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public void WriteAllText(string path, string text)
        {
            File.WriteAllText(path, text);
        }

        public bool FolderExists(string path)
        {
            return Directory.Exists(path);
        }

        public void DeleteFile(string path)
        {
            if (FileExists(path))
                File.Delete(path);
        }

        public void DeleteDirectory(string path, bool recursive)
        {
            if (FolderExists(path))
                Directory.Delete(path, recursive);
        }

        public virtual string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public IEnumerable<string> GetDirectories(string directory)
        {
            return Directory.GetDirectories(directory);
        }

        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public Stream Open(string path)
        {
            return File.Open(path, FileMode.OpenOrCreate);
        }

        public Stream Open(string path, FileMode mode, FileAccess access)
        {
            return new FileStream(path, mode, access);
        }

        public void Save(string path, Stream stream)
        {
            using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
            {
                byte[] data = GetContent(stream);
                fs.Write(data, 0, data.Length);
            }
        }

        public byte[] GetContent(Stream stream)
        {
            byte[] data = new byte[stream.Length];
            int remaining = data.Length;
            int offset = 0;

            while(remaining > 0)
            {
                int read = stream.Read(data, offset, remaining);
                remaining -= read;
                offset += read;
            }

            stream.Position = 0;

            return data;
        }

        public string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }


    }
}

