// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace IdentityServer4.Quickstart.UI
{
    public class ProcessConsentResult
    {
        public string RedirectUri { get; set; }

        public ConsentViewModel ViewModel { get; set; }

        public string ValidationError { get; set; }
    }
}
