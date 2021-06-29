using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static T instance = null;

    public static T Instance {
        get {
            if (instance == null) {
                instance = GameObject.FindObjectOfType<T>();

                if (instance == null) {
                    throw new UnityException("No singleton object for " + typeof(T).ToString() + " exists in this scene.");
                }
            }

            return instance;
        }
    }

    public static bool DoesExist() {
        if (instance == null) {
            instance = GameObject.FindObjectOfType<T>();
        }

        return instance != null;
    }

    /// <summary>
    /// Call this (usually in Awake()) to make a singleton persistent across scenes.
    /// This will kill an instance that tries to be persistent but isn't the current instance.
    /// </summary>
    /// <param name="mb">The MonoBehaviour to make persistent.</param>
    /// <returns>Returns true if the function successfully made the singleton persistent.</returns>
    public static bool DontDestroyElseKill(Singleton<T> mb) {
        if (mb == Instance) {
            MonoBehaviour.DontDestroyOnLoad(Instance.gameObject);
            return true;
        } else {
            MonoBehaviour.Destroy(mb.gameObject);
            return false;
        }
    }

    /// <summary>
    /// Makes this object a persistent singleton unless the singleton already exists, in which case
    /// the current object is destroyed.
    /// </summary>
    /// <returns>Returns true if the function successfully made the singleton persistent.</returns>
    protected bool MakePersistent() {
        return DontDestroyElseKill(this);
    }

    protected bool IsInstance {
        get {
            return Instance == this;
        }
    }
}
