//public class AuthMiddleware
//{
//    private readonly RequestDelegate _next;

//    public AuthMiddleware(RequestDelegate next)
//    {
//        _next = next;
//    }

//    public async Task InvokeAsync(HttpContext context)
//    {
//        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

//        if (string.IsNullOrEmpty(token))
//        {
//            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//            await context.Response.WriteAsync("Authorization header is missing.");
//            return;
//        }

//        // Check if the token is valid
//        if (!IsValidToken(token))
//        {
//            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//            await context.Response.WriteAsync("Invalid token.");
//            return;
//        }

//        // If token is valid, proceed
//        await _next(context);
//    }

//    private bool IsValidToken(string token)
//    {
//        // Here you can validate the token (e.g., check it against a list or a generated token)
//        // For simplicity, we assume the token is valid if it's not empty.
//        return true;  // Replace with real validation if necessary.
//    }
//}
