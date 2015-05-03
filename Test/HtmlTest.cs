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

namespace Microsoft.PackageManagement.SwidTag.Test {
    using System.IO;
    using System.Linq;
    using System.Security.Permissions;
    using JsonLD.Core;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Support;
    using Xunit;
    using Xunit.Abstractions;

    public class HtmlTest : Tests {
        public HtmlTest(ITestOutputHelper outputHelper)
            : base(outputHelper) {
        }

        [Fact]
        public void ConsoleTest() {
            using (CaptureConsole) {
                var tag = Swidtag.LoadHtml(System.IO.File.ReadAllText("Samples\\SimpleTags.html"));
                Assert.NotNull(tag);
                Console.WriteLine(tag.SwidTagXml);
            }
        }

        private static JsonLdOptions _options = new JsonLdOptions() {
            useNamespaces = true,
            documentLoader = new xxx(),
        };

        private static JToken _context = JToken.ReadFrom(new JsonTextReader(new StringReader(System.IO.File.ReadAllText("Samples\\Swidtag.context.jsonld"))));


        [Fact]
        public void JSonTest() {
            using (CaptureConsole) {


               // var src = JToken.ReadFrom(new JsonTextReader(new StringReader(System.IO.File.ReadAllText("Samples\\SimpleTag.json"))));
                Console.WriteLine(Swidtag.LoadJson(System.IO.File.ReadAllText("Samples\\SimpleTag.json")).SwidTagXml);
               
            }
        }

        [Fact]
        public void JSonTest2() {
            using (CaptureConsole) {
                /*
                var context = System.IO.File.ReadAllText("Samples\\Swidtag.jsonld");
                var source = System.IO.File.ReadAllText("Samples\\no-context.json");

                var src = JToken.ReadFrom(new JsonTextReader(new StringReader(source)));
                
                var options = new JsonLdOptions() {
                    useNamespaces = true,
                    documentLoader = new xxx()
                };
                var doc = JsonLdProcessor.Expand(src, options);
                Console.WriteLine(doc);


                var newdoc = doc.ToString();
                src = JToken.ReadFrom(new JsonTextReader(new StringReader(newdoc)));
                var con = JToken.ReadFrom(new JsonTextReader(new StringReader(context)));

                var x = JsonLdProcessor.Compact(src, con, options);
                Console.WriteLine(x);



                var terse = System.IO.File.ReadAllText("Samples\\tersetag.json");
                src = JToken.ReadFrom(new JsonTextReader(new StringReader(terse)));
                x = JsonLdProcessor.Compact(src, con, options);
                Console.WriteLine(x);
                 * */
            }
        }
    }
}