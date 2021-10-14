using Microsoft.Azure.Functions.Worker;
using System;
using System.Security.Claims;

public static class FunctionContextExtension {
	public static ClaimsPrincipal GetUser(this FunctionContext FunctionContext) {
		try {
			return (ClaimsPrincipal)FunctionContext.Items["User"];
		}
		catch (Exception e) {
			throw new UnauthorizedAccessException(/*e.Message*/);
		}
	}
}
