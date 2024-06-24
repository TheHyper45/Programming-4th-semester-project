using System;
using System.Text;
using Mono.Data.Sqlite;
using System.Diagnostics;
using System.Collections.Generic;

public class DatabaseModel {
    public class Entity {
        public string Name { get; private set; }
        public string Level { get; private set; }
        public int Priority { get; private set; }
        public Entity(string name,string level,int priority) {
            Name = name;
            Level = level;
            Priority = priority;
        }
    }

    public class User {
        public int Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Login { get; private set; }
        public string Password { get; private set; }
        public string Sex { get; private set; }
        public string CallNumber { get; private set; }
        public List<Entity> Languages { get; private set; }
        public List<Entity> Sports { get; private set; }
        public List<Entity> Subjects { get; private set; }
        public User(int id,string firstName,string lastName,string login,string password,string sex,string callNumber,List<Entity> languages,List<Entity> sports,List<Entity> subjects) {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Login = login;
            Password = password;
            Sex = sex;
            CallNumber = callNumber;
            Languages = languages;
            Sports = sports;
            Subjects = subjects;
        }

    };

    public readonly string path;

    public string CurrentLogin { get; private set; } = null;

    public DatabaseModel(string _path) {
        path = _path;
        DatabaseGeneration.Init(path);
    }

    public void Login(string login,string password) {
        using SqliteConnection conn = new($"Data Source={path};Version=3;New=False;");
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT Id FROM Osoby WHERE Login='{login}' AND Has³o='{password}';";
        using var reader = cmd.ExecuteReader();
        if(reader.HasRows) {
            CurrentLogin = login;
        }
        else throw new Exception("Niepoprawny login lub has³o.");
    }

    public void Logout() {
        CurrentLogin = null;
    }

    public void CreateNewAccount(string login,string password,string firstName,string lastName,string sex,int phoneNumber) {
        using SqliteConnection conn = new($"Data Source={path};Version=3;New=False;");
        conn.Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = $"SELECT Id FROM Osoby WHERE Login='{login}';";
        using(var reader = cmd.ExecuteReader()) {
            if(reader.HasRows) {
                throw new Exception("Ju¿ istnieje takie konto.");
            }
        }

        cmd.CommandText = $"INSERT INTO Osoby(Imiê,Nazwisko,Login,Has³o,P³eæ,Telefon) VALUES ('{firstName}','{lastName}','{login}','{password}','{sex}',{phoneNumber});";
        cmd.ExecuteNonQuery();
    }

    public int GetCurrentAccountID() {
        Trace.Assert(CurrentLogin != null);
        using SqliteConnection conn = new($"Data Source={path};Version=3;New=False;");
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT Id FROM Osoby WHERE Login='{CurrentLogin}';";
        using var reader = cmd.ExecuteReader();
        Trace.Assert(reader.HasRows);
        reader.Read();
        return reader.GetInt32(0);
    }

    public string GetCurrentAccountSex() {
        Trace.Assert(CurrentLogin != null);
        using SqliteConnection conn = new($"Data Source={path};Version=3;New=False;");
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT P³eæ FROM Osoby WHERE Login='{CurrentLogin}';";
        using var reader = cmd.ExecuteReader();
        Trace.Assert(reader.HasRows);
        reader.Read();
        return reader.GetString(0);
    }

    public void DeleteCurrentAccount() {
        Trace.Assert(CurrentLogin != null);
        var accountID = GetCurrentAccountID();

        using SqliteConnection conn = new($"Data Source={path};Version=3;New=False;");
        conn.Open();

        using var cmd = conn.CreateCommand();
        //@TODO: Remove apropriate entries from 'STP' and 'STP_Spotkania' tables.
        cmd.CommandText = $@"
            BEGIN TRANSACTION;
            DELETE FROM Osoby_Jêzyki WHERE Id_o = {accountID};
            DELETE FROM Osoby_Sport WHERE Id_o = {accountID};
            DELETE FROM Osoby_Przedmioty WHERE Id_o = {accountID};
            DELETE FROM Zbanowane_Osoby WHERE Id_1 = {accountID} OR Id_2 = {accountID};
            DELETE FROM Osoby_Spotkania WHERE Id_o = {accountID};
            DELETE FROM Spotkania WHERE Id not in (SELECT Id_s FROM Osoby_Spotkania);
            DELETE FROM Osoby WHERE Id = {accountID};
            COMMIT TRANSACTION;
        ";
        cmd.ExecuteNonQuery();
        Logout();
    }

    private List<Entity> GetEntities(int accountID,string entityTableName,string userTableName,string entityTableIdColumnName) {
        List<Entity> entities = new();
        using SqliteConnection conn = new($"Data Source={path};Version=3;New=False;");
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT {entityTableIdColumnName},Priorytet FROM {userTableName} WHERE Id_o = {accountID};";
        using var reader = cmd.ExecuteReader();
        while(reader.HasRows && reader.Read()) {
            //@TODO: This is confusing.
            using var cmd2 = conn.CreateCommand();
            cmd2.CommandText = $"SELECT Nazwa,Stopieñ FROM {entityTableName} WHERE Id = {reader.GetInt32(0)};";
            using var reader2 = cmd2.ExecuteReader();
            Trace.Assert(reader2.HasRows);
            reader2.Read();
            entities.Add(new(reader2.GetString(0),reader2.GetString(1),reader.GetInt32(1)));
        }
        return entities;
    }

    private List<Entity> GetEntitiesForCurrentAccount(string entityTableName,string userTableName,string entityTableIdColumnName) {
        Trace.Assert(CurrentLogin != null);
        return GetEntities(GetCurrentAccountID(),entityTableName,userTableName,entityTableIdColumnName);
    }

    public List<Entity> GetLanguagesForCurrentAccount() {
        return GetEntitiesForCurrentAccount("Jêzyki","Osoby_Jêzyki","Id_j");
    }

    public List<Entity> GetSportsForCurrentAccount() {
        return GetEntitiesForCurrentAccount("Sport","Osoby_Sport","Id_s");
    }

    public List<Entity> GetSubjectsForCurrentAccount() {
        return GetEntitiesForCurrentAccount("Przedmioty","Osoby_Przedmioty","Id_p");
    }

    private void UpdateEntitiesForCurrentAccount(List<Entity> entities,string entityTableName,string userTableName,string entityTableIdColumnName) {
        Trace.Assert(CurrentLogin != null);
        var accountID = GetCurrentAccountID();

        using SqliteConnection conn = new($"Data Source={path};Version=3;New=False;");
        conn.Open();

        StringBuilder builder = new();
        builder.Append("BEGIN TRANSACTION;");
        builder.Append($"DELETE FROM {userTableName} WHERE Id_o = {accountID};");
        foreach(var entity in entities) {
            builder.Append($@"INSERT INTO {userTableName}(Id_o,{entityTableIdColumnName},Priorytet) VALUES (
                {accountID},
                (SELECT Id FROM {entityTableName} WHERE Nazwa = '{entity.Name}' AND Stopieñ = '{entity.Level}'),
                {entity.Priority}
            );");
        }
        builder.Append("COMMIT TRANSACTION;");

        using var cmd = conn.CreateCommand();
        cmd.CommandText = builder.ToString();
        cmd.ExecuteNonQuery();
    }

    public void UpdateLanguagesForCurrentAccount(List<Entity> languages) {
        UpdateEntitiesForCurrentAccount(languages,"Jêzyki","Osoby_Jêzyki","Id_j");
    }

    public void UpdateSportsForCurrentAccount(List<Entity> sports) {
        UpdateEntitiesForCurrentAccount(sports,"Sport","Osoby_Sport","Id_s");
    }

    public void UpdateSubjectsForCurrentAccount(List<Entity> subjects) {
        UpdateEntitiesForCurrentAccount(subjects,"Przedmioty","Osoby_Przedmioty","Id_p");
    }

    public List<User> GetMatchingUsersForCurrentAccount() {
        Trace.Assert(CurrentLogin != null);
        var currentAccountID = GetCurrentAccountID();
        var currentLanguages = GetLanguagesForCurrentAccount();
        var currentSports = GetSportsForCurrentAccount();
        var currentSubjects = GetSubjectsForCurrentAccount();

        Random random = new();
        List<User> users = new();
        Dictionary<User,int> userEvaluations = new();
        using SqliteConnection conn = new($"Data Source={path};Version=3;New=False;");
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT Id,Imiê,Nazwisko,Login,Has³o,P³eæ,Telefon FROM Osoby WHERE Id <> {currentAccountID};";
        using var reader = cmd.ExecuteReader();
        while(reader.HasRows && reader.Read()) {
            User user = new(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetString(5),
                reader.GetString(6),
                GetEntities(reader.GetInt32(0),"Jêzyki","Osoby_Jêzyki","Id_j"),
                GetEntities(reader.GetInt32(0),"Sport","Osoby_Sport","Id_s"),
                GetEntities(reader.GetInt32(0),"Przedmioty","Osoby_Przedmioty","Id_p")
            );

            int evaluation = 0;
            foreach(var currentLanguage in currentLanguages) {
                foreach(var userLanguage in user.Languages) {
                    if(userLanguage.Name.Equals(currentLanguage.Name)) {
                        evaluation += currentLanguage.Priority * DatabaseGeneration.GetLanguageLevelWeight(userLanguage.Level) + random.Next(-5,6);
                    }
                }
            }
            foreach(var currentSport in currentSports) {
                foreach(var userSport in user.Sports) {
                    if(userSport.Name.Equals(currentSport.Name)) {
                        evaluation += currentSport.Priority * DatabaseGeneration.GetSportLevelWeight(userSport.Level) + random.Next(-5,6);
                    }
                }
            }
            foreach(var currentSubject in currentSubjects) {
                foreach(var userSubject in user.Subjects) {
                    if(userSubject.Name.Equals(currentSubject.Name)) {
                        evaluation += currentSubject.Priority * DatabaseGeneration.GetSubjectLevelWeight(userSubject.Level) + random.Next(-5,6);
                    }
                }
            }
            userEvaluations.Add(user,evaluation);
            users.Add(user);
        }
        users.Sort((User a,User b) => {
            if(a == null && b != null) {
                return 1;
            }
            if(a != null && b == null) {
                return -1;
            }
            return (a == b) ? 0 : (userEvaluations[a] >= userEvaluations[b] ? -1 : 1);
        });
        if(users.Count > 100) {
            users.RemoveRange(100,users.Count - 100);
        }
        return users;
    }
}
