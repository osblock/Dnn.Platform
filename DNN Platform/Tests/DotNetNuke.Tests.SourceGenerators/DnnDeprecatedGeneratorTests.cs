// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Tests.SourceGenerators;

[TestFixture]
public class DnnDeprecatedGeneratorTests
{
    [Test]
    public async Task NotDeprecatedClass_DoesNotGenerateAnything()
    {
        await Verify("""
namespace Example.Test;

using DotNetNuke.Internal.SourceGenerators;

public partial class PagesController
{
}

""");
    }

    [Test]
    public async Task DeprecatedNonPartialClass_ReportsAnErrorDiagnostic()
    {
        await Verify("""
namespace Example.Test;

using DotNetNuke.Internal.SourceGenerators;

[DnnDeprecated(10, 0, 0, "Please resolve IPagesController via dependency injection.")]
public class PagesController
{
}

""");
    }

    [Test]
    public async Task DeprecatedPartialClass_AddsPartialWithObsoleteAttribute()
    {
        await Verify("""
namespace Example.Test;

using DotNetNuke.Internal.SourceGenerators;

[DnnDeprecated(10, 0, 0, "Please resolve IPagesController via dependency injection.")]
public partial class PagesController
{
}

""");
    }

    [Test]
    public async Task DeprecatedPartialInterface_AddsPartialWithObsoleteAttribute()
    {
        await Verify("""
namespace Example.Test;

using DotNetNuke.Internal.SourceGenerators;

[DnnDeprecated(10, 0, 0, "Please use the other IPagesController.")]
public partial interface IPagesController
{
}

""");
    }

    [Test]
    public async Task DeprecatedPartialStruct_AddsPartialWithObsoleteAttribute()
    {
        await Verify("""
namespace Example.Test;

using DotNetNuke.Internal.SourceGenerators;

[DnnDeprecated(10, 0, 0, "Please use PageInfo.")]
public partial struct Page
{
}

""");
    }

    [Test]
    public async Task DeprecatedPartialRecord_AddsPartialWithObsoleteAttribute()
    {
        await Verify("""
namespace Example.Test;

using DotNetNuke.Internal.SourceGenerators;

[DnnDeprecated(10, 0, 0, "Please use PageInfo.")]
public partial record Page
{
}

""");
    }

    [Test]
    public async Task DeprecatedNestedPartialRecord_AddsPartialWithObsoleteAttribute()
    {
        await Verify("""
namespace Example.Test;

using DotNetNuke.Internal.SourceGenerators;

public partial class Page
{
    [DnnDeprecated(9, 1, 2, "Please use InnerPageInfo.")]
    public partial record InnerPage
    {
    }
}

""");
    }

    [Test]
    public async Task DeprecatedMethods_AddsPartialWithObsoleteAttribute()
    {
        await Verify("""
namespace Example.Test;

using System;
using DotNetNuke.Internal.SourceGenerators;

internal partial class Page
{
    internal partial static class StaticWrapper
    {
        [DnnDeprecated(8, 4, 4, "Use overload taking IServiceProvider.")]
        public static partial void DoAThing(string i)
        {
            return i;
        }

        [DnnDeprecated(9, 4, 4, "Use overload taking IApplicationStatusInfo.")]
        public static partial int?[] GetTheseThings(int? a, int b)
        {
            return new[] { a, b, };
        }
    }

    internal partial class Wrapper
    {
        [DnnDeprecated(8, 4, 4, "Use overload taking IApplicationStatusInfo.")]
        public partial (decimal, Int32) GetThemBoth(decimal x)
        {
            return (x + 1, 1);
        }

        [DnnDeprecated(9, 4, 4, "Use overload taking IServiceProvider.")]
        public static partial System.Text.StringBuilder CombineThings(string y, String z)
        {
            return new StringBuilder(y + z);
        }
    }
}

""");
    }

    private static async Task Verify(string source)
    {
        var references =
            AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .Append(MetadataReference.CreateFromFile(typeof(DnnDeprecatedAttribute).Assembly.Location));
        var compilation = CSharpCompilation.Create(
            "AnAssemblyName",
            new[] { CSharpSyntaxTree.ParseText(source), },
            references);
        var driver =
            CSharpGeneratorDriver
                .Create(new DnnDeprecatedGenerator())
                .RunGenerators(compilation);
        await Verifier.Verify(driver).UseDirectory("Snapshots");
    }
}
