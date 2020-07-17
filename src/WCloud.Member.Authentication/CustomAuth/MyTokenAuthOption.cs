using Lib.helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using FluentAssertions;

namespace WCloud.Member.Authentication.CustomAuth
{
    public class MyTokenAuthOption : AuthenticationSchemeOptions
    {
        public CookieBuilder CookieOption { get; set; } = new CookieBuilder();

        public override void Validate()
        {
            this.CookieOption.Should().NotBeNull();
            this.CookieOption.Name.Should().NotBeNullOrEmpty();
            base.Validate();
        }
    }
}
