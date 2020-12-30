using System.IO;

namespace TetrisPlayerWPF.Source.SettingElement
{
    public class FileSettingElement:SettingElementBase<FileInfo>
    {
        public string FileDescription{ get => _file_description; }
        private string _file_description;
        public string FileExtension { get => _file_extension; }
        private string _file_extension;
        public FileSettingElement(FileInfo default_value,string file_description,string file_extension) : base(default_value)
        {
            _file_description = file_description;
            _file_extension = file_extension;
        }
    }
}
