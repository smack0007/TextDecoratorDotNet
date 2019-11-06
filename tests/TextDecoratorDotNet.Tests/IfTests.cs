using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TextDecoratorDotNet.Tests
{
    public class IfTestsContext
    {
        public bool DoIt { get; set; }
    }

    public class IfTests
    {
        [Fact]
        public void InlineIfIsExcluded()
        {
            AssertTemplate.Equal(
@"You are ready!",
@"You are @if(DoIt) {not }ready!",
                new IfTestsContext()
                {
                    DoIt = false
                });
        }

        [Fact]
        public void InlineIfIsIncluded()
        {
            AssertTemplate.Equal(
@"You are not ready!",
@"You are @if(DoIt) {not }ready!",
                new IfTestsContext()
                {
                    DoIt = true
                });
        }

        [Fact]
        public void BlockIfIsExcluded()
        {
            AssertTemplate.Equal(
@"You
are
ready!",
@"You
are
@if(DoIt)
{
not
}ready!",
                new IfTestsContext()
                {
                    DoIt = false
                });
        }

        [Fact]
        public void BlockIfIsIncluded()
        {
            AssertTemplate.Equal(
@"You
are
not
ready!",
@"You
are
@if(DoIt)
{
not
}ready!",
                new IfTestsContext()
                {
                    DoIt = true
                });
        }
    }
}
