namespace RpiHelpers.Models
{
    internal class FilePayloadModel : PayloadModel
    {
        public FilePayloadModel(string fullPath, string name)
            : base(fullPath, name)
        {
        }

        public override bool IsDirectory => false;
        public override bool IsFile => true;
    }
}
