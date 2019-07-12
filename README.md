# Introduction 
Provides basic exception management for an ASP.NET Core Web API.

[![Build status](https://ci.appveyor.com/api/projects/status/d2da3gk2egjjnmtw/branch/master?svg=true)](https://ci.appveyor.com/project/jusbuc2k/csg-aspnetcore-exceptionmanagement/branch/master)

 # Packages 

| Package | NuGet Stable | NuGet Pre-release | MyGet Dev |
| ------- | ------------ | ----------------- | --------- |
| Csg.AspNetCore.ExceptionManagement | n/a | n/a | [Link](https://www.myget.org/feed/csgsolutions-dev/package/nuget/Csg.AspNetCore.ExceptionManagement) |

# Getting Started

Install the Nuget Package from the list above.

### Add Services
Register the exception management related services in ```ConfigureServices()```

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // other services
    services.AddExceptionManagement().AddWebApiDefaults()
    // other services
}
```

### Add Middlware
Add the exception management middleware (exception handler) into the exception handler pipeline.
```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{            
    app.UseExceptionManagement(env.IsDevelopment());
    // other middlware
}
```
The UseExceptionManagement() method has a single parameter, allowUnsafeExceptions, whose default
value is ```false```. This parameter indicates if an ```ExceptionResult``` should be passed to
the exception handler if the ```IsSafe``` property of the result has not been set to ```true```.
In the above example code, we pass in the value of ```env.IsDevelopment()``` in order to ensure
that all error messages are marked as safe when running in a development environment.  In any other
environment, you can use Filters to ensure that only the exception data you want to return to the caller
is sent.

### Adding Filters
An exception filter is a method (usually static) matching the signature of the following delegate:
```csharp
public delegate void ExceptionFilterDelegate(ExceptionContext context);
```
For example, to build an exception filter that handles any thrown ```SqlException```, the following
filter could be used:
```csharp
public class ExceptionFilters
{
    public static void FilterSqlExceptions(Csg.AspNetCore.ExceptionManagement.ExceptionContext context)
    {
        // Handle duplicate key exception
        if (context.Error is SqlException sqlException && sqlException.Number == 2601)
        {
            // The context.Result property will be passed to the exception handler
            context.Result = new Csg.AspNetCore.ExceptionManagement.ExceptionResult()
            {
                StatusCode = 500,
                IsSafe = true, //Ensures this result can be sent to a caller in all environments
                ErrorCode = "SqlException",
                ErrorTitle = "Duplicate Value Error",
                ErrorDetail = "A value provided was the same as an existing value."
            };
        }
    }
}
```
This filter would return an HTTP 500 status code and error information to the caller. It can be registered in ```ConfigureServices```

```csharp
services.AddExceptionManagement()
    .AddWebApiDefaults()
    .AddFilter(ExceptionFilters.FilterSqlExceptions);
```

### Using a Custom Handler

Options can be configured by using a setup action with ```AddExceptionManagement()```.

```csharp
services.AddExceptionManagement(options =>
{
    options.Handler = CustomHandlerDelegate;
    options.UnsafeResult = ExceptionManagementOptions.GenericErrorResult;
});
```

Defines the handler that is responsible for turning an ```ExceptionResult``` into an API response. 
A handler must be a function that matches the ```ExceptionHandlerDelegate``` method signature as follows:
```csharp
public delegate Task ExceptionHandlerDelegate(ExceptionContext context);
```

### Using a Custom Unsafe Result

The value of the UnsafeResult option is passed to the exception handler when an ```ExceptionResult```
is not marked with ```IsSafe=true``` and the ```allowUnsafeExceptions``` parameter passed to 
```UseExceptionManagement()``` is ```false``` (the default). You can set a custom UnsafeResult when adding the services registration:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddExceptionManagement(options =>
    {
        options.UnsafeResult = new ExceptionResult()
        {
            StatusCode = 418,
            ErrorTitle = "The server is a teapot",
            ErrorDetail = "This server is short and stout."
        };
    }).AddWebApiDefaults();
}
```

### Custom Data in the Response

The default handler passes the following payloads depending on content negotiation:

Plain Text:
```
ID: <value>
Title: <value>
Detail: <value>
ErrorCode: <value>
```

JSON:
```json
{
    "ID": "<guid>",
    "Title": "<value>",
    "Detail": "<value>",
    "ErrorCode": "<value>",
    "ErrorData": { } 
}
```
