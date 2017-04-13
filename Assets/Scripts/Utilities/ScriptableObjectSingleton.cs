using UnityEngine;

public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObjectSingleton<T>
{
	static T _instance = null;
	public static T I
	{
		get
		{
			if(!_instance)
			{
				_instance = Resources.Load(typeof(T).ToString()) as T;
				
#if UNITY_EDITOR
				if(!_instance)
					_instance = ScriptableObjectUtility.CreateAsset<T>();
#endif
			}
			
			return _instance;
		}
	}
}