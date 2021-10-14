using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kennemerland.Models {
	//[OpenApiProperty(Description = "This represents the model entity for pet of Swagger Pet Store.")]

	[OpenApiExample(typeof(DummyGoalExample))]
	public class Goal {
		[OpenApiProperty(Description = "Gets or sets the pet ID.")]
		public int GoalId { get; set; }

		[OpenApiProperty(Description = "Gets or sets the description of the goal.")]
		[JsonRequired]
		public string Description { get; set; }

		[OpenApiProperty(Description = "Gets or sets the createdOn date of the goal.")]
		public DateTime CreatedOn { get; set; }

		[OpenApiProperty(Description = "Gets or sets the EditedOn date of the goal.")]
		public DateTime EditedOn { get; set; }

		[OpenApiProperty(Description = "Gets or sets the start date of the goal.")]
		public DateTime StartDate { get; set; }

		[OpenApiProperty(Description = "Gets or sets the end date of the goal")]
		public DateTime EndDate { get; set; }

		[OpenApiProperty(Description = "Gets or sets the owner / creator of this goal")]
		public int CreatorId { get; set; }

		[OpenApiProperty(Description = "Gets or sets the teamId's that have this goal as theirs")]
		[JsonRequired]
		public List<int> RelevantTeams { get; set; }
	}

	public class DummyGoalExample : OpenApiExample<Goal> {
		public override IOpenApiExample<Goal> Build(NamingStrategy NamingStrategy = null) {
			Examples.Add(OpenApiExampleResolver.Resolve("Kostenbesparing", new Goal() { GoalId = 601, Description = "Kostenbesparing", CreatorId = 101, RelevantTeams = new List<int> { 201, 202 } }, NamingStrategy));
			Examples.Add(OpenApiExampleResolver.Resolve("Ziekteverzuim vermindering", new Goal() { GoalId = 602, Description = "Ziekteverzuim vermindering", CreatorId = 101, RelevantTeams = new List<int> { 201 } }, NamingStrategy));
			return this;
		}
	}

	public class DummyGoalExamples : OpenApiExample<List<Goal>> {
		public override IOpenApiExample<List<Goal>> Build(NamingStrategy NamingStrategy = null) {
			Examples.Add(OpenApiExampleResolver.Resolve("Goals", new List<Goal> {
				new Goal() { GoalId = 601, Description = "Kostenbesparing", CreatorId = 101, RelevantTeams = new List<int> { 201, 202 }  },
				new Goal() { GoalId = 602, Description = "Ziekteverzuim vermindering", CreatorId = 101, RelevantTeams = new List<int> { 202, 204 }  },
			}));

			return this;
		}
	}
}
