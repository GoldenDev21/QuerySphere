using QuerySphere.data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using QuerySphere.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<QueryShpereDbContext>(o => o.UseSqlServer(connString));


//var keyVaultEndpoint = new Uri(builder.Configuration["VaultKey"]);
//var secretClient = new SecretClient(keyVaultEndpoint, new DefaultAzureCredential());

//KeyVaultSecret kvs = secretClient.GetSecret("querysphereapisecret");
//builder.Services.AddDbContext<QueryShpereDbContext>(o => o.UseSqlServer(kvs.Value));

var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();


app.MapPost("/adduser", async (OauthToken user, QueryShpereDbContext db) =>    
{
    db.OauthTokens.Add(user);
    await db.SaveChangesAsync();

    return Results.Created($"/adduser/{user.TokenId}", user);
});

app.MapGet("/", async (QueryShpereDbContext db) =>
{
    await db.OauthTokens.ToListAsync();
});

static void UpdateUserFields(OauthToken existingToken, OauthToken newTokenData)
{
    // Assuming that OauthToken has a property named UserRole
    // Update only UserRole. No other fields are changed.
    existingToken.UserRole = newTokenData.UserRole;
}

app.MapPut("/updateuser/{tokenId}", async (int tokenId, OauthToken updatedUser, QueryShpereDbContext db) =>
{
    var userToken = await db.OauthTokens.FindAsync(tokenId);
    if (userToken is null)
    {
        return Results.NotFound();
    }

    // Assume UpdateUserFields is a method to map the fields of updatedUser to userToken
    UpdateUserFields(userToken, updatedUser);
    await db.SaveChangesAsync();

    return Results.Ok(userToken);
});

app.MapDelete("/deleteuser/{tokenId}", async (int tokenId, QueryShpereDbContext db) =>
{
    var userToken = await db.OauthTokens.FindAsync(tokenId);
    if (userToken is null)
    {
        return Results.NotFound();
    }

    db.OauthTokens.Remove(userToken);
    await db.SaveChangesAsync();

    return Results.NoContent(); // Standard response for a successful deletion
});

app.Run();
