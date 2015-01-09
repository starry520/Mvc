// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc.HeaderValueAbstractions;

namespace Microsoft.AspNet.Mvc
{
    public interface IConsumeActionConstraint : IActionConstraint
    {
        IList<MediaTypeHeaderValue> ContentTypes { get; set; }
    }
}