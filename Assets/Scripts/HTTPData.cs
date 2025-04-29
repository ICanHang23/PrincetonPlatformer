using UnityEngine;

[CreateAssetMenu]
public class HTTPData : ScriptableObject
{
    public string prefix()
    {
#if UNITY_EDITOR
        Debug.Log("On unity editor");
        return "http://localhost:5000";
#endif
        Debug.Log("On browser");
        return "https://princetonplatformer.onrender.com";
    }
}
