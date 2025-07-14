using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddCustomLogging();

builder.Services.AddRouting(static options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.AddControllers();
builder.Services.AddAutoMapper(config =>
{
    config.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
}, AppDomain.CurrentDomain.GetAssemblies());

builder.Services
    .AddApplicationServices()
    .AddRepositories()
    .AddDatabase(builder.Configuration)
    .AddSwaggerDocumentation()
    .AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CriarTarefaDtoValidator>();


builder.Services.AddAuthorization();

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
