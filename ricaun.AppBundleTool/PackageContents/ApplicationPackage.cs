using System;
using System.IO;
using System.Xml.Serialization;

namespace ricaun.AppBundleTool.PackageContents
{
    /// <summary>
    /// Represents an application package with details about the schema version, product, name, version, type, and code.
    /// </summary>
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class ApplicationPackage
    {
        /// <summary>
        /// Gets or sets the schema version of the application package.
        /// </summary>
        [XmlAttribute]
        public string SchemaVersion { get; set; }

        /// <summary>
        /// Gets or sets the Autodesk product associated with the application package.
        /// </summary>
        [XmlAttribute]
        public string AutodeskProduct { get; set; }

        /// <summary>
        /// Gets or sets the name of the application package.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version of the application.
        /// </summary>
        [XmlAttribute]
        public string AppVersion { get; set; }

        /// <summary>
        /// Gets or sets the type of the product.
        /// </summary>
        [XmlAttribute]
        public string ProductType { get; set; }

        /// <summary>
        /// Gets or sets the product code of the application package.
        /// </summary>
        [XmlAttribute]
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the company details associated with the application package.
        /// </summary>
        [XmlElement]
        public CompanyDetails CompanyDetails { get; set; }

        /// <summary>
        /// Gets or sets the components of the application package.
        /// </summary>
        [XmlElement]
        public Component[] Components { get; set; }

        /// <summary>
        /// Parses the application package from the specified XML file.
        /// </summary>
        /// <param name="pathPackageContents">The path to the XML file containing the application package details.</param>
        /// <returns>The parsed <see cref="ApplicationPackage"/> object, or null if the file does not exist or the path is invalid.</returns>
        public static ApplicationPackage Parse(string pathPackageContents)
        {
            if (string.IsNullOrWhiteSpace(pathPackageContents)) return null;
            if (!File.Exists(pathPackageContents)) return null;

            XmlSerializer serializer = new XmlSerializer(typeof(ApplicationPackage));
            using (FileStream fileStream = new FileStream(pathPackageContents, FileMode.Open))
            {
                return (ApplicationPackage)serializer.Deserialize(fileStream);
            }
        }
    }
}