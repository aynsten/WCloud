// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;

namespace IdentityServer4.Quickstart.UI
{
    public class LoginViewModel
    {
        public string ReturnUrl { get; set; }

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();

        public IEnumerable<ExternalProvider> VisibleExternalProviders =>
            ExternalProviders.Where(x => !String.IsNullOrWhiteSpace(x.DisplayName));
    }
}