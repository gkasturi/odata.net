﻿//---------------------------------------------------------------------
// <copyright file="SelectTreeNormalizerUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SelectTreeNormalizerUnitTests
    {
        [TestMethod]
        public void NormalizeTreeResultsInReversedPath()
        {
            // $select=1/2/3
            NonSystemToken endPath = new NonSystemToken("3", null, new NonSystemToken("2", null, new NonSystemToken("1", null, null)));
            SelectToken selectToken = new SelectToken(new NonSystemToken[]{endPath});
            SelectTreeNormalizer selectTreeNormalizer = new SelectTreeNormalizer();
            SelectToken normalizedToken = selectTreeNormalizer.NormalizeSelectTree(selectToken);
            normalizedToken.Properties.Single().ShouldBeNonSystemToken("1")
                           .And.NextToken.ShouldBeNonSystemToken("2")
                           .And.NextToken.ShouldBeNonSystemToken("3");
        }

        [TestMethod]
        public void NormalizeTreeWorksForMultipleTerms()
        {
            // $select=1/2/3,4/5/6
            NonSystemToken endPath = new NonSystemToken("3", null, new NonSystemToken("2", null, new NonSystemToken("1", null, null)));
            NonSystemToken endPath1 = new NonSystemToken("6", null, new NonSystemToken("5", null, new NonSystemToken("4", null, null)));
            SelectToken selectToken = new SelectToken(new NonSystemToken[]{endPath, endPath1});
            SelectTreeNormalizer selectTreeNormalizer = new SelectTreeNormalizer();
            SelectToken normalizedToken = selectTreeNormalizer.NormalizeSelectTree(selectToken);
            List<PathSegmentToken> tokens = normalizedToken.Properties.ToList();
            tokens.Should().HaveCount(2);
            tokens.ElementAt(0).ShouldBeNonSystemToken("1")
                  .And.NextToken.ShouldBeNonSystemToken("2")
                  .And.NextToken.ShouldBeNonSystemToken("3");
            tokens.ElementAt(1).ShouldBeNonSystemToken("4")
                  .And.NextToken.ShouldBeNonSystemToken("5")
                  .And.NextToken.ShouldBeNonSystemToken("6");
        }
    }
}
