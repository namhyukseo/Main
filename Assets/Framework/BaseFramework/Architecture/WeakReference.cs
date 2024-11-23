using System;

namespace Framework
{
    public sealed class WeakReference<T> 
    {
        public WeakReference()
        {
            target.Target = null;
        }
        public WeakReference(T _target)
        {
            this.target.Target = _target;
        }
        private readonly WeakReference target = new WeakReference(null);

        public bool IsAlive
        {
            get { return this.target.IsAlive; }
        }
        public T Target
        {
            get
            {
                return (T)target.Target;
            }
            set
            {
                target.Target = value;
            }
        }
    }
}