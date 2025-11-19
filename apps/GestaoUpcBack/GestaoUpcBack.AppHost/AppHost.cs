var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.GestaoUpc_Api>("gestaoupc-api");

builder.Build().Run();
