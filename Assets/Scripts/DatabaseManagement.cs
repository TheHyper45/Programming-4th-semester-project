using System;
using System.IO;
using UnityEngine;
using SQLite4Unity3d;
using UnityEngine.Networking;

public class DatabaseManagement : MonoBehaviour {
    [Table("Osoby")]
    public class PearsonEntity {
        [Column("Id"),PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        [Column("Login"),NotNull]
        public string Login { get; set; }
        [Column("Has³o"),NotNull]
        public string Password { get; set; }
        [Column("Imiê"),NotNull]
        public string FirstName { get; set; }
        [Column("Nazwisko"),NotNull]
        public string LastName { get; set; }
        [Column("P³eæ"),NotNull]
        public bool Sex { get; set; }
    };

    [Table("Lista_Spotkañ")]
    public class MeetingEntity
    {
        [Column("Ocena"),NotNull]
        public int Grade { get; set; }
        [Column("IdOsoby"),NotNull,ForeignKey("Osoby","Id")]
        public int PersonId { get; set; }
    };
    public class NonexistentAccountException : Exception {
        public NonexistentAccountException() { }
    };

    [SerializeField]
    private string fileName;

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
            throw new UnityException($"Couldn't load \"{path}\".");
        }
        return loadingRequest.downloadHandler.data;
    }

    private void Start() {
        if(!Application.isEditor && !File.Exists(FilePath)) {
            var streamingPath = Path.Combine(Application.streamingAssetsPath,fileName);
            var bytes = (Application.platform == RuntimePlatform.Android) ? GetStreamingAssetBytesForAndroid(streamingPath) : File.ReadAllBytes(streamingPath);
            File.WriteAllBytes(FilePath,bytes);
        }
        using var conn = new SQLiteConnection(FilePath,true);
        conn.CreateTable<PearsonEntity>();
        conn.CreateTable<MeetingEntity>();
    }

    private string currentLogin = "";
    private string currentPassword = "";

    public void Connect(string login,string password) {
        using var conn = new SQLiteConnection(FilePath,true);
        var cmd = conn.CreateCommand($"select * from Osoby where Login = \"{login}\" and Has³o = \"{password}\";");

        var entities = cmd.ExecuteQuery<PearsonEntity>();
        if(entities.Count >= 1) {
            currentLogin = login;
            currentPassword = password;
            return;
        }
        throw new NonexistentAccountException();
    }

    public void CreateAccount(string login,string password,string firstName,string lastName,bool isMale) {
        using var conn = new SQLiteConnection(FilePath,true);
        var cmd = conn.CreateCommand($"insert into Osoby (Login,Has³o,Imiê,Nazwisko,P³eæ) values (\"{login}\",\"{password}\",\"{firstName}\",\"{lastName}\",{(isMale ? 1 : 0)});");
        cmd.ExecuteNonQuery();
    }
}
