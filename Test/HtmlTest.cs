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

                Console.WriteLine("Before");
                foreach (var s in Swidtag.testing(  System.IO.File.ReadAllText("Samples\\SimpleTags.html")) ){
                    Console.WriteLine(s);
                }
                Console.WriteLine("after");
            }
        }
    }
}