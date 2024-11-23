using Framework.Architecture;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Scene
{
    /// <summary>
    /// Scene의 Controller
    /// </summary>
    /// <typeparam name="TModel">Scene모델 타입</typeparam>
    public abstract class SceneController<TModel> : IController
        where TModel : SceneModelBase
    {
        public override bool SetModel(IModel _model) 
        {
            if (base.SetModel(_model) == false)
                return false;

            _model.SetController(this);

            return true;
        }

        protected override void Awake()
        {
            this.SetModel(SceneModelBase.GetSceneModel(this.GetType()));
        }
        protected override void Start()
        {
            var _sceneModel = this.GetModel();
            if (_sceneModel == null)
            {
                Debug.LogWarningFormat("[{0}]Scene이 존재하지 않아 [{1}] Controller의 연결이 실패 했습니다.", typeof(TModel).ToString(), this.ToString());
                GameObject.Destroy(this.gameObject);
                return;
            }

            this.OnRefreshView();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected virtual void OnDestroy()
        {
        }
    }

    public abstract class SceneModelBase : IModel
    {
        private readonly static Dictionary<Type, SceneModelBase>   sceneModels = new Dictionary<Type, SceneModelBase>();
        readonly SceneAttribute attribute = null;

        public static SceneModelBase GetSceneModel(Type _controllerType)
        {
            SceneModelBase _model = null;
            if(sceneModels.TryGetValue(_controllerType, out _model))
            {
                return _model;
            }
            return null;
        }
        protected static void RegistSceneModel(Type _controllerType, SceneModelBase _model)
        {
            if (sceneModels.ContainsKey(_controllerType))
            {
                sceneModels.Remove(_controllerType);
                Debug.LogWarningFormat("Aleady Exist Key!! - {0}", _controllerType);
            }
            sceneModels.Add(_controllerType, _model);
        }

        public SceneModelBase()
        {
            this.attribute = SceneAttribute.GetAttribute(this.GetType());
        }

        public override void Dispose()
        {
            base.Dispose();
            sceneModels.Remove(this.ControllerType);
        }

        public static void CreateScene<T>(Action<SceneModelBase> _onLoadComplete) where T : SceneModelBase
        {
            var _ret = typeof(T).GetConstructor(Type.EmptyTypes).Invoke(null) as T;
            ResourceLoader.Instance.LoadSceneControllerAsync<T>(_ret.attribute, _ret, _onLoadComplete);
        }

        /// <summary>
        /// Scene에 진입하는 시점에 호출
        /// </summary>
        /// <param name="_beforeActiveScene">이전에 활성화 되어있던 Scene</param>
        public virtual void OnEnterScene(SceneModelBase _beforeActiveScene)
        {

        }

        /// <summary>
        /// Scene에서 나가는 시점에 호출
        /// </summary>
        /// <param name="_nextActiveScene">활성화 되는 Scene</param>
        public virtual void OnExitScene(SceneModelBase _nextActiveScene)
        {

        }
        /// <summary>
        /// Model의 갱신을 Controller을 통해 View에 알림
        /// </summary>
        public override void UpdateData()
        {
            if(this.Controller)
                this.Controller.RefreshDirtyFlag = true;
        }
    }
    /// <summary>
    /// Scene의 Contents Logic과 Data를 가지고 있는 Model 객체
    /// </summary>
    public abstract class SceneModel<TController> : SceneModelBase
        where TController : Architecture.IController
    {
        public SceneModel()
        {
            SceneModelBase.RegistSceneModel(typeof(TController), this);
        }

        public override void SetController(Architecture.IController _controller)
        {
            if(!_controller is TController)
            {
                Debug.LogErrorFormat("Controller type must be {0}!! arg = {1}", typeof(TController), _controller.GetType());
                return;
            }
            base.SetController(_controller);
         }

        public TController GetController()
        {
            return this.Controller as TController;
        }
    }
}
