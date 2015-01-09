// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.ModelBinding.Internal;

namespace Microsoft.AspNet.Mvc.ModelBinding
{
    /// <summary>
    /// Represents a value provider that contains the parsed form values.
    /// </summary>
    public class FormCollectionValueProvider :
        MetadataAwareValueProvider<IFormDataValueProviderMetadata>,
        IEnumerableValueProvider
    {
        private readonly CultureInfo _culture;
        private PrefixContainer _prefixContainer;
        private readonly Func<Task<IFormCollection>> _valuesFactory;
        private IFormCollection _values;

        /// <summary>
        /// Creates a <see cref="FormCollectionValueProvider"/> wrapping
        /// an existing set of <see cref="IFormCollection"/>.
        /// </summary>
        /// <param name="values">The key value pairs to wrap.</param>
        /// <param name="culture">The culture to return with ValueProviderResult instances.</param>
        public FormCollectionValueProvider([NotNull] IFormCollection values, CultureInfo culture)
        {
            _values = values;
            _culture = culture;
        }

        /// <summary>
        /// Creates a <see cref="FormCollectionValueProvider"/> wrapping
        /// an existing set of <see cref="IFormCollection"/> provided by a delegate.
        /// </summary>
        /// <param name="valuesFactory">The delegate that provides the key value pairs to wrap.</param>
        /// <param name="culture">The culture to return with ValueProviderResult instances.</param>
        public FormCollectionValueProvider([NotNull] Func<Task<IFormCollection>> valuesFactory,
                                                     CultureInfo culture)
        {
            _valuesFactory = valuesFactory;
            _culture = culture;
        }

        public CultureInfo Culture
        {
            get
            {
                return _culture;
            }
        }

        /// <inheritdoc />
        public override async Task<bool> ContainsPrefixAsync(string prefix)
        {
            var prefixContainer = await GetPrefixContainerAsync();
            return prefixContainer.ContainsPrefix(prefix);
        }

        /// <inheritdoc />
        public virtual async Task<IDictionary<string, string>> GetKeysFromPrefixAsync([NotNull] string prefix)
        {
            var prefixContainer = await GetPrefixContainerAsync();
            return prefixContainer.GetKeysFromPrefix(prefix);
        }

        /// <inheritdoc />
        public override async Task<ValueProviderResult> GetValueAsync([NotNull] string key)
        {
            var collection = await GetValueCollectionAsync();
            var values = collection.GetValues(key);

            ValueProviderResult result;
            if (values == null)
            {
                result = null;
            }
            else if (values.Count == 1)
            {
                var value = (string)values[0];
                result = new ValueProviderResult(value, value, _culture);
            }
            else
            {
                result = new ValueProviderResult(values, _values.Get(key), _culture);
            }

            return result;
        }

        private async Task<IFormCollection> GetValueCollectionAsync()
        {
            if (_values == null)
            {
                Debug.Assert(_valuesFactory != null);
                _values = await _valuesFactory();
            }

            return _values;
        }

        private async Task<PrefixContainer> GetPrefixContainerAsync()
        {
            if (_prefixContainer == null)
            {
                // Initialization race is OK providing data remains read-only and object identity is not significant
                var collection = await GetValueCollectionAsync();
                _prefixContainer = new PrefixContainer(collection.Keys);
            }
            return _prefixContainer;
        }
    }
}
