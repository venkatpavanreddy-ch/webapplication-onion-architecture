namespace WebAppication.Core.Models
{
    public class FolderTypesDTO
    {
        public FolderTypesDTO()
        {
            this.PublicPath = string.Empty;
            this.RootFolderPath = string.Empty;
        }

        public string PublicPath { get; set; }
        public string RootFolderPath { get; set; }
    }
}
