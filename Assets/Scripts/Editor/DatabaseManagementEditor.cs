using System;
using UnityEngine;
using UnityEditor;
using Mono.Data.Sqlite;

[CustomEditor(typeof(DatabaseManagement))]
public class DatabaseManagementEditor : Editor {
    private SerializedProperty fileNameProperty;
    private string sqlCommand;
    private string sqlResponse;

    private void OnEnable() {
        fileNameProperty = serializedObject.FindProperty("fileName");
    }

    private void ExecuteComamnd() {
        using SqliteConnection conn = new($"Data Source=./Assets/StreamingAssets/{fileNameProperty.stringValue};Version=3;New=False;");
        conn.Open();

        foreach(var command in sqlCommand.Split(new[]{';','\n'})) {
            if(string.IsNullOrEmpty(command)) continue;

            using var cmd = conn.CreateCommand();
            cmd.CommandText = command + ";";
            if(command.StartsWith("select") || command.StartsWith("SELECT")) {
                using var reader = cmd.ExecuteReader();
                if(reader.HasRows) {
                    for(int i = 0;i < reader.FieldCount;i += 1) {
                        sqlResponse += reader.GetName(i);
                        if(i + 1 < reader.FieldCount) {
                            sqlResponse += "|";
                        }
                    }
                    sqlResponse += "\n";

                    while(reader.HasRows && reader.Read()) {
                        for(int i = 0;i < reader.FieldCount;i += 1) {
                            sqlResponse += reader.GetValue(i);
                            if(i + 1 < reader.FieldCount) {
                                sqlResponse += "|";
                            }
                        }
                        sqlResponse += "\n";
                    }
                    sqlResponse += "\n";
                }
                else {
                    sqlResponse += "No data returned.\n\n";
                }
            }
            else {
                int count = cmd.ExecuteNonQuery();
                sqlResponse += $"Updated {count} rows.\n\n";
            }
        }
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.PropertyField(fileNameProperty);

        EditorGUILayout.Separator();

        GUILayout.Label("SQL Command");
        sqlCommand = GUILayout.TextArea(sqlCommand);

        if(GUILayout.Button("Execute")) {
            sqlResponse = "";
            try {
                ExecuteComamnd();
            }
            catch(SqliteException error) {
                if(!string.IsNullOrEmpty(sqlResponse)) {
                    sqlResponse += "\n";
                }
                sqlResponse += $"[Error] {error.Message}";
            }
        }

        GUILayout.Label("Database response");
        GUILayout.TextArea(sqlResponse);

        serializedObject.ApplyModifiedProperties();
    }
}
