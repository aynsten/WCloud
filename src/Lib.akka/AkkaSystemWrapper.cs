using Akka.Actor;
using Lib.ioc;

namespace Lib.akka
{
    public class AkkaSystemWrapper : ISingleInstanceService
    {
        private readonly ActorSystem _sys;

        public AkkaSystemWrapper(ActorSystem sys)
        {
            this._sys = sys;
        }

        public ActorSystem System => this._sys;

        public int DisposeOrder => int.MinValue;

        public void Dispose()
        {
            this._sys?.Dispose();
        }
    }
}
