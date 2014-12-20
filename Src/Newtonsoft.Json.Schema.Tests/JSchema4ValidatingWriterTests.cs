﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema.V4;
using NUnit.Framework;

namespace Newtonsoft.Json.Schema.Tests
{
    [TestFixture]
    public class JSchema4ValidatingWriterTests : TestFixtureBase
    {
        [Test]
        public void ArrayBasicValidation_Pass()
        {
            JSchema4 schema = new JSchema4();
            schema.Type = JSchemaType.Array;
            schema.Items.Add(new JSchema4
            {
                Type = JSchemaType.Integer
            });

            SchemaValidationEventArgs a = null;

            StringWriter sw = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(sw);
            JSchema4ValidatingWriter validatingWriter = new JSchema4ValidatingWriter(writer);
            validatingWriter.Schema = schema;
            validatingWriter.ValidationEventHandler += (sender, args) =>
            {
                a = args;
            };

            validatingWriter.WriteStartArray();
            validatingWriter.WriteValue(10);
            validatingWriter.WriteValue(10);
            validatingWriter.WriteEndArray();

            Assert.IsNull(a);
        }

        [Test]
        public void ArrayBasicValidation_Fail()
        {
            JSchema4 schema = new JSchema4();
            schema.Type = JSchemaType.Array;
            schema.Items.Add(new JSchema4
            {
                Type = JSchemaType.Integer
            });

            SchemaValidationEventArgs a = null;

            StringWriter sw = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(sw);
            JSchema4ValidatingWriter validatingWriter = new JSchema4ValidatingWriter(writer);
            validatingWriter.Schema = schema;
            validatingWriter.ValidationEventHandler += (sender, args) =>
            {
                a = args;
            };

            validatingWriter.WriteStartArray();

            validatingWriter.WriteValue("string");
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got String.", a.Message);
            a = null;

            validatingWriter.WriteValue(true);
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got Boolean.", a.Message);
            a = null;

            validatingWriter.WriteEndArray();
            Assert.IsNull(a);
        }

        [Test]
        public void ObjectBasicValidation_Pass()
        {
            JSchema4 schema = new JSchema4();
            schema.Type = JSchemaType.Object;
            schema.Properties.Add("prop1", new JSchema4
            {
                Type = JSchemaType.Integer
            });
            schema.Properties.Add("prop2", new JSchema4
            {
                Type = JSchemaType.Boolean
            });

            SchemaValidationEventArgs a = null;

            StringWriter sw = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(sw);
            JSchema4ValidatingWriter validatingWriter = new JSchema4ValidatingWriter(writer);
            validatingWriter.Schema = schema;
            validatingWriter.ValidationEventHandler += (sender, args) =>
            {
                a = args;
            };

            validatingWriter.WriteStartObject();
            validatingWriter.WritePropertyName("prop1");
            validatingWriter.WriteValue(43);
            validatingWriter.WritePropertyName("prop2");
            validatingWriter.WriteValue(true);
            validatingWriter.WriteEndObject();

            Assert.IsNull(a);
        }

        [Test]
        public void ObjectBasicValidation_Fail()
        {
            JSchema4 schema = new JSchema4();
            schema.Type = JSchemaType.Object;
            schema.Properties.Add("prop1", new JSchema4
            {
                Type = JSchemaType.Integer
            });
            schema.Properties.Add("prop2", new JSchema4
            {
                Type = JSchemaType.Boolean
            });

            SchemaValidationEventArgs a = null;

            StringWriter sw = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(sw);
            JSchema4ValidatingWriter validatingWriter = new JSchema4ValidatingWriter(writer);
            validatingWriter.Schema = schema;
            validatingWriter.ValidationEventHandler += (sender, args) =>
            {
                a = args;
            };

            validatingWriter.WriteStartObject();

            validatingWriter.WritePropertyName("prop1");
            validatingWriter.WriteValue(true);
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got Boolean.", a.Message);
            a = null;

            validatingWriter.WritePropertyName("prop2");
            validatingWriter.WriteValue(45);
            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Boolean but got Integer.", a.Message);
            a = null;

            validatingWriter.WritePropertyName("prop3");
            validatingWriter.WriteValue(45);

            validatingWriter.WriteEndObject();

            Assert.IsNull(a);
        }

        [Test]
        public void NestedScopes()
        {
            JSchema4 schema = JSchema4.Parse(@"{
                ""properties"": {
                    ""foo"": {""type"": ""integer""},
                    ""bar"": {""type"": ""string""}
                }
            }");

            SchemaValidationEventArgs a = null;

            StringWriter sw = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(sw);
            JSchema4ValidatingWriter validatingWriter = new JSchema4ValidatingWriter(writer);
            validatingWriter.Schema = schema;
            validatingWriter.ValidationEventHandler += (sender, args) =>
            {
                a = args;
            };

            validatingWriter.WriteStartObject();
            validatingWriter.WritePropertyName("foo");
            validatingWriter.WriteStartArray();

            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected Integer but got Array.", a.Message);
            a = null;

            validatingWriter.WriteEndArray();
            validatingWriter.WritePropertyName("bar");
            validatingWriter.WriteStartObject();

            Assert.IsNotNull(a);
            StringAssert.AreEqual("Invalid type. Expected String but got Object.", a.Message);
            a = null;

            validatingWriter.WriteEndObject();
            validatingWriter.WriteEndObject();

            Assert.IsNull(a);
        }
    }
}