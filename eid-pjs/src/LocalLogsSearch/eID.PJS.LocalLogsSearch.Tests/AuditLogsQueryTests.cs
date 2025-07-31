using eID.PJS.LocalLogsSearch.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#nullable disable
namespace eID.PJS.LocalLogsSearch.Tests;

public class AuditLogsQueryTests
{
    private JObject testObj = JObject.Parse(@"{
            ""FieldDate"": ""2023-07-10T20:25:33"",
            ""FieldString"": ""Hello GOOD world!"",
            ""FieldNum"": 20,
            ""FieldBool"": true,
            ""FieldPayload"": {
                ""p1"": 1234,
                ""p2"": ""hello"",
                ""p3"": ""2023-07-20T05:15:20""
            }
        }");




    public AuditLogsQueryTests()
    {

    }

    [Fact]
    public void NumericRange_AND_QueryTest()
    {
        var json = @"{
                      ""type"": ""logical"",
                      ""operator"": ""and"",
                      ""nodes"": [
                        {
                          ""type"": ""compare"",
                          ""operator"": ""GreaterThan"",
                          ""field"": ""FieldNum"",
                          ""value"": 10
                        },
                        {
                          ""type"": ""compare"",
                          ""operator"": ""LessThanOrEqual"",
                          ""field"": ""FieldNum"",
                          ""value"": 20
                        }
                      ]
                    }";

        var query = JsonConvert.DeserializeObject<QueryNode>(json);
        bool isMatch = query.Match(testObj);

        Assert.True(isMatch);

    }

    [Fact]
    public void NumericRangeNoMatch_AND_QueryTest()
    {
        var json = @"{
                      ""type"": ""logical"",
                      ""operator"": ""and"",
                      ""nodes"": [
                        {
                          ""type"": ""compare"",
                          ""operator"": ""GreaterThan"",
                          ""field"": ""FieldNum"",
                          ""value"": 10
                        },
                        {
                          ""type"": ""compare"",
                          ""operator"": ""LessThan"",
                          ""field"": ""FieldNum"",
                          ""value"": 20
                        }
                      ]
                    }";

        var query = JsonConvert.DeserializeObject<QueryNode>(json);

        bool isMatch = query.Match(testObj);

        Assert.False(isMatch);

    }


    [Fact]
    public void DateRange_AND_QueryTest()
    {
        var json = @"{
                      ""type"": ""logical"",
                      ""operator"": ""and"",
                      ""nodes"": [
                        {
                          ""type"": ""compare"",
                          ""operator"": ""GreaterThan"",
                          ""field"": ""FieldDate"",
                          ""value"": ""2023-07-10T10:15:30""
                        },
                        {
                          ""type"": ""compare"",
                          ""operator"": ""LessThanOrEqual"",
                          ""field"": ""FieldDate"",
                          ""value"": ""2023-07-30T03:45:10""
                        }
                      ]
                    }";

        var query = JsonConvert.DeserializeObject<QueryNode>(json);

        bool isMatch = query.Match(testObj);

        Assert.True(isMatch);

    }


    [Fact]
    public void DateRangeNoMatch_AND_QueryTest()
    {
        var json = @"{
                      ""type"": ""logical"",
                      ""operator"": ""and"",
                      ""nodes"": [
                        {
                          ""type"": ""compare"",
                          ""operator"": ""GreaterThan"",
                          ""field"": ""FieldDate"",
                          ""value"": ""2023-07-11T10:15:30""
                        },
                        {
                          ""type"": ""compare"",
                          ""operator"": ""LessThanOrEqual"",
                          ""field"": ""FieldDate"",
                          ""value"": ""2023-07-12T03:45:10""
                        }
                      ]
                    }";

        var query = JsonConvert.DeserializeObject<QueryNode>(json);

        bool isMatch = query.Match(testObj);

        Assert.False(isMatch);

    }


    [Fact]
    public void MultipleFields_AND_QueryTest()
    {
        var json = @"{
                      ""type"": ""logical"",
                      ""operator"": ""and"",
                      ""nodes"": [
                        {
                          ""type"": ""compare"",
                          ""operator"": ""GreaterThan"",
                          ""field"": ""FieldDate"",
                          ""value"": ""2023-07-01T01:20:00""
                        },
                        {
                          ""type"": ""compare"",
                          ""operator"": ""LessThan"",
                          ""field"": ""FieldDate"",
                          ""value"": ""2023-07-30T22:35:30""
                        },
                        {
                          ""type"": ""compare"",
                          ""operator"": ""Contains"",
                          ""field"": ""FieldString"",
                          ""value"": ""Good""
                        },
                        {
                          ""type"": ""compare"",
                          ""operator"": ""Equal"",
                          ""field"": ""FieldNum"",
                          ""value"": 20
                        },
                        {
                          ""type"": ""compare"",
                          ""operator"": ""Equal"",
                          ""field"": ""FieldBool"",
                          ""value"": true
                        }

                      ]
                    }";

        var query = JsonConvert.DeserializeObject<QueryNode>(json);


        bool isMatch = query.Match(testObj);

        Assert.True(isMatch);
    }


    [Fact]
    public void ComplexQuery1Test()
    {
        var json = @"{
                    ""type"": ""logical"",
                    ""operator"": ""and"",
                    ""nodes"": [
                    {
                        ""type"": ""compare"",
                        ""operator"": ""GreaterThan"",
                        ""field"": ""FieldNum"",
                        ""value"": 10
                    },
                    {
                        ""type"": ""logical"",
                        ""operator"": ""or"",
                        ""nodes"": [
                        {
                            ""type"": ""compare"",
                            ""operator"": ""Equal"",
                            ""field"": ""FieldString"",
                            ""value"": ""alabala""
                        },
                        {
                            ""type"": ""compare"",
                            ""operator"": ""Equal"",
                            ""field"": ""FieldBool"",
                            ""value"": true   
                        }
                        ]
                    }
                    ]
                }";

        var query = JsonConvert.DeserializeObject<QueryNode>(json);

        bool isMatch = query.Match(testObj);

        Assert.True(isMatch);

    }

    [Fact]
    public void ComplexQuery2Test()
    {
        var json = @"{
                    ""type"": ""logical"",
                    ""operator"": ""or"",
                    ""nodes"": [
                    {
                        ""type"": ""compare"",
                        ""operator"": ""GreaterThan"",
                        ""field"": ""FieldNum"",
                        ""value"": 10
                    },
                    {
                        ""type"": ""logical"",
                        ""operator"": ""and"",
                        ""nodes"": [
                        {
                            ""type"": ""compare"",
                            ""operator"": ""Equal"",
                            ""field"": ""FieldString"",
                            ""value"": ""alabala""
                        },
                        {
                            ""type"": ""compare"",
                            ""operator"": ""Equal"",
                            ""field"": ""FieldBool"",
                            ""value"": true   
                        }
                        ]
                    }
                    ]
                }";

        var query = JsonConvert.DeserializeObject<QueryNode>(json);

        bool isMatch = query.Match(testObj);

        Assert.True(isMatch);

    }

    [Fact]
    public void QuerySubObject1Test()
    {
        var json = @"{
                      ""type"": ""logical"",
                      ""operator"": ""and"",
                      ""nodes"": [
                        {
                          ""type"": ""compare"",
                          ""operator"": ""GreaterThan"",
                          ""field"": ""FieldPayload.p1"",
                          ""value"": 1000
                        }
                      ]
                    }";

        var query = JsonConvert.DeserializeObject<QueryNode>(json);

        bool isMatch = query.Match(testObj);

        Assert.True(isMatch);
    }

}

