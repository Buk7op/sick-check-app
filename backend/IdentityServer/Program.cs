

using IdentityServer.Models;
using IdentityServer.Models.Settings;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllers();
services.AddRazorPages();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddControllersWithViews();
services.AddCors(option =>
{
    option.AddPolicy("MyPolicy", builder =>
    {
        builder.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin();
    });
});

var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbConfig)).Get<MongoDbConfig>();
services.AddIdentity<ApplicationUser, ApplicationRole>()
        .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
        (
            mongoDbSettings.ConnectionString, mongoDbSettings.Name
);

var identityServerSettings = builder.Configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>();


services.AddIdentityServer(options =>
{
    options.IssuerUri = "https://identityserver:8081";
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseErrorEvents = true;
    options.UserInteraction.LoginUrl = "/Identity/Login";
})
    .AddAspNetIdentity<ApplicationUser>()
    .AddInMemoryApiScopes(identityServerSettings.ApiScopes)
    .AddInMemoryApiResources(identityServerSettings.ApiResources)
    .AddInMemoryClients(identityServerSettings.Clients)
    .AddInMemoryIdentityResources(identityServerSettings.IdentityResources)
    .AddDeveloperSigningCredential();



var app = builder.Build();





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("MyPolicy");
app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();   

app.Run();
