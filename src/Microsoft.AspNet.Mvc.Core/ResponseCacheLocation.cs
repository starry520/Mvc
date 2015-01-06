// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// Determines the value for the "Cache-control" header in the response.
    /// </summary>
    public enum ResponseCacheLocation
    {
        // Cached in both proxies and client
        Any = 0,
        // Cached only in the client
        Client = 1,
        // "Cache-control" and "Pragma" are set to "no-cache"
        None = 2
    }
}