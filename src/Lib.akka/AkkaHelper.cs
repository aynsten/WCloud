using Akka.Actor;
using Lib.extension;
using Lib.helper;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lib.akka
{
    /// <summary>
    /// akka系统管理
    /// 邮箱里搜索akka
    /// actor是轻量级的，但是任务结束后最好关闭actor，可以用self.stop或者system.stop
    /// </summary>
    public static class AkkaHelper
    {
        public static ActorSystem AkkaSystem;

        public static IActorRef GetActor<T>(string name = null)
            where T : ActorBase, new() =>
            AkkaSystem.CreateActor<T>(name);

        public static IActorRef GetActor(Type t, string name = null) => AkkaSystem.CreateActor(t, name);
    }

    public static class AkkaHelper<T> where T : ActorBase, new()
    {
        public static IActorRef GetActor(string name = null) => AkkaHelper.GetActor<T>(name);

        [Obsolete("每次都会创建新的actor")]
        public static void Tell(object data, string actor_name = null)
        {
            GetActor(actor_name).Tell(data);
        }

        [Obsolete("每次都会创建新的actor")]
        public static async Task<Answer> Ask<Answer>(object data, string actor_name = null,
            TimeSpan? timeout = null, CancellationToken? cancellationToken = null)
        {
            var actor = GetActor(actor_name);
            if (timeout != null && cancellationToken != null)
            {
                return await actor.Ask<Answer>(data, timeout.Value, cancellationToken.Value);
            }
            if (timeout != null)
            {
                return await actor.Ask<Answer>(data, timeout: timeout.Value);
            }
            if (cancellationToken != null)
            {
                return await actor.Ask<Answer>(data, cancellationToken: cancellationToken.Value);
            }

            return await GetActor(actor_name).Ask<Answer>(data);
        }
    }

    public abstract class KillableReceiveActor : ReceiveActor
    {
        public KillableReceiveActor()
        {
            this.Receive<ActorPoisonPill>(x => this.OnReceivePoisonPill(x));
        }

        protected virtual void OnReceivePoisonPill(ActorPoisonPill pill)
        {
            foreach (var child in Context.GetChildren())
            {
                child.FeedPoisonPill();
                //Context.Stop(child);
            }
            Context.Stop(this.Self);
        }

        protected override void PostStop()
        {
            $"{this.GetType()}stoped".AddBusinessInfoLog();
            base.PostStop();
        }
    }

    public class ActorPoisonPill
    {
        public static readonly ActorPoisonPill Instance = new ActorPoisonPill();
    }

    public static class ActorExtension
    {
        /// <summary>
        /// 通知停止actor
        /// </summary>
        public static void FeedPoisonPill(this IActorRef actor)
        {
            actor.Tell(ActorPoisonPill.Instance);
        }

        public static IActorRef CreateActor(this ActorSystem sys, Type t, string name = null)
        {
            if (ValidateHelper.IsNotEmpty(name))
            {
                return sys.ActorOf(Props.Create(t), name);
            }
            else
            {
                return sys.ActorOf(Props.Create(t));
            }
        }

        public static IActorRef CreateActor<T>(this ActorSystem sys, string name = null)
            where T : ActorBase, new()
        {
            if (ValidateHelper.IsNotEmpty(name))
            {
                return sys.ActorOf<T>(name);
            }
            else
            {
                return sys.ActorOf<T>();
            }
        }
    }
}
