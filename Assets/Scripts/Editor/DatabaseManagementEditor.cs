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

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.PropertyField(fileNameProperty);

        EditorGUILayout.Separator();

        GUILayout.Label("Sql Command");
        sqlCommand = GUILayout.TextArea(sqlCommand);

        if(GUILayout.Button("Execute")) {
            sqlResponse = "";
            using SqliteConnection conn = new($"Data Source=./Assets/StreamingAssets/{fileNameProperty.stringValue};Version=3;New=False;");
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sqlCommand;

            if(sqlCommand.StartsWith("SELECT") || sqlCommand.StartsWith("select")) {
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
                }
            }
            else {
                int count = cmd.ExecuteNonQuery();
                sqlResponse = $"Updated {count} rows.";
            }
        }

        GUILayout.Label("Response");
        GUILayout.TextArea(sqlResponse);

        serializedObject.ApplyModifiedProperties();
    }
}
