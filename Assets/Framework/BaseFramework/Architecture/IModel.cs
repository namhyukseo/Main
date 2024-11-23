using System;

using MonoBehaviour = Framework.Architecture.MonoBehaviour;
namespace Framework.Architecture
{
    public abstract class IModel : IDisposable
    {
        private WeakReference<IController> controllerRef;
        protected IController Controller 
        {
            get
            {
                return controllerRef.Target;
            }
            private set
            {
                controllerRef = new WeakReference<IController>(value);
            }
        }
        public Type ControllerType { get; private set; }
        public virtual void Dispose()
        {
            if (this.Controller)
            {
                this.Controller.Dispose();
            }
            this.Controller = null;
        }
        public virtual void SetController(IController _controller)
        {
            this.Controller = _controller;
            this.ControllerType = _controller.GetType();
            this.UpdateData();
        }

        public abstract void UpdateData();
    }
}