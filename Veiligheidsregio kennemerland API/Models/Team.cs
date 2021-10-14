using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kennemerland.Models {
	//[OpenApiProperty(Description = "This represents the model entity for pet of Swagger Pet Store.")]

	[OpenApiExample(typeof(DummyTeamExample))]
	public class Team {
        [OpenApiProperty(Description = "Gets or sets the team ID.")]
		[JsonRequired]
		public int TeamId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the name.")]
		[JsonRequired]
		public string Name { get; set; }

		[OpenApiProperty(Description = "Gets or sets the hierarchy level of the team, the higher the level is, the more steps the team has to the top.")]
		public int HierarchyLevel{ get; set; }

		[OpenApiProperty(Description = "Gets or sets the team object of the team which is hierarchied above this team, if no team is above this one, this means this is the top team (GGD, BRANDWEER).")]
		public Team TeamAbove { get; set; }

		[OpenApiProperty(Description = "Gets or sets the user object who is responsible for the team's goals.")]
		public User TeamBoss { get; set; }

		[OpenApiProperty(Description = "Gets or sets the user object who is responsible for the team's goals.")]
		public List<User> TeamMembers { get; set; }

		[OpenApiProperty(Description = "Gets or sets the createdOn date of the team.")]
		public DateTime CreatedOn { get; set; }

		[OpenApiProperty(Description = "Gets or sets the EditedOn date of the team.")]
		public DateTime EditedOn { get; set; }
	}

	public class DummyTeamExample : OpenApiExample<Team> {
		public override IOpenApiExample<Team> Build(NamingStrategy NamingStrategy = null) {
			Examples.Add(OpenApiExampleResolver.Resolve("Incidentbestrijding", new Team() { TeamId = 201, Name = "Incidentbestrijding" }, NamingStrategy));
			Examples.Add(OpenApiExampleResolver.Resolve("Jeugdgezondheidszorg", new Team() { TeamId = 202, Name = "Jeugdgezondheidszorg" }, NamingStrategy));

			return this;
		}
	}

	public class DummyTeamExamples : OpenApiExample<List<Team>> {
		public override IOpenApiExample<List<Team>> Build(NamingStrategy NamingStrategy = null) {
			Examples.Add(OpenApiExampleResolver.Resolve("Teams", new List<Team> {
				new Team() { TeamId = 201, Name = "Incidentbestrijding" },
				new Team() { TeamId = 202, Name = "Jeugdgezondheidszorg" },
			}));

			return this;
		}
	}
}
