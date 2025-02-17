using System.Xml.Serialization;

namespace ricaun.AppBundleTool.PackageContents
{
    /// <summary>
    /// Represents the runtime requirements for a component in the application package.
    /// </summary>
    [XmlType]
    public class RuntimeRequirements
    {
        /// <summary>
        /// Gets or sets the platform for the runtime requirements.
        /// </summary>
        [XmlAttribute]
        public string Platform { get; set; }

        /// <summary>
        /// Gets or sets the operating system for the runtime requirements.
        /// </summary>
        [XmlAttribute]
        public string OS { get; set; }

        /// <summary>
        /// Gets or sets the minimum series version for the runtime requirements.
        /// </summary>
        [XmlAttribute]
        public string SeriesMin { get; set; }

        /// <summary>
        /// Gets or sets the maximum series version for the runtime requirements.
        /// </summary>
        [XmlAttribute]
        public string SeriesMax { get; set; }
    }
}