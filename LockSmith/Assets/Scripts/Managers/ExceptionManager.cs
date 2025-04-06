using UnityEngine;

public static class ExceptionManager
{
    public static void ThrowException(string objectName, string scriptName, string message)
    {
        Debug.LogError($"ERROR on script {scriptName} on gameobject {objectName} : {message}");
    }
}
