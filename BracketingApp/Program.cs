using BracketingApp.Models;
using BracketingApp.Services; 
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IndividualsService>(); //Di for IndividualsService
builder.Services.AddSingleton<PairsService>();       // Di for PairsService
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bracketing App", Version = "development" });
});

// Configure CORS to allow all origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bracketing App"));
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins"); // CORS policy very allow-all. Only for now for dev... 

app.UseAuthorization();

app.MapControllers();

app.Run();
