# Introduction 
Provides basic exception management for an ASP.NET Core Web API.

[![Build status](https://ci.appveyor.com/api/projects/status/d2da3gk2egjjnmtw/branch/master?svg=true)](https://ci.appveyor.com/project/jusbuc2k/csg-aspnetcore-exceptionmanagement/branch/master)
# Getting Started

Install the Nuget package.

## Packages 

| Package | NuGet Stable | NuGet Pre-release | MyGet Dev |
| ------- | ------------ | ----------------- | --------- |
| Csg.AspNetCore.ExceptionManagement | n/a | n/a | [Link](https://www.myget.org/feed/csgsolutions-dev/package/nuget/Csg.AspNetCore.ExceptionManagement) |

## Examples
```
dotnet add package Csg.AspNetCore.ExceptionManagement
```
```xml
<PackageReference Include="Csg.AspNetCore.ExceptionManagement" Version="<version>" />
```

## Add Services
Register the exception management related services in ```ConfigureServices()```

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // other services
    services.AddExceptionManagement().AddWebApiDefaults()
    // other services
}
```

## Add Middlware
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

## Adding Filters
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

The context.Result property must be set to an instance of ExceptionResult, which has the following properties:

| Property  | Data Type | Default Value  | Description  | 
|---|---|---|---|---|
| StatusCode | Int32 | 500  | The HTTP Status Code that should be sent in the response. |
| IsSafe | String | false  | Indicates if the error information provided is safe to display to a caller in any environment. |
| ErrorCode | String | null | An opaque value used to convey information to a caller. | 
| ErrorTitle | String | null | An opaque value used to convey information to a caller. |
| ErrorDetail | String | null | An opaque value used to convey information to a caller. |
| ErrorData | Object | null | An opaque value used to convey information to a caller. |

The ErrorData property can be used to return complex objects to the caller with additional
error information.

## Using a Custom Handler

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

## Using a Custom Unsafe Result
When using ```AddWebApiDefaults()```, the value of the UnsafeResult configuration option is passed to the exception handler when:
 *  ```ExceptionContext.Result.IsSafe == false```
 *  and ```allowUnsafeExceptions``` passed to ```UseExceptionManagement()``` is ```false``` (the default).

You can use a custom UnsafeResult instead by setting the configuration option.

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

## Default Handler Behavior

When using ```AddWebApiDefaults()```, the configured handler is ```Handlers.WebApiExceptionHandler```. 
This handler produces the following output when the request Accept header is for text/plain:
```
ID: <value>
Title: <value>
Detail: <value>
ErrorCode: <value>
```
and this output when the request Accept header is missing or for application/json:
```json
{
    "ID": "<guid>",
    "Title": "<value of Result.ErrorTitle>",
    "Detail": "<value of Result.ErrorDetail>",
    "Code": "<value of Result.ErrorCode>",
    "Data": { Result.ErrorData }
}
```

All the above values are from their corresponding properties on ExceptionResult, with the exception of ID,
which is from the ErrorID on the ExceptionContext object passed into each filter. By default, this contains
a new UUID (generated with ```System.Guid.NewGuid()```). The ID property can be assigned by any registered filter.
