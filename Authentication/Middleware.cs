namespace Authentication
{
    public class Middleware(RequestDelegate next)
    {
       public async Task InvokeAsync(HttpContext context)
        {
             var referrer = context.Request.Headers["Referrer"].FirstOrDefault();
            if(string.IsNullOrEmpty(referrer))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Hmm, Cant reach this page");
                return;
            
             }
            else
            {
                await next(context);
            }
        }

    }
}
 