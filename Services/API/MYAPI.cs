namespace EShopperAngular.Services.API.MYAPI
{
    public class AddHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public AddHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.OnStarting(state =>
            {
                if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                }

                return Task.CompletedTask;
            }, context);
            await _next(context);
        }
    }
}
