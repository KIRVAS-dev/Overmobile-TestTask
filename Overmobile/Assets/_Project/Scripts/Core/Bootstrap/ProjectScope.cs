using Core.Bootstrap.Api;
using VContainer;
using VContainer.Unity;

namespace Core.Bootstrap
{
    public sealed class ProjectScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<CoreLoader>(Lifetime.Singleton).As<ISceneLoader>();
            builder.RegisterEntryPoint<EntryPoint>();
        }
    }
}
