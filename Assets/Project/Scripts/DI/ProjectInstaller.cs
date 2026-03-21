using System.Collections.Generic;
using Project.Scripts.Game.GameRoot;
using Project.Scripts.Services;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Injectors;
using Unity.VisualScripting;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Project.Scripts.DI
{
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        private readonly List<object> _monoServices = new();
        private readonly List<GameObject> _monoServiceObjects = new();
        
        [SerializeField] private UIRootView _uiRootViewPrefab;
        [SerializeField] private GameEntryPoint _gameEntryPointPrefab;
        [SerializeField] private AudioSoundsService _audioSoundsServicePrefab;

        private void OnDestroy()
        {
            foreach (var obj in _monoServiceObjects)
            {
                if (obj != null) Destroy(obj);
            }
        }

        public void InstallBindings(ContainerBuilder builder)
        {
            RegisterCoreServices(builder);
            CreateMonoServices();
            RegisterCreatedServices(builder);
            RegisterContainerDependentServices(builder);
        }

        private void RegisterCoreServices(ContainerBuilder builder)
        {
            builder.RegisterType(
                typeof(ResourceService),
                new [] {typeof(IResourceService)},
                Lifetime.Singleton,
                Resolution.Lazy);
            
            builder.RegisterType(
                typeof(TweenAnimationService),
                new [] {typeof(ITweenAnimationService)},
                Lifetime.Singleton,
                Resolution.Lazy);
            
            builder.RegisterType(
                typeof(PauseService),
                new [] {typeof(IPauseService)},
                Lifetime.Singleton,
                Resolution.Lazy);
        }

        private void CreateMonoServices()
        {
            CreateService(_uiRootViewPrefab);
            CreateService(_gameEntryPointPrefab);
            CreateService(_audioSoundsServicePrefab);
        }

        private void CreateService<T>(T prefab)
            where T : MonoBehaviour
        {
            var instance = Instantiate(prefab);
            _monoServices.Add(instance);
            _monoServiceObjects.Add(instance.gameObject);
            DontDestroyOnLoad(instance);
        }

        private void RegisterCreatedServices(ContainerBuilder builder)
        {
            foreach (var service in _monoServices)
            {
                builder.RegisterValue(service);

                var serviceType = service.GetType();
                var interfaces = serviceType.GetInterfaces();

                foreach (var interfaceType in interfaces)
                {
                    builder.RegisterValue((serviceType, new[] {interfaceType} ));
                }
            }
        }

        private void RegisterContainerDependentServices(ContainerBuilder builder)
        {
            builder.OnContainerBuilt += container =>
            {
                foreach (var service in _monoServiceObjects)
                {
                    GameObjectInjector.InjectObject(service, container);
                }

                foreach (var service in _monoServices)
                {
                    if (service is IInitializable initializable)
                    {
                        initializable.Initialize();
                    }
                }
            };
        }
    }
}