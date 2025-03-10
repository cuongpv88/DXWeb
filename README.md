# DXWeb
Repository for several DevExpress releated NuGet packages:
These packages follow the minimum requirements as described in DevExpress documentation.

**Minimum framework versions: .NET Framework 4.6.2, .NET 8.0 and .NET 9.0**

#### DX.Utils
This package contains several helper classes for working with:
* Attributes
* Bit operations
* Collections
* Conversion helpers
* Logging helpers
* Url manipulations

#### DX.Data
* DataStore<TKey, TModel> base class to work with DataSets ->
  Abstract DTO Mapping, FluentValidator validation and ApiDataStore for BlazorWASM with CRUD support.

#### DX.Data.Xpo
This package contains the XpoDatabase and XpoDataStore for easy config and use of DTO pattern **no mapping implementation**

#### DX.Data.Xpo.AutoMapper
This package contains the XpoDatabase and XpoDataStore for easy config and use of DTO pattern by using AutoMapper

#### DX.Data.Xpo.AutoMapper
This package contains the XpoDatabase and XpoDataStore for easy config and use of DTO pattern by using Mapster

#### DX.Data.Xpo.Identity
This package contains an XPO based *abstract* storage mechanism for use with MS Identity to support a small dozen different DB engines.

#### DX.Data.Xpo.Identity.AutoMapper
This package contains an XPO based storage mechanism for use with MS Identity to support a small dozen different DB engines with AutMapper DTO handling.

#### DX.Data.Xpo.Identity.Mapster
This package contains an XPO based storage mechanism for use with MS Identity to support a small dozen different DB engines with Mapster DTO handling.

#### Note
From v23.2.3.31, you will need to include either **DX.Data.Xpo.Identity.AutoMapper**  or **DX.Data.Xpo.Identity.Mapster** (**NOT BOTH**), depending on the DTO Mapper you're already (or planning) using! 


Quick config on .NET:
```cs
public void ConfigureServices(IServiceCollection services)
        {
            string connStrName = "DefaultConnection";
            //Initialize XPODataLayer / Database	
            services.AddXpoDatabase((o) => {
                o.Name = connStrName;
                o.ConnectionString = Configuration.GetConnectionString(connStrName);
            });

            //Initialize identity to use XPO
            services
                .AddIdentity<ApplicationUser>(options => {
                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 3;					
                })
                /***** OLD OBSOLETE CONFIG < 23.2.3.31 
                .AddXpoIdentityStores<ApplicationUser, XpoApplicationUser>(connStrName,
					new ApplicationUserMapper(), 
					new ApplicationRoleMapper(),
					new XPUserStoreValidator<string, ApplicationUser, XpoApplicationUser>(),
					new XPRoleStoreValidator<string, ApplicationRole, XpoApplicationRole>())
		*/
		// When using AutoMapper
                .AddXpoAutoMapperIdentityStores<ApplicationUser/*, XpoApplicationUser*/>(connStrName)
		// When using Mapster
                .AddXpoMapsterIdentityStores<ApplicationUser/*, XpoApplicationUser*/>(connStrName)
                .AddDefaultTokenProviders();
				
		// token config
		builder.Services.AddScoped<ITokenService<string, ApplicationUser>, TokenService<string, ApplicationUser> >();

		var jwtSettings = builder.Configuration.GetSection("JWTSettings");
		builder.Services.AddAuthentication(opt =>
		{
			opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(options =>
		{
			options.TokenValidationParameters = new TokenValidationParameters
			{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,

			ValidIssuer = jwtSettings["validIssuer"],
			ValidAudience = jwtSettings["validAudience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["securityKey"]))
			};
		});


            // Add other services ...
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

```

#### DX.Blazor.Identity (Server/Wasm)

Initial publish with code to make incoporate MS Identity in you Blazor apps simple

##### Blazor Server steps:
Add package DX.Blazor.Identity.Server to project

In Startup.ConfigureServices:
```cs			
			services.AddScoped<IAuthService<RegisterUserModel>, AuthenticationService<ApplicationUser, RegisterUserModel>>();
			services.AddScoped<DX.Blazor.Identity.Server.TokenProvider>();
			services.AddScoped<AuthenticationStateProvider, DX.Blazor.Identity.Server.AuthStateProvider<ApplicationUser>>();
```
Create AuthorizationController:

```cs
    [Route("api/accounts")]
    public class AuthenticationController : AuthenticationControllerBase<ApplicationUser>
    {
        public AuthenticationController(UserManager<ApplicationUser> userManager, 
                SignInManager<ApplicationUser> signInManager, 
                IDataProtectionProvider dataProtectionProvider, 
                ILogger<AuthenticationControllerBase<string, ApplicationUser, RegisterUserModel>> logger, IConfiguration configuration) 
                : base(userManager, signInManager, dataProtectionProvider, logger, configuration)
        {

        }

    }

```


##### Blazor Hosted WASM steps:
In web api add DX.Blazor.Identity.Server

In startup.ConfigureServices (or Program.cs)

```c
builder.Services.AddScoped<IAuthService<RegistrationModel, AuthenticationModel>, AuthenticationService<ApplicationUser, RegistrationModel>>();
builder.Services.AddScoped<DX.Blazor.Identity.Server.TokenProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, DX.Blazor.Identity.Server.AuthStateProvider<ApplicationUser>>();
```

Create AuthorizationController and TokenController:

```cs

    [Route("api/accounts")]
    public class AccountController : AuthenticationControllerBase<ApplicationUser>
    {
        public AccountController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            IDataProtectionProvider dataProtectionProvider, 
            ILogger<AuthenticationControllerBase<ApplicationUser>> logger, 
            IConfiguration configuration) 
            : base(userManager, signInManager, dataProtectionProvider, logger, configuration)
        {

        }
    }


    [Route("api/token")]
	[ApiController]
	public class TokenController : TokenControllerBase<string, ApplicationUser>
    {
        public TokenController(UserManager<ApplicationUser> userManager, 
			ITokenService<string, ApplicationUser> tokenService) : base(userManager, tokenService)
        {

        }
    }

```


In WASM project add DX.Blazor.Identity.WASM, Blazored.LocalStorage and Toolbelt.HttpClientInterceptor

In Startup.ConfigureServices (or Program.cs)

```cs
    builder.Services.AddBlazoredLocalStorage(); //Blazored.LocalStorage											
    builder.Services.AddHttpClientInterceptor(); //Toolbelt.HttpClientInterceptor
    //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
    builder.Services.AddScoped(sp => new HttpClient
    {        
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress + "api/")
    }
    .EnableIntercept(sp));
    builder.Services.AddScoped<IAuthService<RegistrationModel, AuthenticationModel>, DX.Blazor.Identity.Wasm.Services.AuthenticationService>();
    builder.Services.AddScoped<DX.Blazor.Identity.Wasm.Services.RefreshTokenService>();
    builder.Services.AddScoped<AuthenticationStateProvider, DX.Blazor.Identity.Wasm.AuthStateProvider>();
    builder.Services.AddScoped<DX.Blazor.Identity.Wasm.Services.HttpInterceptorService>();

```


For both projects (WASM and Server) you can use the following Login.razor:


```cs
@inject NavigationManager navigationManager
@inject DX.Blazor.Identity.IAuthService AuthenticationService 

<EditForm Model="@userModel" .... >

</EditForm>

@code {
    AuthenticationModel usermodel = new AuthenticationModel();

    // ....

    protected async Task LoginAction()
    {
        errors.Clear();
        userModel.ReturnUrl = redirectUrl;
        var result = await AuthenticationService.Login(userModel);
        if (!result.IsAuthSuccessful)
        {
            errors.Add(result.ErrorMessage);            
        }
        else
        {
            navigationManager.NavigateTo("/");
        }              
    }
}



#### DX.Data.Xpo.Mvc (For ASP.NET Framework ASPxGridView MVC Extension only)

**Please note:** _You will need at least an **active** [DevExpress ASP.NET **License**](https://www.devexpress.com/products/net/controls/asp/) for this package_

Some helper classes to have the ASPxGridView extensions support server-side filtering and sorting based on the XPDataStore implementation.

More documentation to follow...
