using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Models
{
	[OpenApiExample(typeof(DummyUserExample))]
	public class User
	{
		[OpenApiProperty(Description = "Gets or sets the user ID.")]
		[JsonRequired]
		public int UserId { get; set; }

		[OpenApiProperty(Description = "Gets or sets the user type.")]
		[JsonRequired]
		public UserType UserType { get; set; }

		[OpenApiProperty(Description = "Gets or sets the name.")]
		[JsonRequired]
		public string Name { get; set; }

		[OpenApiProperty(Description = "Gets or sets the user's username to log in")]
		[JsonRequired]
		public string Username { get; set; }

		[OpenApiProperty(Description = "Gets or sets the user's password (will be hashed)")]
		[JsonRequired]
		public string Password { get; set; }

		[OpenApiProperty(Description = "Gets or sets the createdOn date of the task.")]
		public DateTime CreatedOn { get; set; }

		[OpenApiProperty(Description = "Gets or sets the EditedOn date of the task.")]
		public DateTime EditedOn { get; set; }

		[OpenApiProperty(Description = "Gets or sets the LastLogin date of the task.")]
		public DateTime LastLogin { get; set; }
	}

	public class DummyUserExample : OpenApiExample<User>
	{
		public override IOpenApiExample<User> Build(NamingStrategy NamingStrategy = null)
		{
			Examples.Add(OpenApiExampleResolver.Resolve("Kjell Pepping", new User() { UserId = 101, Name = "Kjell Pepping", UserType = UserType.Directie, Username = "KjellPepping", Password = "Test101" }, NamingStrategy));
			Examples.Add(OpenApiExampleResolver.Resolve("Chris van Roode", new User() { UserId = 102, Name = "Chris van Roode", UserType = UserType.Teamleider, Username = "ChrisvanRoode", Password = "Test102" }, NamingStrategy));

			return this;
		}
	}

	public class DummyUserExamples : OpenApiExample<List<User>>
	{
		public override IOpenApiExample<List<User>> Build(NamingStrategy NamingStrategy = null)
		{
			Examples.Add(OpenApiExampleResolver.Resolve("Users", new List<User> {
				new User() { UserId = 101, Name = "Kjell Pepping", UserType = UserType.Directie, Username = "KjellPepping", Password = "Test101" },
				new User() { UserId = 102, Name = "Chris van Roode",  UserType = UserType.Gebruiker, Username = "ChrisvanRoode", Password = "Test102"  },
			}));

			return this;
		}
	}
}
