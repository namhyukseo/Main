using Framework.UI;
using System;

namespace Framework.Architecture
{
    public class ResourceAttribute : Attribute
    {
        /// <summary>
        /// Resource의 Path
        /// </summary>
        public string Path { get; set; }
        public virtual string BundePath() { return Path; }
    }
    /// <summary>
    /// Window의 속성
    /// </summary>
    public class WindowAttribute : ResourceAttribute
    {
        static public WindowAttribute GetAttribute<T>() where T : Architecture.IModel
        {
            return GetAttribute(typeof(T));
        }
        static public WindowAttribute GetAttribute(Type _type)
        {
            var _attribute = _type.GetCustomAttributes(typeof(WindowAttribute), true);
            if (_attribute.Length == 0)
            {
                UnityEngine.Debug.LogErrorFormat("[{0}]윈도우에 Attribute가 정의 되어있지 않습니다.", _type.ToString());
            }
            return _attribute[0] as WindowAttribute;
        }
        public override string BundePath()
        {
            return "bundle";
        }
        public WINDOW_LAYER Layer { get; set; }
        public bool IsModal { get; set; }
    }

    /// <summary>
    /// Scene의 속성
    /// </summary>
    public class SceneAttribute : ResourceAttribute
    {
        static public SceneAttribute GetAttribute<T>() where T : Architecture.IModel
        {
            return GetAttribute(typeof(T));
        }
        static public SceneAttribute GetAttribute(Type _type)
        {
            var _attribute = _type.GetCustomAttributes(typeof(SceneAttribute), true);
            if (_attribute.Length == 0)
            {
                UnityEngine.Debug.LogErrorFormat("[{0}]Scene에 Attribute가 정의 되어있지 않습니다.", _type.ToString());
            }
            return _attribute[0] as SceneAttribute;
        }
        public override string BundePath()
        {
            return "bundle";
        }
        public UnityEngine.SceneManagement.LoadSceneMode LoadMode { get; set; }
    }
}