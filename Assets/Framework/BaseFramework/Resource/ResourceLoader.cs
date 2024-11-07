using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Framework.Architecture;
using Framework.UI;
using Framework.Scene;
using Framework.Singleton;
using System;

namespace Framework
{
    public class ResourceLoader : Singleton<ResourceLoader>
    {
        protected override void OnInit()
        {
            base.OnInit();
        }

        protected override void OnRelease()
        {
            base.OnRelease();
        }

        public void LoadControllerAsync<T>(ResourceAttribute _attribute, WeakReference<T> _model) where T : IModel
        {
            Action<GameObject> _onLoadComplete = (_go) => 
            {
                var _controller = _go.GetComponent<IController>();
                _controller.SetModel(_model.Target);
            };
                
            Asset.LoadAssetAsync<GameObject>(_attribute, _onLoadComplete);
        }

        public void LoadSceneControllerAsync<T>(SceneAttribute _attribute, T _scene, Action<SceneModelBase> _onLoadComplete) where T : SceneModelBase
        {
            Asset.LoadSceneAsync(_attribute, _scene,_onLoadComplete);
        }

        public void LoadGameObject<T>(ResourceAttribute _attribute, WeakReference<T> _component) where T : Component
        {
            Action<GameObject> _onLoadComplete = (_go) =>
            {
                var _component = _go.GetComponent<T>();
            };
            Asset.LoadAssetAsync<GameObject>(_attribute, _onLoadComplete);
        }
    }

    /// <summary>
    /// Asset�� �����ϱ� ���� ��ü
    /// </summary>
    internal class Asset
    {
        public bool IsBundleMode { private get; set; }

        protected Asset()
        {
        }
        static readonly Asset instance = new Asset();

        Dictionary<string, List<string>> assetBundleManifest = new Dictionary<string, List<string>>();
        
        /// <summary>
        /// Load�� AssetBundle�� wraaping�� ��ü 
        /// </summary>
        private class AssetBundleElement
        {
            static readonly Dictionary<string, AssetBundleElement> loadedBundleElements = new Dictionary<string, AssetBundleElement>();

            static public AssetBundleElement CreateAssetBundleElement(string _name)
            {
                if (!loadedBundleElements.TryGetValue(_name, out AssetBundleElement _bundleElement))
                {
                    _bundleElement = new AssetBundleElement(_name, AssetBundle.LoadFromFileAsync(_name));
                    loadedBundleElements.Add(_name, _bundleElement);
                }

                return _bundleElement;                    
            }
            private AssetBundleElement(string _name, AssetBundleCreateRequest _request)
            {
                this.Name = _name;
                this.Request = _request;
            }
            public string Name { get; private set; }
            public AssetBundleCreateRequest Request { get; private set; }
            public bool IsDone 
            {
                get
                {
                    return (this.Request != null) ? this.Request.isDone : false;
                }
            }
            public AssetBundle Bundle
            {
                get
                {
                    return (Request != null) ? this.Request.assetBundle : null;
                }
            }

            public bool TryLoad(List<string> _dependenciesBundleName)
            {
                bool _ret = this.IsDone;

                for (int i=0;i< _dependenciesBundleName.Count;++i)
                {
                    var _bundleElement = CreateAssetBundleElement(_dependenciesBundleName[i]);
                    if (_bundleElement == null || !_bundleElement.IsDone)
                    {
                        _ret = false;
                    }
                }
                return _ret;
            }

            public void Unload(bool _unloadAllLoadedObjects)
            {
                if(this.Bundle != null)
                    this.Bundle.Unload(_unloadAllLoadedObjects);

                loadedBundleElements.Remove(this.Name);
            }
        }

        static public void LoadAssetAsync<T>(ResourceAttribute _attribute, Action<T> _onLoadAsset) where T : UnityEngine.Object
        {
            RootMonoBehaviour.Start_Coroutine(instance.OoLoadAssetAsync(_attribute, _onLoadAsset));
        }
        static public void LoadSceneAsync(SceneAttribute _attribute, SceneModelBase _scene, Action<SceneModelBase> _onLoadScene)
        {
            RootMonoBehaviour.Start_Coroutine(instance.OoLoadSceneAsync(_attribute, _scene, _onLoadScene));
        }

        private IEnumerator OoLoadSceneAsync(SceneAttribute _attribute, SceneModelBase _scene, Action<SceneModelBase> _onLoadAsset)
        {
            AsyncOperation _loadQuery = null;
#if UNITY_EDITOR            
            if (!IsBundleMode)
            {
                LoadSceneParameters _param = new LoadSceneParameters
                {
                    loadSceneMode = _attribute.LoadMode
                };
                _loadQuery = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(_attribute.Path, _param);
            }
            else
#endif
            {
                string _bundleName = _attribute.BundePath();

                if (!assetBundleManifest.TryGetValue(_bundleName, out List<string> _dependencies))
                {
                    yield break;
                }

                AssetBundleElement _element = AssetBundleElement.CreateAssetBundleElement(_bundleName);
                while (!_element.TryLoad(_dependencies))
                {
                    yield return null;
                }
                _loadQuery = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_attribute.Path, _attribute.LoadMode);
            }

            while (!_loadQuery.isDone)
            {
                yield return null;
            }
            _onLoadAsset(_scene);
        }

        private IEnumerator OoLoadAssetAsync<T>(ResourceAttribute _attribute, Action<T> _onLoadAsset) where T : UnityEngine.Object
        {
            T _obj = null;
#if UNITY_EDITOR
            if (!IsBundleMode)
            {
                _obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(_attribute.Path);
            }
            else
#endif
            {
                string _bundleName = _attribute.BundePath();

                if (!assetBundleManifest.TryGetValue(_bundleName, out List<string> _dependencies))
                {
                    yield break;
                }

                AssetBundleElement _element = AssetBundleElement.CreateAssetBundleElement(_bundleName);
                while (!_element.TryLoad(_dependencies))
                {
                    yield return null;
                }

                var _request = _element.Bundle.LoadAssetAsync<T>(_attribute.Path);
                while (!_element.IsDone)
                {
                    yield return null;
                }
                _obj = _request.asset as T;
            }

            T _instance = GameObject.Instantiate<T>(_obj);
            _onLoadAsset(_instance);
        }
    }
}
