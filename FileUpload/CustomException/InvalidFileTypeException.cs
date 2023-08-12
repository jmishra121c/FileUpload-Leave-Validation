using FileUpload.Enum;

namespace FileUpload.CustomException
{
    public class InvalidFileTypeException : Exception
    {
        public FileType FileType { get; }

        public InvalidFileTypeException(FileType fileType, string message) : base(message)
        {
            FileType = fileType;
        }
    }
}
