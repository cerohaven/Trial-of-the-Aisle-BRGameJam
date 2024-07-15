
using UnityEngine;

//When a script calls the class that inherits from Singleton, it will create a gameObject if there is not already one in the scene
public class Singleton <T> : MonoBehaviour where T: MonoBehaviour
{
    public static T _instance;

    //Getter.
    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));

                if (_instance != null) return _instance;

                GameObject go = new GameObject();
                _instance = go.AddComponent<T>();
            }

            return _instance;
        }
    }
}
