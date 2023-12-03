#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Pointauc.Api
{
	[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, GenerationMode = JsonSourceGenerationMode.Serialization)]
	[JsonSerializable(typeof(Bid))]
	[JsonSerializable(typeof(BidsRequest))]
	[JsonDerivedType(typeof(bool?), typeDiscriminator: "base")]

	public partial class SourceGenerationContext : JsonSerializerContext
	{
	}
	public static class JsonSerializationModifiers
	{
		public static void IgnoreNoSerializeAttribute(JsonTypeInfo type)
		{
			var propertiesToRemove = type.Properties
				.Select(x => x.PropertyType.GetGenericTypeDefinition())
				.ToList();

			//foreach (var prop in propertiesToRemove)
			//{
			//	prop.ShouldSerialize = (_, _) => false;
			//}
		}
	}
	internal class MyClass : JsonConverter<bool?>
	{
		public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}

		public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
		{
			if (value.HasValue)
			{
				writer.WriteBooleanValue(value.Value);
			}
		}
	}

}
#endif
