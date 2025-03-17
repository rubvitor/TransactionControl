using Application.Services;
using Domain.Interfaces;
using Infrastructure.Repositories;
using MongoDB.Driver;

string _queueUrl = "https://sqs.us-east-1.amazonaws.com/YOUR_ACCOUNT_ID/YOUR_QUEUE_NAME";
string mongoConnectionString = "";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

var mongoUrlBuilder = new MongoUrlBuilder(mongoConnectionString);
var client = new MongoClient(mongoUrlBuilder.ToMongoUrl());
var database = client.GetDatabase("MyDB");

// Register existing object instance
builder.Services.AddSingleton(database);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

SQSManager.StartAsync(_queueUrl, app.Services.GetRequiredService<TransactionService>());

app.Run();
