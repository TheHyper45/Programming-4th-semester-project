using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Mono.Data.Sqlite;

public class DatabaseGeneration {
    public static readonly string[] Languages =
        {"polski","angielski","niemiecki","francuski","hiszpañski","portugalski","w³oski","rosyjski","ukraiñski","czeski"};
    public static readonly string[] LanguageLevels =
        {"brak","A1","A2","B1","B2","C1","C2"};
    public static readonly string[] Sports =
        {"pi³ka no¿na","pi³ka rêczna","siatkówka","koszykówka","hokej","tenis","tenis sto³owy","golf","bieganie"};
    public static readonly string[] SportLevels =
        {"brak","pocz¹tkuj¹cy","œredniozaawansowany","zaawansowany"};
    public static readonly string[] Subjects =
        {"matematyka","fizyka","chemia","biologia","geografia","edukacja seksualna","programowanie","polityka"};
    public static readonly string[] SubjectLevels =
        {"brak","pocz¹tkuj¹cy","œredniozaawansowany","zaawansowany"};

    private class UserEntity {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Sex { get; set; }
        public string CallNumber { get; set; }
    };

    private static readonly string[] FirstNamesMale =
        {"Dawid","Mateusz","Dominik","Adam","Pawe³","Andrzej","Kacper","Bartek","Wojtek","Micha³","Waldemar"};
    private static readonly string[] LastNamesMale =
        {"Nowak","Kowalski","Wiœniewski","Wójcik","Duda","Kowalczyk","Szymañski","WoŸniak","Kamiñski","Krawczyk"};
    private static readonly string[] FirstNamesFemale =
        {"Asia","Agnieszka","Agata","Karolina","Barbara","Laura","Aleksandra","Patrycja","Edyta","Basia","Monika"};
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
                Priorytet INT NOT NULL,
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
                Priorytet INT NOT NULL,
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
                Priorytet INT NOT NULL,
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
                Ocena INT NOT NULL,
                Chêtny BOOLEAN NOT NULL,
                FOREIGN KEY(Id_o) REFERENCES Osoby(Id),
                FOREIGN KEY(Id_s) REFERENCES Spotkania(Id)
            );");
            builder.Append(@"CREATE TABLE STP(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Id_o INTEGER NOT NULL,
                Stopieñ TEXT NOT NULL,
                Priorytet INT NOT NULL,
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
            cmd.CommandText = builder.ToString();
            cmd.ExecuteNonQuery();

            builder.Clear();
            foreach(var sport in Sports) {
                foreach(var sportLevel in SportLevels) {
                    builder.Append($"INSERT INTO Sport(Nazwa,Stopieñ) VALUES ('{sport}','{sportLevel}');");
                }
            }
            cmd.CommandText = builder.ToString();
            cmd.ExecuteNonQuery();

            builder.Clear();
            foreach(var subject in Subjects) {
                foreach(var subjectLevel in SubjectLevels) {
                    builder.Append($"INSERT INTO Przedmioty(Nazwa,Stopieñ) VALUES ('{subject}','{subjectLevel}');");
                }
            }
            cmd.CommandText = builder.ToString();
            cmd.ExecuteNonQuery();

            Random random = new();
            builder.Clear();
            foreach(var firstName in FirstNamesMale) {
                foreach(var lastName in LastNamesMale) {
                    builder.Append($@"INSERT INTO Osoby(Imiê,Nazwisko,Login,Has³o,P³eæ,Telefon) VALUES (
                        '{firstName}',
                        '{lastName}',
                        '{firstName[0]}{lastName}@example.com',
                        '{firstName}{lastName}',
                        'Mê¿czyzna',
                        '{GenerateCallNumber(random)}'
                    );");
                }
            }
            cmd.CommandText = builder.ToString();
            cmd.ExecuteNonQuery();

            builder.Clear();
            foreach(var firstName in FirstNamesFemale) {
                foreach(var lastName in LastNamesFemale) {
                    builder.Append($@"INSERT INTO Osoby(Imiê,Nazwisko,Login,Has³o,P³eæ,Telefon) VALUES (
                        '{firstName}',
                        '{lastName}',
                        '{firstName[0]}{lastName}@example.com',
                        '{firstName}{lastName}',
                        'Kobieta',
                        '{GenerateCallNumber(random)}'
                    );");
                }
            }
            cmd.CommandText = builder.ToString();
            cmd.ExecuteNonQuery();
        }
    }

    private static readonly string Digits = "0123456789";

    private static string GenerateCallNumber(Random random) {
        StringBuilder builder = new();
        for(int i = 0;i < 12;i += 1) {
            builder.Append(Digits[random.Next(0,9)]);
        }
        return builder.ToString();
    }
}
