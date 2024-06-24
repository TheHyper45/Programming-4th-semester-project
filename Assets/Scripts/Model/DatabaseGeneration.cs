using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class DatabaseGeneration {
    public static readonly string[] Languages =
        {"Polski","Angielski","Niemiecki","Francuski","Hiszpañski","Portugalski","W³oski","Rosyjski","Ukraiñski","Czeski"};
    public static readonly string[] LanguageLevels =
        {"brak","A1","A2","B1","B2","C1","C2"};
    public static readonly string[] Sports =
        {"Pi³ka no¿na","Pi³ka rêczna","Siatkówka","Koszykówka","Hokej","Tenis","Tenis sto³owy","Golf","Bieganie"};
    public static readonly string[] SportLevels =
        {"brak","pocz¹tkuj¹cy","œredniozaawansowany","zaawansowany"};
    public static readonly string[] Subjects =
        {"Matematyka","Fizyka","Chemia","Biologia","Geografia","Edukacja seksualna","Programowanie","Polityka"};
    public static readonly string[] SubjectLevels =
        {"brak","pocz¹tkuj¹cy","œredniozaawansowany","zaawansowany"};

    private static readonly string[] FirstNamesMale =
        {"Dawid","Mateusz","Dominik","Adam","Pawe³","Andrzej","Kacper","Bartek","Wojtek","Micha³","Waldemar","Mariusz"};
    private static readonly string[] LastNamesMale =
        {"Nowak","Kowalski","Wiœniewski","Wójcik","Duda","Kowalczyk","Szymañski","WoŸniak","Kamiñski","Krawczyk"};
    private static readonly string[] FirstNamesFemale =
        {"Asia","Agnieszka","Agata","Karolina","Barbara","Laura","Aleksandra","Patrycja","Edyta","Basia","Monika","Katarzyna"};
    private static readonly string[] LastNamesFemale =
        {"Nowak","Kowalska","Wiœniewska","Kaczmarek","Duda","Kowalczyk","Szymañska","WoŸniak","Kamiñska","Mazur"};

    public static void Init(string path) {
        if(!File.Exists(path)) {
            SqliteConnection.CreateFile(path);
            using SqliteConnection conn = new($"Data Source={path};Version=3;New=False;");
            conn.Open();
            using var cmd = conn.CreateCommand();

            StringBuilder builder = new();
            builder.Append(@"CREATE TABLE Osoby(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Imiê TEXT NOT NULL,
                Nazwisko TEXT NOT NULL,
                Login TEXT UNIQUE NOT NULL,
                Has³o TEXT NOT NULL,
                P³eæ TEXT CHECK(P³eæ in ('Mê¿czyzna','Kobieta')),
                Telefon VARCHAR(12) NOT NULL
            );");
            builder.Append(@"CREATE TABLE Jêzyki(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nazwa TEXT NOT NULL,
                Stopieñ TEXT NOT NULL
            );");
            builder.Append(@"CREATE TABLE Osoby_Jêzyki(
                Id_o INTEGER NOT NULL,
                Id_j INTEGER NOT NULL,
                Priorytet INTEGER NOT NULL,
                FOREIGN KEY(Id_o) REFERENCES Osoby(Id),
                FOREIGN KEY(Id_j) REFERENCES Jêzyki(Id)
            );");
            builder.Append(@"CREATE TABLE Sport(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nazwa TEXT NOT NULL,
                Stopieñ TEXT NOT NULL
            );");
            builder.Append(@"CREATE TABLE Osoby_Sport(
                Id_o INTEGER NOT NULL,
                Id_s INTEGER NOT NULL,
                Priorytet INTEGER NOT NULL,
                FOREIGN KEY(Id_o) REFERENCES Osoby(Id),
                FOREIGN KEY(Id_s) REFERENCES Sport(Id)
            );");
            builder.Append(@"CREATE TABLE Przedmioty(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nazwa TEXT NOT NULL,
                Stopieñ TEXT NOT NULL
            );");
            builder.Append(@"CREATE TABLE Osoby_Przedmioty(
                Id_o INTEGER NOT NULL,
                Id_p INTEGER NOT NULL,
                Priorytet INTEGER NOT NULL,
                FOREIGN KEY(Id_o) REFERENCES Osoby(Id),
                FOREIGN KEY(Id_p) REFERENCES Przedmioty(Id)
            );");
            builder.Append(@"CREATE TABLE Zbanowane_Osoby(
                Id_1 INTEGER NOT NULL,
                Id_2 INTEGER NOT NULL,
                FOREIGN KEY(Id_1) REFERENCES Osoby(Id),
                FOREIGN KEY(Id_2) REFERENCES Osoby(Id)
            );");
            builder.Append(@"CREATE TABLE Spotkania(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Status TEXT NOT NULL,
                Data DATE NOT NULL
            );");
            builder.Append(@"CREATE TABLE Osoby_Spotkania(
                Id_o INTEGER NOT NULL,
                Id_s INTEGER NOT NULL,
                Ocena INTEGER NOT NULL,
                Chêtny BOOLEAN NOT NULL,
                FOREIGN KEY(Id_o) REFERENCES Osoby(Id),
                FOREIGN KEY(Id_s) REFERENCES Spotkania(Id)
            );");
            builder.Append(@"CREATE TABLE STP(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Id_o INTEGER NOT NULL,
                Stopieñ TEXT NOT NULL,
                Priorytet INTEGER NOT NULL,
                Temat_Grupa TEXT NOT NULL,
                Temat TEXT NOT NULL,
                FOREIGN KEY(Id_o) REFERENCES Osoby(Id)
            );");
            builder.Append(@"CREATE TABLE STP_Spotkania(
                Id_s INTEGER NOT NULL,
                Id_st INTEGER NOT NULL,
                FOREIGN KEY(Id_s) REFERENCES Spotkania(Id),
                FOREIGN KEY(Id_st) REFERENCES STP(Id)
            );");
            cmd.CommandText = builder.ToString();
            cmd.ExecuteNonQuery();

            builder.Clear();
            foreach(var language in Languages) {
                foreach(var languageLevel in LanguageLevels) {
                    builder.Append($"INSERT INTO Jêzyki(Nazwa,Stopieñ) VALUES ('{language}','{languageLevel}');");
                }
            }
            foreach(var sport in Sports) {
                foreach(var sportLevel in SportLevels) {
                    builder.Append($"INSERT INTO Sport(Nazwa,Stopieñ) VALUES ('{sport}','{sportLevel}');");
                }
            }
            foreach(var subject in Subjects) {
                foreach(var subjectLevel in SubjectLevels) {
                    builder.Append($"INSERT INTO Przedmioty(Nazwa,Stopieñ) VALUES ('{subject}','{subjectLevel}');");
                }
            }
            cmd.CommandText = builder.ToString();
            cmd.ExecuteNonQuery();

            Random random = new();
            builder.Clear();
            GenerateUsers(builder,random,FirstNamesMale,LastNamesMale,"Mê¿czyzna");
            GenerateUsers(builder,random,FirstNamesFemale,LastNamesFemale,"Kobieta");
            cmd.CommandText = builder.ToString();
            cmd.ExecuteNonQuery();

            var userCount = FirstNamesMale.Length * LastNamesMale.Length + FirstNamesFemale.Length * LastNamesFemale.Length;
            builder.Clear();
            GenerateEntities(builder,random,userCount,Languages,LanguageLevels,"Jêzyki","Osoby_Jêzyki","Id_j");
            GenerateEntities(builder,random,userCount,Sports,SportLevels,"Sport","Osoby_Sport","Id_s");
            GenerateEntities(builder,random,userCount,Subjects,SubjectLevels,"Przedmioty","Osoby_Przedmioty","Id_p");
            cmd.CommandText = builder.ToString();
            cmd.ExecuteNonQuery();
        }
    }

    private static readonly string Digits = "0123456789";

    private static string GeneratePhoneNumber(Random random) {
        StringBuilder builder = new();
        for(int i = 0;i < 12;i += 1) {
            builder.Append(Digits[random.Next(0,9)]);
        }
        return builder.ToString();
    }

    private static void GenerateUsers(StringBuilder builder,Random random,string[] firstNames,string[] lastNames,string sex) {
        int counter = 0;
        foreach(var firstName in firstNames) {
            foreach(var lastName in lastNames) {
                builder.Append($@"INSERT INTO Osoby(Imiê,Nazwisko,Login,Has³o,P³eæ,Telefon) VALUES (
                    '{firstName}',
                    '{lastName}',
                    '{Char.ToLower(firstName[0])}{lastName.ToLower()}{counter}@example.com',
                    '{firstName.ToLower()}{lastName.ToLower()}',
                    '{sex}',
                    '{GeneratePhoneNumber(random)}'
                );");
                counter += 1;
            }
        }
    }

    private static void GenerateEntities(StringBuilder builder,Random random,int userCount,string[] entityNames,string[] entityLevels,string entityTableName,string userEntityTableName,string userEntityTableColumnIndex) {
        Trace.Assert(entityNames.Length >= 4);
        for(int userIndex = 0;userIndex < userCount;userIndex += 1) {
            int entityCount = random.Next(1,5);

            Dictionary<string,bool> entityIds = new();
            for(int entityIndex = 0;entityIndex < entityCount;entityIndex += 1) {
                for(int tmpIndex = 0;tmpIndex < 10;tmpIndex += 1) {
                    var str = entityNames[random.Next(0,entityNames.Length)];
                    if(!entityIds.ContainsKey(str)) {
                        entityIds[str] = true;
                        break;
                    }
                }
            }

            foreach(var entityName in entityIds.Keys) {
                string level = entityLevels[random.Next(0,entityLevels.Length)];
                builder.Append($@"INSERT INTO {userEntityTableName}(Id_o,{userEntityTableColumnIndex},Priorytet) VALUES (
                    {userIndex},
                    (SELECT Id FROM {entityTableName} WHERE Nazwa='{entityName}' AND Stopieñ='{level}'),
                    {random.Next(0,11)}
                );");
            }
        }
    }
}