using UnityEngine;
using System.Collections;

public class LoadingScript : MonoBehaviour {

    int i = 0;
    public string scenetoload;
    IEnumerator Start()
    {
        AsyncOperation op = Application.LoadLevelAsync(scenetoload);
        Debug.Log("Loading STARTED");
        while (!op.isDone)
        {
            i++;
            Debug.Log("Loading " + i);//just to see when async load is done
            yield return 0;
        }
        Debug.Log("Loading DONE");
    }

}
