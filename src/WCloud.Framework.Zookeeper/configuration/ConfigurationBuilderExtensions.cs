// Copyright (c) Winton. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENCE in the project root for license information.

using System;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace Lib.zookeeper.configuration
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddZookeeper(
            this IConfigurationBuilder builder,
            AlwaysOnZooKeeperClient client,
            CancellationToken cancellationToken,
            Func<ZKConfigurationOption, ZKConfigurationOption> config = null)
        {
            var option = new ZKConfigurationOption();
            if (config != null)
            {
                option = config.Invoke(option);
            }

            var zkConfigSource = new ZKConfigurationSource(option, client, cancellationToken);

            return builder.Add(zkConfigSource);
        }
    }
}