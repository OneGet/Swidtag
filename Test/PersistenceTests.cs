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
    using System.Windows.Forms;
    using Support;
    using Xunit;
    using Xunit.Abstractions;

    public class PersistenceTests : Tests {
        public PersistenceTests(ITestOutputHelper outputHelper)
            : base(outputHelper) {
        }

        [Fact]
        public void ConsoleTest() {
            using (CaptureConsole) {
                var tag = Swidtag.LoadHtml(File.ReadAllText("Samples\\SimpleTags.html"));
                Assert.NotNull(tag);
                Console.WriteLine(tag.SwidTagXml);
            }
        }

        [Fact]
        public void JSonTest() {
            using (CaptureConsole) {
                var tag = Swidtag.LoadJson(File.ReadAllText("Samples\\SimpleTag.json"));
                Assert.NotNull(tag);
                Console.WriteLine(tag.SwidTagXml);
                Console.WriteLine(tag.SwidTagJson);
            }
        }


        [Fact]
        public void RemoveLinkTest() {
            using (CaptureConsole) {
                var tag = Swidtag.LoadXml(File.ReadAllText("Samples\\swid.feed.xml"));
                
                Console.WriteLine(tag.SwidTagXml);
                tag.RemoveLink(tag.Links.FirstOrDefault().HRef);
                Console.WriteLine(tag.SwidTagXml);

            }
        }

        [Fact]
        public void XmlTest() {
            using (CaptureConsole) {
                var tag = Swidtag.LoadXml(File.ReadAllText("Samples\\swid.feed.xml"));
                Assert.NotNull(tag);
                Console.WriteLine(tag.SwidTagXml);
                Console.WriteLine(tag.SwidTagJson);

            }
        }
    }
}