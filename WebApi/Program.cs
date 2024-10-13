using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Services;
using Services.Contracts;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
    config.CacheProfiles.Add("5mins", new CacheProfile() { Duration = 300 });
})
    .AddXmlDataContractSerializerFormatters()
    .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly)
    //.AddNewtonsoftJson()
    .AddCustomCsvFormatter();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Service Extensions
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureAutoMapperService();
builder.Services.ConfigureActionFilters();
builder.Services.ConfigureCors();
builder.Services.ConfigureDataShaper();
builder.Services.AddCustomMediaTypes();
builder.Services.AddScoped<IBookLinks, BookLinks>();
builder.Services.ConfigureVersioning();
builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureHttpCacheHeaders();
builder.Services.AddMemoryCache();
builder.Services.ConfiguraRateLimitOptions();
builder.Services.AddHttpContextAccessor();
#endregion

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerService>();
app.ConfigureExceptionHandler(logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseIpRateLimiting();

app.UseCors("CorsPolicy");
//microsoft recommending using the cache mechanism after calling UseCors
app.UseResponseCaching();
app.UseHttpCacheHeaders();

app.UseAuthorization();

app.MapControllers();

app.Run();
