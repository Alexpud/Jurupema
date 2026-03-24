using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Nodes;

namespace Jurupema.Api.OpenApi;

public sealed class EnumAsStringSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        var type = Nullable.GetUnderlyingType(context.Type) ?? context.Type;
        if (!type.IsEnum)
        {
            return;
        }

        if (schema is not OpenApiSchema mutable)
        {
            return;
        }

        mutable.Type = JsonSchemaType.String;
        mutable.Format = null;
        var values = new List<JsonNode>();
        foreach (var name in Enum.GetNames(type))
        {
            values.Add(JsonValue.Create(name));
        }

        mutable.Enum = values;
    }
}
