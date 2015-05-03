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
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using JsonLD.Core;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Sgml;
    using Utility;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Swidtag : BaseElement {
        private static readonly JsonLdOptions _options = new JsonLdOptions() {
            useNamespaces = true,
            documentLoader = new ContextDownloader(),
        };

        private static JToken _context = JToken.ReadFrom(new JsonTextReader(new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.PackageManagement.SwidTag.Properties.Swidtag.context.jsonld"))));
        private readonly XDocument _swidTag;

        public Swidtag(XDocument document)
            : base(document.Root) {
            _swidTag = document;
            _swidTag.Root.SetAttributeValue(Iso19770_2.Discovery.NamespaceDeclaration, Iso19770_2.Namespace.Discovery.NamespaceName);
        }

        public Swidtag()
            : this(new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(Iso19770_2.Elements.SoftwareIdentity))) {
        }

        public Swidtag(XElement xmlDocument)
            : this(new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                xmlDocument)) {
        }

        public string SwidTagJson {
            get {
                JObject result = SetStandardContext(new JObject());
                var element = _swidTag.Root;
                if (element != null) {
                    foreach (var attr in element.Attributes().Where(attr => !attr.IsNamespaceDeclaration)) {
                        result.Add(attr.Name.ToJsonId(element.Name.Namespace), attr.Value);
                    }

                    if (Links.Any()) {
                        var lnk = new JObject();

                        foreach (var link in Links) {
                            var each = new JObject();
                            foreach (var attr in link.Element.Attributes().Where(attr => attr.Name != Iso19770_2.Attributes.HRef)) {
                                each.Add(attr.Name.ToJsonId(element.Name.Namespace), attr.Value);
                            }
                            lnk.Add(link.HRef.ToString(), each);
                        }
                        result.Add(Iso19770_2.Elements.Link.ToJsonId(), lnk);
                    }

                    if (Meta.Any()) {
                        var meta = new JObject();
                        foreach (var attr in Meta.SelectMany(m => m.Element.Attributes())) {
                            meta.Add(attr.Name.ToJsonId(element.Name.Namespace), attr.Value);
                        }
                        result.Add(Iso19770_2.Elements.Meta.ToJsonId(), meta);
                    }
                    // return result.ToString();
                    var doc = SetStandardContext(Compact(result)).ToString(Formatting.None);
                    return doc.Replace(@"""discovery:", @"""")
                        .Replace(@"""swid:", @"""")
                        .Replace(@"""install:", @"""");
                }

                return null;
            }
        }

        public string SwidTagXml {
            get {
                var stringBuilder = new StringBuilder();

                var settings = new XmlWriterSettings {
                    OmitXmlDeclaration = false,
                    Indent = true,
                    NewLineOnAttributes = true,
                    NamespaceHandling = NamespaceHandling.OmitDuplicates
                };

                using (var xmlWriter = XmlWriter.Create(stringBuilder, settings)) {
                    _swidTag.Save(xmlWriter);
                }

                return stringBuilder.ToString();
            }
        }

        public static Swidtag LoadXml(string swidTagXml) {
            try {
                using (var reader = XmlReader.Create(new StringReader(swidTagXml), new XmlReaderSettings())) {
                    var document = XDocument.Load(reader);
                    if (IsSwidtag(document.Root)) {
                        return new Swidtag(document);
                    }
                }
            } catch (Exception e) {
                e.Dump();
            }
            return null;
        }

        internal static JObject Normalize(string swidTagText) {
            return Expand(JToken.ReadFrom(new JsonTextReader(new StringReader(swidTagText))));
        }

        internal static JObject SetStandardContext(JObject obj) {
            obj.Remove("@context");
            obj.Add("@context", JToken.FromObject("http://packagemanagement.org/discovery"));
            return obj;
        }

        internal static JObject Compact(JToken swidTag) {
            // first, compact it to the canonical context
            return JsonLdProcessor.Compact(swidTag, _context, _options);
        }

        internal static JObject Expand(JToken swidTag) {
            // then, expand it out so we can walk it with strong types
            var expanded = JsonLdProcessor.Expand(Compact(swidTag), _options).FirstOrDefault().ToJObject();

            return SetStandardContext(expanded);
        }

        public static Swidtag LoadJson(string swidTagJson) {
            var swidTag = new Swidtag();
            Meta meta = null;

            var expanded = Normalize(swidTagJson);

            foreach (var member in expanded) {
                var memberName = member.Key;
                if (member.Value.Type == JTokenType.Array) {
                    foreach (var element in member.Value.Cast<JObject>()) {
                        var index = element.Index();
                        var value = element.Val();

                        if (index != null) {
                            if (value != null) {
                                if (memberName == Iso19770_2.JSonMembers.Meta) {
                                    meta = meta ?? swidTag.AddMeta();
                                    meta.AddAttribute(index, value);
                                }
                            } else {
                                if (memberName == Iso19770_2.JSonMembers.Link) {
                                    try {
                                        var href = new Uri(index);
                                        var relationship = element.PropertyValue(Iso19770_2.JSonMembers.Relationship);
                                        var link = swidTag.AddLink(href, relationship);
                                        foreach (var property in element.Properties().Where(each => each.Name != "@index" && each.Name != Iso19770_2.JSonMembers.Relationship)) {
                                            link.AddAttribute(property.Name.ToXName(), property.PropertyValue());
                                        }
                                    } catch {
                                    }
                                }
                            }
                        } else {
                            swidTag.AddAttribute(memberName.ToXName(), value);
                        }
                    }
                    continue;
                }
                // Console.WriteLine("'{0}' -- '{1}'", memberName, member.Value.Type);
            }
            return swidTag;
        }

        public static Swidtag LoadHtml(string swidTagHtml) {
            try {
                using (var reader = new SgmlReader {
                    DocType = "HTML",
                    WhitespaceHandling = WhitespaceHandling.All,
                    StripDocType = true,
                    InputStream = new StringReader(swidTagHtml),
                    CaseFolding = CaseFolding.ToLower
                }) {
                    var document = XDocument.Load(reader);

                    if (document.Root != null && document.Root.Name.LocalName == "html") {
                        var swidTag = new Swidtag {
                            Name = "Anonymous",
                            Version = "1.0",
                            VersionScheme = Iso19770_2.VersionScheme.MultipartNumeric
                        };

                        var html = document.Root;
                        var ns = html.Name.Namespace;

                        var head = html.Element(ns + "head");
                        if (head != null) {
                            var links = head.Elements(ns + "link");

                            foreach (var link in links) {
                                var href = link.Attribute("href");
                                var rel = link.Attribute("rel");

                                if (href != null && rel != null) {
                                    var l = swidTag.AddLink(new Uri(href.Value), rel.Value);
                                    foreach (var attr in link.Attributes()) {
                                        l.AddAttribute(attr.Name, attr.Value);
                                    }
                                }
                            }
                        }
                        return swidTag;
                    }
                }
            } catch (Exception e) {
                e.Dump();
            }
            return null;
        }

        public static bool IsSwidtag(XElement xmlDocument) {
            return xmlDocument.Name == Iso19770_2.Elements.SoftwareIdentity;
        }

        public bool IsApplicable(Hashtable environment) {
            return MediaQuery.IsApplicable(AppliesToMedia, environment);
        }

        #region Attributes

        public bool? IsCorpus {
            get {
                return GetAttribute(Iso19770_2.Attributes.Corpus).IsTruePreserveNull();
            }
            internal set {
                if (value != null) {
                    AddAttribute(Iso19770_2.Attributes.Corpus, value.ToString());
                }
            }
        }

        public string Name {
            get {
                return GetAttribute(Iso19770_2.Attributes.Name);
            }
            internal set {
                AddAttribute(Iso19770_2.Attributes.Name, value);
            }
        }

        public string Version {
            get {
                return GetAttribute(Iso19770_2.Attributes.Version);
            }
            internal set {
                AddAttribute(Iso19770_2.Attributes.Version, value);
            }
        }

        public string VersionScheme {
            get {
                return GetAttribute(Iso19770_2.Attributes.VersionScheme);
            }
            internal set {
                AddAttribute(Iso19770_2.Attributes.VersionScheme, value);
            }
        }

        public string TagVersion {
            get {
                return GetAttribute(Iso19770_2.Attributes.TagVersion);
            }
            internal set {
                AddAttribute(Iso19770_2.Attributes.TagVersion, value);
            }
        }

        public string TagId {
            get {
                return GetAttribute(Iso19770_2.Attributes.TagId);
            }
            internal set {
                AddAttribute(Iso19770_2.Attributes.TagId, value);
            }
        }

        public bool? IsPatch {
            get {
                return GetAttribute(Iso19770_2.Attributes.Patch).IsTruePreserveNull();
            }
            internal set {
                if (value != null) {
                    AddAttribute(Iso19770_2.Attributes.Patch, value.ToString());
                }
            }
        }

        public bool? IsSupplemental {
            get {
                return GetAttribute(Iso19770_2.Attributes.Supplemental).IsTruePreserveNull();
            }
            internal set {
                if (value != null) {
                    AddAttribute(Iso19770_2.Attributes.Supplemental, value.ToString());
                }
            }
        }

        public string AppliesToMedia {
            get {
                return GetAttribute(Iso19770_2.Attributes.Media);
            }
            internal set {
                AddAttribute(Iso19770_2.Attributes.Media, value);
            }
        }

        #endregion

        #region Elements

        public IEnumerable<SoftwareMetadata> Meta {
            get {
                return Element.Elements(Iso19770_2.Elements.Meta).Select(each => new SoftwareMetadata(each)).ReEnumerable();
            }
        }

        internal SoftwareMetadata AddMeta() {
            return AddElement(new SoftwareMetadata());
        }

        public IEnumerable<Link> Links {
            get {
                return Element.Elements(Iso19770_2.Elements.Link).Select(each => new Link(each)).ReEnumerable();
            }
        }

        internal Link AddLink(Uri referenceUri, string relationship) {
            return AddElement(new Link(referenceUri, relationship));
        }

        public IEnumerable<Entity> Entities {
            get {
                return Element.Elements(Iso19770_2.Elements.Entity).Select(each => new Entity(each)).ReEnumerable();
            }
        }

        internal Entity AddEntity(string name, string regId, string role) {
            return AddElement(new Entity(name, regId, role));
        }

        /// <summary>
        ///     A ResourceCollection for the 'Payload' of the SwidTag.
        ///     This value is null if the swidtag does not contain a Payload element.
        ///     from swidtag XSD:
        ///     The items that may be installed on a device when the software is
        ///     installed.  Note that Payload may be a superset of the items
        ///     installed and, depending on optimization systems for a device,
        ///     may or may not include every item that could be created or
        ///     executed on a device when software is installed.
        ///     In general, payload will be used to indicate the files that
        ///     may be installed with a software product and will often be a
        ///     superset of those files (i.e. if a particular optional
        ///     component is not installed, the files associated with that
        ///     component may be included in payload, but not installed on
        ///     the device).
        /// </summary>
        public Payload Payload {
            get {
                return Element.Elements(Iso19770_2.Elements.Payload).Select(each => new Payload(each)).FirstOrDefault();
            }
        }

        /// <summary>
        ///     Adds a Payload resource collection element.
        /// </summary>
        /// <returns>The ResourceCollection added. If the Payload already exists, returns the current Payload.</returns>
        internal Payload AddPayload() {
            // should we just detect and add the evidence element when a provider is adding items to the evidence
            // instead of requiring someone to explicity add the element?
            if (Element.Elements(Iso19770_2.Elements.Payload).Any()) {
                return Payload;
            }
            return AddElement(new Payload());
        }

        /// <summary>
        ///     An Evidence element representing the discovered for a swidtag.
        ///     This value is null if the swidtag does not contain an Evidence element
        ///     from swidtag XSD:
        ///     The element is used to provide results from a scan of a system
        ///     where software that does not have a SWID tag is discovered.
        ///     This information is not provided by the software creator, and
        ///     is instead created when a system is being scanned and the
        ///     evidence for why software is believed to be installed on the
        ///     device is provided in the Evidence element.
        /// </summary>
        public Evidence Evidence {
            get {
                return Element.Elements(Iso19770_2.Elements.Evidence).Select(each => new Evidence(each)).FirstOrDefault();
            }
        }

        /// <summary>
        ///     Adds an Evidence element.
        /// </summary>
        /// <returns>The added Evidence element. If the Evidence element already exists, returns the current element.</returns>
        internal Evidence AddEvidence() {
            // should we just detect and add the evidence element when a provider is adding items to the evidence
            // instead of requiring someone to explicity add the element?
            if (Element.Elements(Iso19770_2.Elements.Evidence).Any()) {
                return Evidence;
            }
            return AddElement(new Evidence());
        }

        #endregion
    }
}