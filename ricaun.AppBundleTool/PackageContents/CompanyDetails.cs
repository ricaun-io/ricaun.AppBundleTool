using System.Xml.Serialization;

namespace ricaun.AppBundleTool.PackageContents
{
    /// <summary>
    /// Represents the details of a company associated with the application package.
    /// </summary>
    [XmlType]
    public partial class CompanyDetails
    {
        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }
    }
}