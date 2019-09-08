namespace RpiHelpers.Models
{
    internal class DirectoryPayloadModel : PayloadModel
    {
        public DirectoryPayloadModel(string fullPath, string name)
            : base(fullPath, name)
        {
        }

        public override bool IsDirectory => true;
        public override bool IsFile => false;
    }
}
