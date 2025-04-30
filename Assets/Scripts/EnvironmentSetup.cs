using UnityEngine;

public static class EnvironmentSetup
{
    public static void ConfigurePrefix(HTTPData http)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string host = Application.absoluteURL;

        if (host.Contains("princetonplatformer.onrender.com"))
        {
            http.prefix = "https://princetonplatformer.onrender.com";
        }
        else
        {
            http.prefix = "http://localhost:5000";
        }
#else
        http.prefix = "http://localhost:5000";
#endif

        Debug.Log("Website URL set to: " + http.prefix);
    }
}
