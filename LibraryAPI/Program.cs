
using LibraryAPI.Data;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Controllers;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Host.SystemWeb;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace LibraryAPI;

public class Program
{
    public static void Main(string[] args)
    {
        ApplicationContext _context;
        RoleManager<IdentityRole> _roleManager;
        UserManager<AppUser> _userManager;
        IdentityRole identityRole;
        AppUser appUser;
        
       
        var builder = WebApplication.CreateBuilder(args);
        var key = Encoding.ASCII.GetBytes(builder.Configuration["JWT:Key"]);
        // Add services to the container.

        builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationContext")));
        builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
        opt.TokenLifespan = TimeSpan.FromHours(2));


        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key)

                       
                    };
                });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
        });

        var app = builder.Build();

        

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        


        app.UseAuthentication();
        app.UseAuthorization();
        //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

        //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
        //{
        //    ClientId = "YOUR_GOOGLE_CLIENT_ID",
        //    ClientSecret = "YOUR_GOOGLE_CLIENT_SECRET"
        //});
        app.MapControllers();
        _context = app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationContext>();
        _roleManager= app.Services.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        _userManager= app.Services.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        _context.Database.Migrate();

        if (_roleManager.FindByNameAsync("Admin").Result==null)
        {
            identityRole = new IdentityRole("Admin");
            _roleManager.CreateAsync(identityRole).Wait();
        }

        if (_userManager.FindByNameAsync("Admin").Result == null)
        {
            appUser = new AppUser();
            appUser.UserName = "Admin";
            _userManager.CreateAsync(appUser, "Admin123!").Wait();
            _userManager.AddToRoleAsync(appUser, "Admin").Wait();
        }

        app.Run();
    }
}