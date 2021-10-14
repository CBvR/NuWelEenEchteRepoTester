using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Models {
	//[OpenApiProperty(Description = "This represents the model entity for pet of Swagger Pet Store.")]

	[OpenApiExample(typeof(DummyTaskExample))]
	public class Task {
		[OpenApiProperty(Description = "Gets or sets the task ID.")]
		[JsonRequired]
		public int TaskId { get; set; }

		[OpenApiProperty(Description = "Gets or sets the start date of the task.")]
		public DateTime StartDate { get; set; }

		[OpenApiProperty(Description = "Gets or sets the end date of the task")]
		public DateTime EndDate { get; set; }

		[OpenApiProperty(Description = "Gets or sets the createdOn date of the task.")]
		public DateTime CreatedOn { get; set; }

		[OpenApiProperty(Description = "Gets or sets the EditedOn date of the task.")]
		public DateTime EditedOn { get; set; }

		[OpenApiProperty(Description = "Gets or sets the description for a task")]
		public string Description { get; set; }

		[OpenApiProperty(Description = "Gets or sets the description for a task")]
		public bool Completed { get; set; }

		[OpenApiProperty(Description = "Gets or sets the completed date of the task.")]
		public DateTime CompletedWhen { get; set; }

		[OpenApiProperty(Description = "Gets or sets the goal the task is linked to")]
		public Goal linkedGoal { get; set; }

		[OpenApiProperty(Description = "Gets or sets the team which is responsible for the task")]
		public Team linkedTeam { get; set; }

	}

	public class DummyTaskExample : OpenApiExample<Task> {
		public override IOpenApiExample<Task> Build(NamingStrategy NamingStrategy = null) {
			Examples.Add(OpenApiExampleResolver.Resolve("task1", new Task() { TaskId = 401, Description = "task1", Completed = false }, NamingStrategy));
			Examples.Add(OpenApiExampleResolver.Resolve("task2", new Task() { TaskId = 402, Description = "task2", Completed = false }, NamingStrategy));

			return this;
		}
	}

	public class DummyTaskExamples : OpenApiExample<List<Task>> {
		public override IOpenApiExample<List<Task>> Build(NamingStrategy NamingStrategy = null) {
			Examples.Add(OpenApiExampleResolver.Resolve("Tasks", new List<Task> {
				new Task() { TaskId = 401, Description = "task1", Completed = false },
				new Task() { TaskId = 402, Description = "task2", Completed = false},
			}));

			return this;
		}
	}
}
