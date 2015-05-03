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
    using System.Linq;
    using System.Xml.Linq;
    using Newtonsoft.Json.Linq;

    public static class JsonExtensions {
        public static JObject ToJObject(this JToken token) {
            return token as JObject ?? JObject.Parse((token ?? "{}").ToString());
        }

        public static string PropertyValue(this JProperty property) {
            var value = property.Value;

            switch (property.Value.Type) {
                case JTokenType.Array:
                    return value.FirstOrDefault().ToJObject()["@value"].Value<string>();

                default:
                    return value.Value<string>();
            }
        }

        public static string PropertyValue(this JObject obj, string prop) {
            var i = obj[prop];
            if (i != null) {
                switch (i.Type) {
                    case JTokenType.Array:
                        return i.FirstOrDefault().ToJObject()["@value"].Value<string>();

                    default:
                        return i.Value<string>();
                }
            }
            return null;
        }

        public static string Index(this JObject obj) {
            return obj.PropertyValue("@index");
        }

        public static string Val(this JObject obj) {
            return obj.PropertyValue("@value");
        }

        public static string Type(this JObject obj) {
            return obj.PropertyValue("@type");
        }

        public static string ToJsonId(this XName name, XNamespace defaultNamespace = null) {
            if (string.IsNullOrEmpty(name.NamespaceName) && defaultNamespace != null) {
                return defaultNamespace.NamespaceName + "#" + name.LocalName;
            }
            return name.NamespaceName + "#" + name.LocalName;
        }

        public static XName ToXName(this string name) {
            return (XNamespace)name.Substring(0, name.IndexOf("#")) + name.Substring(name.LastIndexOf("#") + 1);
        }
    }
}