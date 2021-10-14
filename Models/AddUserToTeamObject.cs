using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Models
{
    [OpenApiExample(typeof(DummyAddUserToTeamObjectExample))]
    public class AddUserToTeamObject
    {
        [OpenApiProperty(Description = "The team id that is used to add the user to this team")]
        [JsonRequired]
        public int TeamId { get; set; }

        [OpenApiProperty(Description = "The user id which is add to the given team id")]
        [JsonRequired]
        public int UserId { get; set; }
    }

    public class DummyAddUserToTeamObjectExample : OpenApiExample<AddUserToTeamObject>
    {
        public override IOpenApiExample<AddUserToTeamObject> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Add 101 to 201", new AddUserToTeamObject() { TeamId = 201, UserId = 101 }, NamingStrategy));
            Examples.Add(OpenApiExampleResolver.Resolve("Add 102 to 202", new AddUserToTeamObject() { TeamId = 202, UserId = 102 }, NamingStrategy));
            return this;
        }
    }

    public class DummyAddUserToTeamObjectExamples : OpenApiExample<List<AddUserToTeamObject>>
    {
        public override IOpenApiExample<List<AddUserToTeamObject>> Build(NamingStrategy NamingStrategy = null) {
            Examples.Add(OpenApiExampleResolver.Resolve("Add users to teams", new List<AddUserToTeamObject> {
                new AddUserToTeamObject() { TeamId = 201, UserId = 101 },
                new AddUserToTeamObject() { TeamId = 202, UserId = 102 },
            }));

            return this;
        }
    }
}
