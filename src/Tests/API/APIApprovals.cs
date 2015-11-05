﻿namespace NServiceBus.Azure.WindowsAzureServiceBus.Tests.API
{
    using System.IO;
    using System.Runtime.CompilerServices;
    using ApiApprover;
    using ApprovalTests;
    using ApprovalTests.Reporters;
    using Mono.Cecil;
    using NUnit.Framework;

    [TestFixture]
    public class APIApprovals
    {
        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
        public void ApproveAzureServiceBusTransport()
        {
            var assemblyPath = Path.GetFullPath(typeof(AzureServiceBusTransport).Assembly.Location);
            var asm = AssemblyDefinition.ReadAssembly(assemblyPath);
            var publicApi = PublicApiGenerator.CreatePublicApiForAssembly(asm, definition => true, false);
            Approvals.Verify(publicApi);
        }
    }
}