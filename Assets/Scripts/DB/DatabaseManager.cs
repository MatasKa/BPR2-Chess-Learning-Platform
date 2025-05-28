using UnityEngine;
using System.IO;
using System.Linq;
using SQLite;
using System.Security.Cryptography;
using System.Text;

public class DatabaseManager : MonoBehaviour
{
    private string _dbPath;
    void Awake()
    {
        // Determine source and destination paths
        var streamingPath = Path.Combine(Application.streamingAssetsPath, "game.db");
        var persistentPath = Path.Combine(Application.persistentDataPath, "game.db");

        // If the database isn't in persistent storage yet, copy it
        if (!File.Exists(persistentPath))
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            // On Android StreamingAssets are compressed
            StartCoroutine(CopyAndroidDb(streamingPath, persistentPath));
#else
            File.Copy(streamingPath, persistentPath);
#endif
        }
        _dbPath = persistentPath;
    }
#if UNITY_ANDROID && !UNITY_EDITOR
    private System.Collections.IEnumerator CopyAndroidDb(string src, string dst)
    {
        using (var www = UnityEngine.Networking.UnityWebRequest.Get(src))
        {
            yield return www.SendWebRequest();
            File.WriteAllBytes(dst, www.downloadHandler.data);
        }
    }
#endif
    // Define the database tables
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int user_id { get; set; }
        public string email { get; set; }
        public string password_hash { get; set; }
        public string username { get; set; }
        public int darkmode { get; set; }
    }
    void Start()
    {
        using (var conn = new SQLiteConnection(_dbPath))
        {
            conn.CreateTable<User>();
        }
        //ClearAndResetAllData();
    }
    public static string HashPassword(string password)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(password);

        using (var sha = SHA256.Create())
        {
            byte[] hash = sha.ComputeHash(bytes);

            // 3) Turn bytes into hex string
            var sb = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
    public void CreateUser(string username, string email, string password)
    {
        var newUser = new User
        {
            email = email,
            password_hash = HashPassword(password),
            username = username,
            darkmode = 0
        };

        using (var conn = new SQLiteConnection(_dbPath))
        {
            conn.Insert(newUser);
        }

        Debug.Log($"Inserted User with ID={newUser.user_id}");
    }
    public void LogAllUsers()
    {
        using (var conn = new SQLiteConnection(_dbPath))
        {
            var users = conn.Table<User>().ToList();
            foreach (var u in users)
                Debug.Log($"[User] ID={u.user_id} Email={u.email} Username={u.username} Password={u.password_hash}");
        }
    }
    public User Login(string username, string password)
    {
        string hPasword = HashPassword(password);
        using (var conn = new SQLiteConnection(_dbPath))
        {
            var user = conn.Table<User>()
                           .FirstOrDefault(u =>
                               u.username == username &&
                               u.password_hash == hPasword
                           );
            return user;
        }
    }
    public void ClearAndResetAllData()
    {
        using (var conn = new SQLiteConnection(_dbPath))
        {
            conn.DeleteAll<User>();

            conn.Execute("DELETE FROM sqlite_sequence WHERE name='User'");

            conn.Execute("VACUUM");
        }
        Debug.Log("All data cleared and IDs reset.");
    }
}