using System.Xml.Serialization;

namespace ricaun.AppBundleTool.PackageContents
{
    /// <summary>
    /// Represents a component of the application package.
    /// </summary>
    [XmlType]
    public class Component
    {
        /// <summary>
        /// Gets or sets the description of the component.
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the runtime requirements for the component.
        /// </summary>
        [XmlElement]
        public RuntimeRequirements RuntimeRequirements { get; set; }

        /// <summary>
        /// Gets or sets the entries of the component.
        /// </summary>
        [XmlElement]
        public ComponentEntry[] ComponentEntry { get; set; }
    }
}