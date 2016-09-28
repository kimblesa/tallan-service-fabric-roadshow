using Owin;

namespace EventHubActor.Interfaces
{
    public interface IOwinAppBuilder
    {
        void Configuration(IAppBuilder appBuilder);
    }
}
