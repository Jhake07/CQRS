using CQRS.Application;
using CQRS.Application.Features;
using CQRS.Identity;
using CQRS.Infrastructure;
using CQRS.Persistence;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
// --------------------------------------
// 1. Swagger (Dev only)
// --------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// --------------------------------------
// 2. HTTPS Redirection
// --------------------------------------
app.UseHttpsRedirection();
// --------------------------------------
// 3. CORS MUST COME BEFORE EVERYTHING ELSE
// --------------------------------------
app.UseCors("AllowFrontend");
// --------------------------------------
// 4. Authentication comes BEFORE your custom wrappers
// --------------------------------------
app.UseAuthentication();
// --------------------------------------
// 5. Custom Middlewares (Order matters!)
// --------------------------------------
app.UseMiddleware<ResponseHandlingMiddleware>();  // MUST run before exceptions
app.UseMiddleware<ExceptionHandlingMiddleware>();
// --------------------------------------
// 6. Authorization
// --------------------------------------
app.UseAuthorization();
// --------------------------------------
// 7. Controllers
// --------------------------------------
app.MapControllers();
await app.RunAsync();