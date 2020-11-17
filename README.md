# Vue CLI Server Middleware

[![](https://buildstats.info/nuget/Proggmatic.SpaServices.VueCli)](https://www.nuget.org/packages/Proggmatic.SpaServices.VueCli/)

Provides dev-time support for [Vue CLI](https://cli.vuejs.org/) in ASP.NET Core's SPA scenarios. 

Only .NET 5.0 and higher will be supported.

This is mostly copied and modified from ASP.net Core's implementation for React:
[https://github.com/dotnet/aspnetcore/blob/master/src/Middleware/SpaServices.Extensions/src/ReactDevelopmentServer](https://github.com/dotnet/aspnetcore/blob/master/src/Middleware/SpaServices.Extensions/src/ReactDevelopmentServer).

# Usage

## ASP.NET Project

Install the `Proggmatic.SpaServices.VueCli` NuGet package on the
ASP.NET Core web project, then modify the `Startup.cs` file similar to the following.


```cs
using Proggmatic.SpaServices.VueCli;                  //-- new addition --//


public void ConfigureServices(IServiceCollection services)
{
  // ... other ASP.NET configuration skipped


  //-- new addition --//
  services.AddSpaStaticFiles(configuration =>
  {
    configuration.RootPath = "ClientApp/dist";
  });
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
  // ... other aspnet configuration skipped here

  app.UseStaticFiles();

  app.UseSpaStaticFiles();                            //-- new addition --//
  
  // ... more default stuff

  //-- new addition --//
  app.UseSpa(spa =>
  {
    // spa.Options.SourcePath = "ClientApp";          // Optional. If this string is commented, ClientApp will be used
    // spa.Options.PackageManagerCommand = "npm";     // Optional. If this string is commented, npm will be used. You may use yarn instead of npm.

    if (env.IsDevelopment())
    {
      spa.UseVueCliServer();
    }
  });
}
```


## Vue Project

The vue project is a typical one created by vue cli such as `vue create ClientApp` and
placed inside the ASPNET site's project folder.



## .csproj Configuration

If publishing the ASPNET Core's project is needed then edit the .csproj file like below.
Change `SpaRoot` value to the actual vue project's folder name. Change yarn to npm if necessary.

```xml
  <PropertyGroup>
    <SpaRoot>ClientApp\</SpaRoot>
    <DefaultItemExcludes>$(DefaultItemExcludes);$(SpaRoot)node_modules\**</DefaultItemExcludes>
  </PropertyGroup>

  <ItemGroup>
    <!-- Don't publish the SPA source files, but do show them in the project files list -->
    <Content Remove="$(SpaRoot)**" />
    <None Remove="$(SpaRoot)**" />
    <None Include="$(SpaRoot)**" Exclude="$(SpaRoot)node_modules\**" />
  </ItemGroup>

  <Target Name="DebugEnsureNodeEnv" BeforeTargets="Build" Condition=" '$(Configuration)' == 'Debug' And !Exists('$(SpaRoot)node_modules') ">

    <!-- Ensure Node.js is installed -->
    <Exec Command="node --version" ContinueOnError="true">
      <Output TaskParameter="ExitCode" PropertyName="ErrorCode" />
    </Exec>
    <Error Condition="'$(ErrorCode)' != '0'" Text="Node.js is required to build and run this project. To continue, please install Node.js from https://nodejs.org/, and then restart your command prompt or IDE." />

    <!-- Or ensure Yarn is installed -->
    <Exec Command="yarn --version" ContinueOnError="true">
      <Output TaskParameter="ExitCode" PropertyName="ErrorCode" />
    </Exec>
    <Error Condition="'$(ErrorCode)' != '0'" Text="Yarn is required to build and run this project." />
    <Message Importance="high" Text="Restoring dependencies using 'yarn'. This may take several minutes..." />
    <Exec WorkingDirectory="$(SpaRoot)" Command="yarn install" />
  </Target>

  <Target Name="PublishRunWebpack" AfterTargets="ComputeFilesToPublish">
    <!-- As part of publishing, ensure the JS resources are freshly built in production mode -->
    <Exec WorkingDirectory="$(SpaRoot)" Command="yarn install" />
    <Exec WorkingDirectory="$(SpaRoot)" Command="yarn build" />

    <!-- Include the newly-built files in the publish output -->
    <ItemGroup>
      <DistFiles Include="$(SpaRoot)dist\**" />
      <ResolvedFileToPublish Include="@(DistFiles->'%(FullPath)')" Exclude="@(ResolvedFileToPublish)">
        <RelativePath>%(DistFiles.Identity)</RelativePath>
        <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
        <ExcludeFromSingleFile>true</ExcludeFromSingleFile>
      </ResolvedFileToPublish>
    </ItemGroup>
  </Target>
```

# Notes

* To get hot-module-reloading to work, both Vue's dev server and aspnet's 
site need to be on the same protocol (http or https).
* When using https in dev server, it needs to use a trusted certificate or
aspnet will refuse to connect to it.
* Progress of `serve` command writes to logger with name Microsoft.AspNetCore.SpaServices.
* Since dev server's progress is written to stderror there will be lots of "fail"s logged in dotnet. 
To minimize this add `progress: false` to the `devServer` section in `vue.config.js` file. 
See [this page](https://cli.vuejs.org/config/#devserver) on how to add it.