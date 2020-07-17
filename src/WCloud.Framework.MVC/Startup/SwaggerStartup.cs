using FluentAssertions;
using Lib.extension;
using Lib.helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace WCloud.Framework.Startup
{
    public static class SwaggerStartup
    {
        public static bool SwaggerEnabled(this IConfiguration _config)
        {
            var res = (_config["swagger"] ?? "false").ToBool();
            return res;
        }

        public static IServiceCollection AddSwaggerDoc_(this IServiceCollection collection,
            Assembly apiAssembly, string service_name,
            OpenApiInfo info = null,
            Action<SwaggerGenOptions> optionBuilder = null)
        {
            collection.AddSwaggerGen(option =>
            {
                info ??= new OpenApiInfo();
                info.Title ??= service_name;

                option.SwaggerDoc(name: service_name, info: info);
                option.DocInclusionPredicate((docName, description) => true);
                option.CustomSchemaIds(type => type.FullName);

                var assembly_name = apiAssembly.GetName().Name;
                var path = Path.Combine(AppContext.BaseDirectory, $"{assembly_name}.xml");
                if (File.Exists(path))
                {
                    option.IncludeXmlComments(path);
                }

                optionBuilder?.Invoke(option);
            });
            return collection;
        }

        public static SwaggerGenOptions UseOauth_(this SwaggerGenOptions option, IConfiguration config,
            bool enable_oauth_flow = true,
            bool enable_bearer_auth = true)
        {
            var securityRequirement = new OpenApiSecurityRequirement();

            if (enable_oauth_flow)
            {
                var identity_server = config.GetIdentityServerAddressOrThrow();
                // Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)
                var oauth_flow = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {
                        Implicit = new OpenApiOAuthFlow()
                        {
                            AuthorizationUrl = new Uri($"{identity_server}/connect/authorize"),
                            Scopes = new Dictionary<string, string>()
                            {
                                ["openid"] = "openid",
                                ["offline_access"] = "offline_access",
                                ["water"] = "water",
                            }
                        },
                    },
                };
                option.AddSecurityDefinition("oauth2", oauth_flow);
                securityRequirement.Add(new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "oauth2"
                    }
                }, new string[] { });
            }

            if (enable_bearer_auth)
            {
                var bearer_auth = new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "请输入OAuth接口返回的Token，前置Bearer。示例：Bearer {Token}",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                };
                option.AddSecurityDefinition("Bearer", bearer_auth);
                securityRequirement.Add(new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                }, new string[] { });
            }

            option.AddSecurityRequirement(securityRequirement);

            return option;
        }

        /// <summary>
        /// 暴露接口定义json
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSwaggerDefaultDefinitionJson(this IApplicationBuilder app)
        {
            //在网关层不要用这种route模板暴露json，否则会被swagger中间件拦截json请求。
            //这时应该另外定义模板，然后把api开头的url交给网关转发到下层服务
            app.UseSwagger(option => option.RouteTemplate = __default_template__("{documentName}"));
            return app;
        }

        /// <summary>
        /// 暴露接口定义json
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSwaggerGatewayDefinitionJson(this IApplicationBuilder app)
        {
            app.UseSwagger(option => option.RouteTemplate = __gateway_template__("{documentName}"));
            return app;
        }

        /// <summary>
        /// swagger/{documentName}/swagger.json
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static string __default_template__(string name) => $"/swagger/{name}/swagger.json";
        static string __gateway_template__(string name) => $"/api/{name}/swagger/swagger.json";

        public static IApplicationBuilder UseSwaggerWithUI(this IApplicationBuilder app,
            string[] gateway_template_services,
            string[] default_template_services = null,
            Dictionary<string, string> endpoints = null,
            Action<SwaggerUIOptions> optionBuilder = null)
        {
            gateway_template_services.Should().NotBeNull();
            //ui
            app.UseSwaggerUI(option =>
            {
                endpoints ??= new Dictionary<string, string>();
                //default template
                if (ValidateHelper.IsNotEmpty(default_template_services))
                {
                    foreach (var name in default_template_services)
                    {
                        endpoints[name] = __default_template__(name);
                    }
                }

                //gateway template
                foreach (var name in gateway_template_services)
                {
                    endpoints[name] = __gateway_template__(name);
                }

                //添加json节点
                foreach (var kv in endpoints)
                {
                    option.SwaggerEndpoint(url: kv.Value, name: kv.Key);
                }

                optionBuilder?.Invoke(option);
            });
            return app;
        }
    }
}
