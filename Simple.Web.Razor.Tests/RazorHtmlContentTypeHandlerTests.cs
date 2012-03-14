﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Web.Razor.Tests
{
    using System.IO;
    using Xunit;

    public class RazorHtmlContentTypeHandlerTests
    {
        private const string TemplateText = @"@model Simple.Web.Razor.Tests.TestModel
<!DOCTYPE html><html><body>@Model.Text</body></html>";

        [Fact]
        public void GetsAType()
        {
            Type type;
            using (var reader = new StringReader(TemplateText))
            {
                type = new RazorHtmlContentTypeHandler().CreateType(reader, typeof (TestModel));
            }
            Assert.NotNull(type);
        }

        [Fact]
        public void GetsModelTypeFromRazorMarkup()
        {
            Type type;
            using (var reader = new StringReader(TemplateText))
            {
                type = new RazorHtmlContentTypeHandler().CreateType(reader);
            }
            Assert.NotNull(type);
            var genericArguments = type.BaseType.GetGenericArguments();
            Assert.Equal(1, genericArguments.Length);
            Assert.Equal(typeof(TestModel), genericArguments[0]);
        }

        [Fact]
        public void Renders()
        {
            const string expected = @"<!DOCTYPE html><html><body>Test Text</body></html>";
            Type type;
            using (var reader = new StringReader(TemplateText))
            {
                type = new RazorHtmlContentTypeHandler().CreateType(reader, typeof (TestModel));
            }

            var instance = (SimpleTemplateBase)Activator.CreateInstance(type);
            instance.SetModel(new TestModel {Text = "Test Text"});
            var writer = new StringWriter();
            instance.Writer = writer;
            instance.Execute();
            Assert.Equal(expected, writer.ToString().Trim());
        }
    }

    public class TestModel
    {
        public string Text { get; set; }
    }
}
