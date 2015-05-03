// 
//  Copyright (c) Microsoft Corporation. All rights reserved. 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//  

namespace Microsoft.PackageManagement.SwidTag {
    using System;
    using System.ComponentModel.Design;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Xml.Linq;
    using Utility;

    public static class Iso19770_2 {
        public static XAttribute SwidtagNamespace {
            get {
                return new XAttribute(XNamespace.Xmlns + "swid", Namespace.Iso19770_2);
            }
        }

        /// <summary>
        ///     Gets the attribute value for a given element.
        /// </summary>
        /// <param name="element">the element that possesses the attribute</param>
        /// <param name="attribute">the attribute to find</param>
        /// <returns>the string value of the element. Returns null if the element or attribute does not exist.</returns>
        public static string GetAttribute(this XElement element, XName attribute) {
            if (element == null || attribute == null || string.IsNullOrWhiteSpace(attribute.ToString())) {
                return null;
            }
            var a = element.Attribute(attribute);
            return a == null ? null : a.Value;
        }

        /// <summary>
        ///     Adds a new attribute to the element
        ///     Does not permit modification of an existing attribute.
        ///     Does not add empty or null attributes or values.
        /// </summary>
        /// <param name="element">The element to add the attribute to</param>
        /// <param name="attribute">The attribute to add</param>
        /// <param name="value">the value of the attribute to add</param>
        /// <returns>The element passed in. (Permits fluent usage)</returns>
        public static XElement AddAttribute(this XElement element, XName attribute, string value) {
            if (element == null) {
                return null;
            }

            // we quietly ignore attempts to add empty data or attributes.
            if (string.IsNullOrWhiteSpace(value) || attribute == null || string.IsNullOrWhiteSpace(attribute.ToString())) {
                return element;
            }

            // Swidtag attributes can be added but not changed -- if it already exists, that's not permitted.
            var current = element.GetAttribute(attribute);
            if (!string.IsNullOrWhiteSpace(current)) {
                if (value != current) {
                    throw new Exception("Attempt to change Attribute '{0}' present in element '{1}'".format(attribute.LocalName, element.Name.LocalName));
                }

                // if the value was set to that already, don't worry about it.
                return element;
            }

            if (element.Name.Namespace == attribute.Namespace || string.IsNullOrWhiteSpace(attribute.NamespaceName) || attribute.Namespace == Namespace.XmNs || attribute.Namespace == Namespace.Xml) {
                element.SetAttributeValue(attribute.LocalName, value);
            } else {
                element.EnsureNamespaceAtTop(attribute.Namespace);
                element.SetAttributeValue(attribute, value);
            }

            return element;
        }

        private static int x = 0;

        public static void EnsureNamespaceAtTop(this XElement element, XNamespace ns) {
            if (string.IsNullOrEmpty(ns.NamespaceName)) {
                return;
            }
            while (true) {
                if (element.Parent != null) {
                    element = element.Parent;
                    continue;
                }
                if (element.Attributes().Any(each => each.Value == ns.NamespaceName)) {
                    return;
                }
                element.SetAttributeValue(Namespace.XmNs + ("pp" + (++x)), ns.NamespaceName);
                break;
            }
        }

        public static class Attributes {
            public static readonly XName Name = "name";
            public static readonly XName Patch = "patch";
            public static readonly XName Media = "media";
            public static readonly XName Supplemental = "supplemental";
            public static readonly XName TagVersion = "tagVersion";
            public static readonly XName TagId = "tagId";
            public static readonly XName Version = "version";
            public static readonly XName VersionScheme = "versionScheme";
            public static readonly XName Corpus = "corpus";
            public static readonly XName Summary = "summary";
            public static readonly XName Description = "description";
            public static readonly XName ActivationStatus = "activationStatus";
            public static readonly XName ChannelType = "channelType";
            public static readonly XName ColloquialVersion = "colloquialVersion";
            public static readonly XName Edition = "edition";
            public static readonly XName EntitlementDataRequired = "entitlementDataRequired";
            public static readonly XName EntitlementKey = "entitlementKey";
            public static readonly XName Generator = "generator";
            public static readonly XName PersistentId = "persistentId";
            public static readonly XName Product = "product";
            public static readonly XName ProductFamily = "productFamily";
            public static readonly XName Revision = "revision";
            public static readonly XName UnspscCode = "unspscCode";
            public static readonly XName UnspscVersion = "unspscVersion";
            public static readonly XName RegId = "regId";
            public static readonly XName Role = "role";
            public static readonly XName Thumbprint = "thumbprint";
            public static readonly XName HRef = "href";
            public static readonly XName Relationship = "rel";
            public static readonly XName MediaType = "type";
            public static readonly XName Ownership = "ownership";
            public static readonly XName Use = "use";
            public static readonly XName Artifact = "artifact";
            public static readonly XName Type = "type";
            public static readonly XName Key = "key";
            public static readonly XName Root = "root";
            public static readonly XName Location = "location";
            public static readonly XName Size = "size";
            public static readonly XName Pid = "pid";
            public static readonly XName Date = "date";
            public static readonly XName DeviceId = "deviceId";
            public static readonly XName XmlLang = Namespace.Xml + "lang";
        }

        public static class Discovery {
            public static readonly XName NamespaceDeclaration = Namespace.XmNs + "discovery";

            public static readonly XName Name = Namespace.Discovery + "name";
            // Feed Link Extended attributes: 
            public static readonly XName MinimumName = Namespace.Discovery + "min-name";
            public static readonly XName MaximumName = Namespace.Discovery + "max-name";
            public static readonly XName MinimumVersion = Namespace.Discovery + "min-version";
            public static readonly XName MaximumVersion = Namespace.Discovery + "max-version";
            public static readonly XName Keyword = Namespace.Discovery + "keyword";
            // Package Link Extended Attributes 
            public static readonly XName Version = Namespace.Discovery + "version";
            public static readonly XName Latest = Namespace.Discovery + "latest";
            public static readonly XName TargetFilename = Namespace.Discovery + "targetFilename";
            public static readonly XName Type = Namespace.Discovery + "type";
        }

        public static class MediaType {
            public const string PackageReference = "application/vnd.packagemanagement-canonicalid";
            public const string SwidTagXml = "application/swid-tag+xml";
            public const string SwidTagJsonLd = "application/swid-tag+json";
            public const string MsiPackage = "application/vnd.ms.msi-package";
            public const string MsuPackage = "application/vnd.ms.msu-package";
            public const string ExePackage = "application/vnd.packagemanagement.exe-package";
            public const string NuGetPackage = "application/vnd.packagemanagement.nuget-package";
            public const string ChocolateyPackage = "application/vnd.packagemanagement.chocolatey-package";
        }

        public static class Relationship {
            public const string Requires = "requires";
            public const string InstallationMedia = "installationmedia";
            public const string Component = "component";
            public const string Supplemental = "supplemental";
            public const string Parent = "parent";
            public const string Ancestor = "ancestor";
            // Package Discovery Relationships:
            public const string Feed = "feed"; // should point to a swidtag the represents a feed of packages
            public const string Package = "package"; // should point ot a swidtag that represents an installation package
        }

        public static class Role {
            public const string Aggregator = "aggregator";
            public const string Distributor = "distributor";
            public const string Licensor = "licensor";
            public const string SoftwareCreator = "softwareCreator";
            public const string Author = "author";
            public const string Contributor = "contributor";
            public const string Publisher = "publisher";
            public const string TagCreator = "tagCreator";
        }

        public static class Use {
            public const string Required = "required";
            public const string Recommended = "recommended";
            public const string Optional = "optional";
        }

        public static class VersionScheme {
            public const string Alphanumeric = "alphanumeric";
            public const string Decimal = "decimal";
            public const string MultipartNumeric = "multipartnumeric";
            public const string MultipartNumericPlusSuffix = "multipartnumeric+suffix";
            public const string SemVer = "semver";
            public const string Unknown = "unknown";
        }

        public static class Ownership {
            public const string Abandon = "abandon";
            public const string Private = "private";
            public const string Shared = "shared";
        }

        public static class Namespace {
            public static readonly XNamespace Iso19770_2 = XNamespace.Get("http://standards.iso.org/iso/19770/-2/2015/schema.xsd");
            public static readonly XNamespace Discovery = XNamespace.Get("http://packagemanagement.org/discovery");
            public static readonly XNamespace OneGet = XNamespace.Get("http://oneget.org/packagemanagement");
            public static readonly XNamespace Xml = XNamespace.Get("http://www.w3.org/XML/1998/namespace");
            public static XNamespace XmlDsig = XNamespace.Get("http://www.w3.org/2000/09/xmldsig#");
            public static XNamespace XmNs = XNamespace.Get("http://www.w3.org/2000/xmlns/");
        }

        public static class Elements {
            public static readonly XName SoftwareIdentity = Namespace.Iso19770_2 + "SoftwareIdentity";
            public static readonly XName Entity = Namespace.Iso19770_2 + "Entity";
            public static readonly XName Link = Namespace.Iso19770_2 + "Link";
            public static readonly XName Evidence = Namespace.Iso19770_2 + "Evidence";
            public static readonly XName Payload = Namespace.Iso19770_2 + "Payload";
            public static readonly XName Meta = Namespace.Iso19770_2 + "Meta";
            public static readonly XName Directory = Namespace.Iso19770_2 + "Directory";
            public static readonly XName File = Namespace.Iso19770_2 + "File";
            public static readonly XName Process = Namespace.Iso19770_2 + "Process";
            public static readonly XName Resource = Namespace.Iso19770_2 + "Resource";

            public static readonly XName[] MetaElements = {
                Meta, Directory, File, Process, Resource
            };
        }

        public static class JSonMembers {
            public static readonly string SoftwareIdentity = Elements.SoftwareIdentity.ToJsonId();
            public static readonly string Entity = Elements.Entity.ToJsonId();
            public static readonly string Link = Elements.Link.ToJsonId();
            public static readonly string Evidence = Elements.Evidence.ToJsonId();
            public static readonly string Payload = Elements.Payload.ToJsonId();
            public static readonly string Meta = Elements.Meta.ToJsonId();
            public static readonly string Directory = Elements.Directory.ToJsonId();
            public static readonly string File = Elements.File.ToJsonId();
            public static readonly string Process = Elements.Process.ToJsonId();
            public static readonly string Resource = Elements.Resource.ToJsonId();

            public static readonly string Name = Namespace.Iso19770_2.ToString() + "#"+ "name";
            public static readonly string Patch = Namespace.Iso19770_2.ToString() + "#"+  "patch";
            public static readonly string Media = Namespace.Iso19770_2.ToString() + "#"+  "media";
            public static readonly string Supplemental = Namespace.Iso19770_2.ToString() + "#"+  "supplemental";
            public static readonly string TagVersion = Namespace.Iso19770_2.ToString() + "#"+  "tagVersion";
            public static readonly string TagId = Namespace.Iso19770_2.ToString() + "#"+  "tagId";
            public static readonly string Version = Namespace.Iso19770_2.ToString() + "#"+  "version";
            public static readonly string VersionScheme = Namespace.Iso19770_2.ToString() + "#"+  "versionScheme";
            public static readonly string Corpus = Namespace.Iso19770_2.ToString() + "#"+  "corpus";
            public static readonly string Summary = Namespace.Iso19770_2.ToString() + "#"+  "summary";
            public static readonly string Description = Namespace.Iso19770_2.ToString() + "#"+  "description";
            public static readonly string ActivationStatus = Namespace.Iso19770_2.ToString() + "#"+  "activationStatus";
            public static readonly string ChannelType = Namespace.Iso19770_2.ToString() + "#"+  "channelType";
            public static readonly string ColloquialVersion = Namespace.Iso19770_2.ToString() + "#"+  "colloquialVersion";
            public static readonly string Edition = Namespace.Iso19770_2.ToString() + "#"+  "edition";
            public static readonly string EntitlementDataRequired = Namespace.Iso19770_2.ToString() + "#"+  "entitlementDataRequired";
            public static readonly string EntitlementKey = Namespace.Iso19770_2.ToString() + "#"+  "entitlementKey";
            public static readonly string Generator = Namespace.Iso19770_2.ToString() + "#"+  "generator";
            public static readonly string PersistentId = Namespace.Iso19770_2.ToString() + "#"+  "persistentId";
            public static readonly string Product = Namespace.Iso19770_2.ToString() + "#"+  "product";
            public static readonly string ProductFamily = Namespace.Iso19770_2.ToString() + "#"+  "productFamily";
            public static readonly string Revision = Namespace.Iso19770_2.ToString() + "#"+  "revision";
            public static readonly string UnspscCode = Namespace.Iso19770_2.ToString() + "#"+  "unspscCode";
            public static readonly string UnspscVersion = Namespace.Iso19770_2.ToString() + "#"+  "unspscVersion";
            public static readonly string RegId = Namespace.Iso19770_2.ToString() + "#"+  "regId";
            public static readonly string Role = Namespace.Iso19770_2.ToString() + "#"+  "role";
            public static readonly string Thumbprint = Namespace.Iso19770_2.ToString() + "#"+  "thumbprint";
            public static readonly string HRef = Namespace.Iso19770_2.ToString() + "#"+  "href";
            public static readonly string Relationship = Namespace.Iso19770_2.ToString() + "#"+  "rel";
            public static readonly string MediaType = Namespace.Iso19770_2.ToString() + "#"+  "type";
            public static readonly string Ownership = Namespace.Iso19770_2.ToString() + "#"+  "ownership";
            public static readonly string Use = Namespace.Iso19770_2.ToString() + "#"+  "use";
            public static readonly string Artifact = Namespace.Iso19770_2.ToString() + "#"+  "artifact";
            public static readonly string Type = Namespace.Iso19770_2.ToString() + "#"+  "type";
            public static readonly string Key = Namespace.Iso19770_2.ToString() + "#"+  "key";
            public static readonly string Root = Namespace.Iso19770_2.ToString() + "#"+  "root";
            public static readonly string Location = Namespace.Iso19770_2.ToString() + "#"+  "location";
            public static readonly string Size = Namespace.Iso19770_2.ToString() + "#"+  "size";
            public static readonly string Pid = Namespace.Iso19770_2.ToString() + "#"+  "pid";
            public static readonly string Date = Namespace.Iso19770_2.ToString() + "#"+  "date";
            public static readonly string DeviceId = Namespace.Iso19770_2.ToString() + "#"+  "deviceId";

        }
    }
}