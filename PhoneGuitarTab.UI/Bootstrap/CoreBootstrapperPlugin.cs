﻿using PhoneGuitarTab.Core.Bootstrap;
using PhoneGuitarTab.Core.Dependencies;
using PhoneGuitarTab.Core.Diagnostic;
using PhoneGuitarTab.Core.Services;
using PhoneGuitarTab.Search;
using PhoneGuitarTab.Search.Lastfm;
using PhoneGuitarTab.Search.UltimateGuitar;
using PhoneGuitarTab.Search.SoundCloud;
using PhoneGuitarTab.UI.Infrastructure;

namespace PhoneGuitarTab.UI.Bootstrap
{
    public class CoreBootstrapperPlugin : IBootstrapperPlugin
    {
        public string Name
        {
            get { return "Core"; }
        }

        [Dependency]
        private IContainer Container { get; set; }

        public bool Run()
        {
            Container.Register(Component.For<ITrace>().Use<DefaultTrace>().Singleton());
            Container.Register(Component.For<TraceCategory>().Use<TraceCategory>("Default").Singleton());
                // TODO create several named categories

            Container.Register(Component.For<IFileSystemService>().Use<IsolatedStorageFileService>().Singleton());
            Container.Register(Component.For<TabFileStorage>().Use<TabFileStorage>().Singleton());
            Container.Register(Component.For<ISettingService>().Use<IsolatedStorageSettingService>().Singleton());

            Container.Register(Component.For<MessageHub>().Use<MessageHub>().Singleton());

            Container.Register(Component.For<RatingService>().Use<RatingService>().Singleton());
            Container.Register(Component.For<IDialogController>().Use<ToastDialogController>().Singleton());

            Container.Register(Component.For<ITabSearcher>().Use<UltimateGuitarTabSearcher>());
            Container.Register(Component.For<IAudioSearcher>().Use<SoundCloudSearch>());
            Container.Register(Component.For<IMediaSearcherFactory>().Use<MediaSearcherFactory>());
           
            return true;
        }

        public bool Update()
        {
            return true;
        }
    }
}