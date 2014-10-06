// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.PipelineCore.Collections;

namespace Microsoft.AspNet.Mvc
{
    public class ActionResultContext
    {
        private IHeaderDictionary _createOnReadWriteHeaders;

        internal IHeaderDictionary CreateOnReadWriteHeaders
        {
            get
            {
                return _createOnReadWriteHeaders;
            }
        }

        public int? StatusCode { get; set; }

        public IHeaderDictionary Headers
        {
            get
            {
                if (_createOnReadWriteHeaders == null)
                {
                    _createOnReadWriteHeaders = new HeaderDictionary(
                        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase));
                }

                return _createOnReadWriteHeaders;
            }
        }

        public long? ContentLength
        {
            get
            {
                if (_createOnReadWriteHeaders == null)
                {
                    return null;
                }
                else
                {
                    // Parsing Helpers are not public (yet?), for the sake of this sample
                    // temporarily hack something together
                    return ParsingHelpers.GetContentLength(Headers);
                }
            }

            set
            {
                Headers["Content-Length"] = value == null ? null : value.Value.ToString("D");
            }
        }

        public string ContentType
        {
            get
            {
                return _createOnReadWriteHeaders?["Content-Type"];
            }

            set
            {
                Headers["Content-Type"] = value;
            }
        }
    }
}
