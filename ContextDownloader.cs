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
    using System.IO;
    using JsonLD.Core;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class ContextDownloader : DocumentLoader {
        public override RemoteDocument LoadDocument(string url) {
            if (url == "http://packagemanagement.org/discovery/Meta") {
                var c = System.IO.File.ReadAllText("Samples\\Meta.context.jsonld");
                return new RemoteDocument(url, JToken.ReadFrom(new JsonTextReader(new StringReader(c))));
            }
            var context = System.IO.File.ReadAllText("Samples\\Swidtag.context.jsonld");
            return new RemoteDocument(url, JToken.ReadFrom(new JsonTextReader(new StringReader(context))));
        }
    }
}