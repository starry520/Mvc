// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.Mvc
{
    public class MockActionBindingContextAccessor : IContextAccessor<ActionBindingContext>
    {
        public ActionBindingContext Value { get; set; }

        public IDisposable SetContextSource(Func<ActionBindingContext> access, Func<ActionBindingContext, ActionBindingContext> exchange)
        {
            throw new NotImplementedException();
        }

        public ActionBindingContext SetValue(ActionBindingContext value)
        {
            var oldValue = Value;
            Value = value;
            return oldValue;
        }
    }
}