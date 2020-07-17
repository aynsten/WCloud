// Copyright (c) Winton. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENCE in the project root for license information.

using System.Threading;
using Microsoft.Extensions.Configuration;

namespace Lib.zookeeper.configuration
{
    public sealed class ZKConfigurationSource : IConfigurationSource
    {
        private readonly IConfigurationProvider _provider;

        public ZKConfigurationSource(ZKConfigurationOption option, AlwaysOnZooKeeperClient client, CancellationToken cancellationToken)
        {
            this._provider = new ZKConfigurationProvider(option, client);
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return this._provider;
        }
    }
}