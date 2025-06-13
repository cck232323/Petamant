using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyDotnetApp.Data;
using MyDotnetApp.Services;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器
builder.Services.AddControllers();
builder.Services.AddControllersWithViews(); // 在 builder.Services.AddControllers(); 之后添加
builder.Services.AddEndpointsApiExplorer();
// 添加服务注册
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "宠物活动聚合社交平台 API", Version = "v1" });
    
    // 添加JWT认证配置到Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// 使用内存数据库替代PostgreSQL
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseInMemoryDatabase("PetActivitiesDb"));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 添加AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// 添加服务
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
// 添加其他服务...
// builder.Services.AddAuthorization(options =>
// {
//     // 修改为不使用默认策略，而是添加一个命名策略
//     options.AddPolicy("RequireAuthenticated", policy =>
//         policy.RequireAuthenticatedUser());
// });
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .Build();
});

// JWT 认证配置
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = builder.Configuration["Jwt:Issuer"],
//             ValidAudience = builder.Configuration["Jwt:Audience"],
//             IssuerSigningKey = new SymmetricSecurityKey(
//                 Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "defaultkey_for_development_only"))
//         };
//     });
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"] ?? "DefaultSecretKeyForDevelopmentOnly12345678901234567890")),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "default_issuer",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "default_audience",
            ClockSkew = TimeSpan.Zero
        };
    });

// 在 builder.Services 部分添加

// Console.WriteLine($"Database provider: {builder.Services.BuildServiceProvider().GetService<ApplicationDbContext>()?.Database.ProviderName}");

var app = builder.Build();

// 初始化数据库
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "初始化数据库时出错");
    }
}

// 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "宠物活动聚合社交平台 API v1"));
}

// app.UseStaticFiles(); // 确保这行代码在 UseAuthentication 和 UseAuthorization 之前
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // 对于静态文件请求不要求身份验证
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=600");
    }
});
app.Use(async (context, next) =>
{
    // 如果是静态文件请求，跳过授权检查
    if (context.Request.Path.StartsWithSegments("/js") || 
        context.Request.Path.StartsWithSegments("/css") || 
        context.Request.Path.StartsWithSegments("/lib") || 
        context.Request.Path.StartsWithSegments("/images"))
    {
        context.Items["SkipAuthorization"] = true;
    }
    
    await next();
});
app.UseRouting();
app.UseAuthentication(); // 确保这个在 UseAuthorization 之前
app.UseAuthorization();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();