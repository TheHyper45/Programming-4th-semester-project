using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DatabaseManagement : MonoBehaviour {
    [SerializeField]
    private string fileName;

    public DatabaseModel Model { get; private set; }

    private string FilePath => Application.isEditor ? $"./Assets/StreamingAssets/{fileName}" : $"{Application.persistentDataPath}/{fileName}";

    private byte[] GetStreamingAssetBytesForAndroid(string path) {
        var loadingRequest = UnityWebRequest.Get(path);
        loadingRequest.SendWebRequest();
        while(!loadingRequest.isDone) {
            if(loadingRequest.result == UnityWebRequest.Result.ConnectionError ||
                loadingRequest.result == UnityWebRequest.Result.ProtocolError) {
                break;
            }
        }
        if(loadingRequest.result == UnityWebRequest.Result.ConnectionError ||
            loadingRequest.result == UnityWebRequest.Result.ProtocolError) {
            return null;
        }
        return loadingRequest.downloadHandler.data;
    }

    private void Start() {
        if(!Application.isEditor && !File.Exists(FilePath)) {
            var streamingPath = Path.Combine(Application.streamingAssetsPath,fileName);
            if(Application.platform == RuntimePlatform.Android) {
                var bytes = GetStreamingAssetBytesForAndroid(streamingPath);
                if(bytes != null) {
                    File.WriteAllBytes(FilePath,bytes);
                }
            }
            else File.Copy(streamingPath,FilePath);
        }
        Model = new(FilePath);
    }
}
