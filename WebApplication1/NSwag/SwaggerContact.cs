using NSwag;

namespace WebApplication1.NSwag
{
    internal class SwaggerContact : OpenApiContact
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
    }
}
