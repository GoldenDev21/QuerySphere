using QuerySphere.data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//var connString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<QueryShpereDbContext>(o => o.UseSqlServer(connString));

var keyVaultEndpoint = new Uri(builder.Configuration["VaultKey"]);
var secretClient = new SecretClient(keyVaultEndpoint, new DefaultAzureCredential());

KeyVaultSecret kvs = secretClient.GetSecret("querysphereapikyvalut");
builder.Services.AddDbContext<QueryShpereDbContext>(o => o.UseSqlServer(kvs.Value));


var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();

// app.MapGet("api/autos", async ([FromServices] QueryShpereDbContext db) => {
//     return await db.;
// });

app.Run();
