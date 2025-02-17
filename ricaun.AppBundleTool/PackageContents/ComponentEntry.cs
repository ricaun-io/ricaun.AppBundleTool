using System.Xml.Serialization;

namespace ricaun.AppBundleTool.PackageContents
{
    /// <summary>
    /// Represents an entry of a component in the application package.
    /// </summary>
    [XmlType]
    public class ComponentEntry
    {
        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        [XmlAttribute]
        public string AppName { get; set; }

        /// <summary>
        /// Gets or sets the name of the module.
        /// </summary>
        [XmlAttribute]
        public string ModuleName { get; set; }
    }
}