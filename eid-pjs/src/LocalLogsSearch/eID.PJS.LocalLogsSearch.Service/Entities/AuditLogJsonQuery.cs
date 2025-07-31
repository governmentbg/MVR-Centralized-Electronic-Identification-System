using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;


#nullable disable

namespace eID.PJS.LocalLogsSearch.Service;


/// <summary>
/// Represents the possible comparison operations
/// </summary>
public enum ComparisonOperator
{
    Equal,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
    Contains,
    StartsWith,
    EndsWith
}

/// <summary>
/// Represents the possible logical operations
/// </summary>
public enum LogicalOperator
{
    And,
    Or
}

/// <summary>
/// Represents a node in the query. Nodes can be nested.
/// </summary>
[JsonConverter(typeof(QueryNodeConverter))]
public abstract class QueryNode
{
    public abstract bool Match(JObject obj);
}

/// <summary>
/// A node that represents a comparison operation
/// </summary>
/// <seealso cref="eID.PJS.LocalLogsSearch.Service.QueryNode" />
public class CompareNode : QueryNode
{
    /// <summary>
    /// Gets or sets the comparison operator.
    /// </summary>
    /// <value>
    /// The operator.
    /// </value>
    [JsonProperty("operator")]
    public ComparisonOperator Operator { get; set; }

    /// <summary>
    /// Gets or sets the field of the object to filter on.
    /// Supports path to the field so you can look in the hierarchy.
    /// </summary>
    /// <value>
    /// The field.
    /// </value>
    [JsonProperty("field")]
    public string Field { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    [JsonProperty("value")]
    public object Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompareNode"/> class.
    /// </summary>
    /// <param name="operator">The operator.</param>
    /// <param name="field">The field.</param>
    /// <param name="value">The value.</param>
    public CompareNode(ComparisonOperator @operator, string field, object value)
    {
        Operator = @operator;
        Field = field;
        Value = value;
    }

    /// <summary>
    /// Matches the specified object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>True if tehre is a match, otherwise returns false</returns>
    public override bool Match(JObject obj)
    {
        var propertyValue = obj.SelectToken(Field)?.ToObject<object>();

        return Operator switch
        {
            ComparisonOperator.Equal => Equals(propertyValue, Value),
            ComparisonOperator.GreaterThan => Comparer.Default.Compare(propertyValue, Value) > 0,
            ComparisonOperator.LessThan => Comparer.Default.Compare(propertyValue, Value) < 0,
            ComparisonOperator.GreaterThanOrEqual => Comparer.Default.Compare(propertyValue, Value) >= 0,
            ComparisonOperator.LessThanOrEqual => Comparer.Default.Compare(propertyValue, Value) <= 0,
            ComparisonOperator.Contains => propertyValue is string str && str.Contains(Value.ToString(), StringComparison.OrdinalIgnoreCase),
            ComparisonOperator.StartsWith => propertyValue is string str && str.StartsWith(Value.ToString(), StringComparison.OrdinalIgnoreCase),
            ComparisonOperator.EndsWith => propertyValue is string str && str.EndsWith(Value.ToString(), StringComparison.OrdinalIgnoreCase),
            _ => throw new SerializationException($"Unsupported comparison operator: {Operator}")
        };
    }
}

/// <summary>
/// A node that represents a logical operation such as AND or OR
/// </summary>
/// <seealso cref="eID.PJS.LocalLogsSearch.Service.QueryNode" />
public class LogicalNode : QueryNode
{
    /// <summary>
    /// Gets or sets the operator.
    /// </summary>
    /// <value>
    /// The operator.
    /// </value>
    [JsonProperty("operator")]
    public LogicalOperator Operator { get; set; }

    /// <summary>
    /// Gets or sets the nodes of the query.
    /// </summary>
    /// <value>
    /// The nodes.
    /// </value>
    [JsonProperty("nodes")]
    public List<QueryNode> Nodes { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogicalNode"/> class.
    /// </summary>
    /// <param name="operator">The operator.</param>
    /// <param name="nodes">The nodes.</param>
    public LogicalNode(LogicalOperator @operator, List<QueryNode> nodes)
    {
        Operator = @operator;
        Nodes = nodes;
    }

    /// <summary>
    /// Executes the matching
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>Returns true if there is a match, otherwise returns false</returns>
    public override bool Match(JObject obj)
    {
        return Operator switch
        {
            LogicalOperator.And => Nodes.All(node => node.Match(obj)),
            LogicalOperator.Or => Nodes.Any(node => node.Match(obj)),
            _ => throw new SerializationException($"Unsupported logical operator: {Operator}")
        };
    }
}

/// <summary>
/// Json converter to serialize and deserialize the query nodes
/// </summary>
/// <seealso cref="Newtonsoft.Json.JsonConverter" />
public class QueryNodeConverter : JsonConverter
{
    /// <summary>
    /// Determines whether this instance can convert the specified object type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>
    /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
    /// </returns>
    public override bool CanConvert(Type objectType)
    {
        return typeof(QueryNode).IsAssignableFrom(objectType);
    }

    /// <summary>
    /// Reads the JSON representation of the object.
    /// </summary>
    /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing value of object being read.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>
    /// The object value.
    /// </returns>
    /// <exception cref="System.Text.Json.JsonException">
    /// Invalid type: {typeName}
    /// or
    /// Missing 'type' property in JSON
    /// </exception>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {

        JObject jsonObject = JObject.Load(reader);

        if (!jsonObject.HasValues)
            return null;

        if (jsonObject.TryGetValue("type", StringComparison.OrdinalIgnoreCase, out var typeToken))
        {
            string typeName = typeToken.Value<string>();

            switch (typeName)
            {
                case "compare":
                    var compareNode = new CompareNode(ComparisonOperator.Equal, "", null);
                    serializer.Populate(jsonObject.CreateReader(), compareNode);

                    if (string.IsNullOrWhiteSpace(compareNode.Field))
                        throw new SerializationException("Field is required for the compare node.");

                    if (!Enum.IsDefined(typeof(ComparisonOperator), compareNode.Operator))
                    {
                        throw new ArgumentOutOfRangeException(nameof(compareNode.Operator), $"Unknown operator '{compareNode.Operator}' in compare node");
                    }

                    return compareNode;
                case "logical":
                    var logicalNode = new LogicalNode(LogicalOperator.And, new List<QueryNode>());
                    serializer.Populate(jsonObject.CreateReader(), logicalNode);

                    if (!Enum.IsDefined(typeof(LogicalOperator), logicalNode.Operator))
                    {
                        throw new ArgumentOutOfRangeException(nameof(logicalNode.Operator), $"Unknown operator '{logicalNode.Operator}' in logical node");
                    }

                    return logicalNode;
                default:
                    throw new SerializationException($"Invalid node type: {typeName}");
            }
        }

        throw new SerializationException("Missing 'type' property in JSON");

    }

    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is CompareNode compareNode)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("compare");
            writer.WritePropertyName("operator");
            writer.WriteValue(compareNode.Operator.ToString());
            writer.WritePropertyName("field");
            writer.WriteValue(compareNode.Field);
            writer.WritePropertyName("value");
            serializer.Serialize(writer, compareNode.Value);
            writer.WriteEndObject();
        }
        else if (value is LogicalNode logicalNode)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("logical");
            writer.WritePropertyName("operator");
            writer.WriteValue(logicalNode.Operator.ToString());
            writer.WritePropertyName("nodes");
            serializer.Serialize(writer, logicalNode.Nodes);
            writer.WriteEndObject();
        }
    }
}




