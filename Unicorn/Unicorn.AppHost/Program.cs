var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.Unicorn_ApiService>("apiservice");

builder.AddProject<Projects.Unicorn_Web>("webfrontend")
	.WithReference(cache)
	.WithReference(apiService);

builder.Build().Run();