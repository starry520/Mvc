// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.HeaderValueAbstractions;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Routing;

namespace Microsoft.AspNet.Mvc
{
    public class ConsumesAttribute : AuthorizationFilterAttribute, IConsumeActionConstraint
    {
        public ConsumesAttribute(string contentType, params string[] otherContentTypes)
        {
            ContentTypes = GetContentTypes(contentType, otherContentTypes);
        }

        public IList<MediaTypeHeaderValue> ContentTypes { get; set; }

        public override async Task OnAuthorizationAsync([NotNull] AuthorizationContext context)
        {
            await base.OnAuthorizationAsync(context);
            MediaTypeHeaderValue requestContentType = null;
            MediaTypeHeaderValue.TryParse(context.HttpContext.Request.ContentType, out requestContentType);

            if (requestContentType == null && context.ActionDescriptor.ActionConstraints.OfType<IConsumeActionConstraint>().Any())
            {

            }

            if (requestContentType != null && !ContentTypes.Any(contentType => contentType.IsSubsetOf(requestContentType)))
            {
                // short circut
                context.Result = new UnsupportedMediaTypeResult();
            }
        }

        public bool Accept(ActionConstraintContext context)
        {
            MediaTypeHeaderValue requestContentType = null;
            MediaTypeHeaderValue.TryParse(context.RouteContext.HttpContext.Request.ContentType, out requestContentType);

            if (requestContentType == null)
            {
                if (context.Candidates.Any(candidate => candidate.Constraints.OfType<IConsumeActionConstraint>().Any()))
                {
                    // there is a candidate without any action 
                }
            }

            if (requestContentType != null && ContentTypes.Any(c => c.IsSubsetOf(requestContentType)))
            {
                return true;
            }


            if (context.Candidates.First() != context.CurrentCandidate)
            {
                return false;
            }

            // Select candidates which have the consumes attribute
            var candidates = context.Candidates;//.Where(c => c.Constraints.OfType<IConsumeActionConstraint>().Any());
            var foundACandidateWithoutConstraint = false;
            foreach (var candidate in candidates.Skip(1))
            {
                var tempContext = new ActionConstraintContext()
                {
                    Candidates = context.Candidates,
                    RouteContext = context.RouteContext,
                    CurrentCandidate = candidate
                };

             
                // Harshg : we are assumming that as this is already an IConsumeConstraint all other candidates would
                // be consume constraints only.
                if (candidate.Constraints.Any(constraint => constraint.Accept(tempContext)))
                {
                    // There is someone later in the chain which can handle the request.
                    // end the process here.
                    return false;
                }
            }

            // There is no one later in the chain that can handle this content type return a false positive so that
            // later we can detect and return a 415. this is to be done only if there is no other candidate with consumes
            // constraint.
            return !foundACandidateWithoutConstraint;
        }

        private List<MediaTypeHeaderValue> GetContentTypes(string firstArg, string[] args)
        {
            var contentTypes = new List<MediaTypeHeaderValue>();
            contentTypes.Add(MediaTypeHeaderValue.Parse(firstArg));
            foreach (var item in args)
            {
                var contentType = MediaTypeHeaderValue.Parse(item);
                contentTypes.Add(contentType);
            }

            return contentTypes;
        }
    }
}