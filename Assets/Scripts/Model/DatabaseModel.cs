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

    public void CreateNewAccount(string login,string password,string firstName,string lastName,string sex,int callNumber) {
        using SqliteConnection conn = new($"Data Source={path};Version=3;New=False;");
        conn.Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = $"SELECT Id FROM Osoby WHERE Login='{login}';";
        using(var reader = cmd.ExecuteReader()) {
            if(reader.HasRows) {
                throw new Exception("Ju¿ istnieje takie konto.");
            }
        }

        cmd.CommandText = $"INSERT INTO Osoby(Imiê,Nazwisko,Login,Has³o,P³eæ,Telefon) VALUES ('{firstName}','{lastName}','{login}','{password}','{sex}',{callNumber});";
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
            DELETE FROM Spotkania WHERE Id in (SELECT Id_s FROM Osoby_Spotkania WHERE Id_o = {accountID});
            DELETE FROM Osoby WHERE Id = {accountID};
            COMMIT TRANSACTION;
        ";
        cmd.ExecuteNonQuery();
        Logout();
    }

    private List<Entity> GetEntitiesForCurrentAccount(string entityTableName,string userTableName,string entityTableIdColumnName) {
        Trace.Assert(CurrentLogin != null);
        var accountID = GetCurrentAccountID();

        using SqliteConnection conn = new($"Data Source={path};Version=3;New=False;");
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT {entityTableIdColumnName},Priorytet FROM {userTableName} WHERE Id_o = {accountID};";
        using var reader = cmd.ExecuteReader();
        if(reader.HasRows) {
            List<Entity> entities = new();
            while(reader.Read()) {
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
        return new();
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
}
