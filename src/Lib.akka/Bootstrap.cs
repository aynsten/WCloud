using Akka.Actor;
using Lib.helper;
using Lib.ioc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Lib.akka
{
    public static class AkkaActorBootstrap
    {
        public static IServiceCollection UseAkkaSystem(this IServiceCollection collection,
            string system_name)
        {
            if (ValidateHelper.IsEmpty(system_name))
                throw new ArgumentException(nameof(system_name));
            /*
            if (collection.ExistService_(typeof(ActorSystem)))
                throw new AlreadyExistException("已经存在actor system");
                */

            var sys = new AkkaSystemWrapper(ActorSystem.Create(system_name));
            collection.AddDisposableSingleInstanceService(sys);
            return collection;
        }
    }
}
