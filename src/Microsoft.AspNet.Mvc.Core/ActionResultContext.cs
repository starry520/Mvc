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
        private IHeaderDictionary _lazyHeaders;

        internal IHeaderDictionary LazyHeaders
        {
            get
            {
                return _lazyHeaders;
            }
        }

        public int? StatusCode { get; set; }

        public IHeaderDictionary Headers
        {
            get
            {
                if (_lazyHeaders == null)
                {
                    _lazyHeaders = new HeaderDictionary(
                        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase));
                }

                return _lazyHeaders;
            }
        }

        // TODO make these be backed by the actual headers.
        public long? ContentLength { get; set; }
        public string ContentType { get; set; }
    }
}
