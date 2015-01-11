// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.PipelineCore;
using Microsoft.AspNet.PipelineCore.Collections;
using Moq;
using Xunit;

namespace Microsoft.AspNet.Mvc.ModelBinding.Test
{
    public class FormFileModelBinderTest
    {
        [Fact]
        public async Task FormFileModelBinder_ExpectMultipleFiles_BindSuccessful()
        {
            // Arrange
            var bindingContext = GetBindingContext(typeof(IEnumerable<IFormFile>));
            var binder = new FormFileModelBinder();

            // Act
            var retVal = await binder.BindModelAsync(bindingContext);

            // Assert
            Assert.True(retVal);
            var files = bindingContext.Model as IList<IFormFile>;
            Assert.Equal(files.Count, 2);
        }

        [Fact]
        public async Task FormFileModelBinder_ExpectSingleFile_BindFirstFile()
        {
            // Arrange
            var bindingContext = GetBindingContext(typeof(IFormFile));
            var binder = new FormFileModelBinder();

            // Act
            var retVal = await binder.BindModelAsync(bindingContext);

            // Assert
            Assert.True(retVal);
            var file = bindingContext.Model as IFormFile;
            Assert.Equal(file.ContentDisposition,
                        "form-data; name=\"file\"; filename=\"file1.txt\"");
        }

        private static ModelBindingContext GetBindingContext(Type modelType)
        {
            var metadataProvider = new EmptyModelMetadataProvider();
            var bindingContext = new ModelBindingContext
            {
                ModelMetadata = metadataProvider.GetMetadataForType(null, modelType),
                ModelName = "file",
                OperationBindingContext = new OperationBindingContext
                {
                    ModelBinder = new FormFileModelBinder(),
                    MetadataProvider = metadataProvider,
                    HttpContext = GetMockHttpContext(),
                }
            };

            return bindingContext;
        }

        private static HttpContext GetMockHttpContext()
        {
            var httpContext = new Mock<DefaultHttpContext>();
            httpContext.Setup(h => h.Request.ReadFormAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(GetMockFormCollection()));
            httpContext.Setup(h => h.Request.HasFormContentType).Returns(true);
            return httpContext.Object;
        }

        private static IFormCollection GetMockFormCollection()
        {
            var formFiles = new FormFileCollection();
            formFiles.Add(GetMockFormFile("file", "file1.txt"));
            formFiles.Add(GetMockFormFile("file", "file2.txt"));
            var formCollection = new Mock<IFormCollection>();
            formCollection.Setup(f => f.Files).Returns(formFiles);
            return formCollection.Object;
        }

        private static IFormFile GetMockFormFile(string modelName, string filename)
        {
            var formFile = new Mock<IFormFile>();
            formFile.Setup(f => f.ContentDisposition)
                .Returns(string.Format("form-data; name=\"{0}\"; filename=\"{1}\"",
                        modelName,
                        filename));
            return formFile.Object;
        }
    }
}
