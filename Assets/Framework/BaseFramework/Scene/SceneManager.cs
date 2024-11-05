using Framework.Singleton;
using System.Collections;
using UnityEngine;

namespace Framework.Scene
{
    public class SceneManager : Singleton<SceneManager>
    {
        SceneModelBase currentScene = null;

        protected override void OnInit()
        {
        }

        protected override void OnRelease()
        {
        }

        public void ChangeScene<T>() where T : SceneModelBase
        {
            SceneModelBase.CreateScene<T>(this.OnLoadSceneComplete);
        }

        void OnLoadSceneComplete(SceneModelBase _scene)
        {
            if(currentScene != null)
            {
                currentScene.OnExitScene(_scene);
            }
            if(_scene != null)
            {
                _scene.OnEnterScene(currentScene);
            }
            currentScene = _scene;
        }
    }
}