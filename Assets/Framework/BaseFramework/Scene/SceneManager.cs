using Framework.Architecture;
using Framework.Singleton;
using UnityEngine.Assertions;

namespace Framework.Scene
{
    public class SceneManager : Singleton<SceneManager>, IPostLateUpdate
    {
        WeakReference<SceneModelBase> currentScene  = new WeakReference<SceneModelBase>(null);
        WeakReference<SceneModelBase> nextScene     = new WeakReference<SceneModelBase>(null);

        protected override void OnInit()
        {
            base.OnInit();
        }

        protected override void OnRelease()
        {
            base.OnRelease();
        }

        public void ChangeScene<T>() where T : SceneModelBase
        {
            SceneModelBase.CreateScene<T>(this.OnLoadSceneComplete);
        }

        private void OnLoadSceneComplete(SceneModelBase _scene)
        {
            UnityEngine.Debug.Assert(!this.nextScene.IsAlive);
            UnityEngine.Debug.Assert(_scene != null);

            this.nextScene.Target = _scene;
        }

        public void OnPostLateUpdate(float _deltaTime)
        {
            if(this.nextScene.IsAlive)
            {
                if (this.currentScene.IsAlive)
                {
                    this.currentScene.Target.OnExitScene(this.nextScene.Target);
                }
                if (this.nextScene.IsAlive)
                {
                    this.nextScene.Target.OnEnterScene(this.currentScene.Target);
                }

                if(this.currentScene.IsAlive)
                    this.currentScene.Target.Dispose();

                this.currentScene.Target = this.nextScene.Target;
                this.nextScene.Target = null;
            }
        }
    }
}