using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class DatabaseGeneration {
    private const string FILE_CONTENT = @"using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Entity = System.Int32;

[BurstCompile]
public static class Database {
// Define Data

    public static void Init() {
// Create Data
    }

    public static void Clean() {
// Dispose Data
    }
}";

    private const string DEFINE_NATIVE = "public static NativeHashMap<Entity, Component> Component;";
    private const string DEFINE_MANAGED = "public static Dictionary<Entity, Component> Component;";
    private const string CREATE_NATIVE = "Component = new NativeHashMap<Entity, Component>(16, Allocator.Persistent);";
    private const string CREATE_MANAGED = "Component = new Dictionary<Entity, Component>();";
    private const string DISPOSE = "Component.Dispose();";

    [MenuItem("Tools/Flush ECS Database")]
    public static void Generate() {
        var types = CollectComponents();
        var define = DefineComponents(types);
        var create = CreateComponents(types);
        var dispose = DisposeComponents(types);
        var code = ReplaceCode(define, create, dispose);
        var path = FindFile("Database.cs");

        File.WriteAllText(path, code);
        Debug.Log("Write Database to " + path);

        EditorUtility.RequestScriptReload();
    }

    private static string FindFile(string name) {
        var files = Directory.GetFiles("Assets", name, SearchOption.AllDirectories);

        return files.Length > 0 ? files[0] : "Assets/" + name;
    }

    private static List<Type> CollectComponents() {
        var types = new List<Type>();

        var type = typeof(ComponentAttribute);
        var asm = type.Assembly;
        var allTypes = asm.GetExportedTypes();

        foreach (var t in allTypes) {
            if (t.GetCustomAttribute<ComponentAttribute>() != null) {
                types.Add(t);
            }
        }

        return types;
    }

    private static string DefineComponents(List<Type> types) {
        var sb = new StringBuilder();

        for (int i = 0; i < types.Count; i++) {
            var t = types[i];
            var attr = t.GetCustomAttribute<ComponentAttribute>();
            var line = attr.managed ? DEFINE_MANAGED : DEFINE_NATIVE;
            line = line.Replace("Component", t.Name);

            sb.Append("\t" + line);

            if (i < types.Count - 1) {
                sb.Append("\n");
            }
        }

        return sb.ToString();
    }

    private static string CreateComponents(List<Type> types) {
        var sb = new StringBuilder();

        for (int i = 0; i < types.Count; i++) {
            var t = types[i];
            var attr = t.GetCustomAttribute<ComponentAttribute>();
            var line = attr.managed ? CREATE_MANAGED : CREATE_NATIVE;
            line = line.Replace("Component", t.Name);

            sb.Append("\t\t" + line);

            if (i < types.Count - 1) {
                sb.Append("\n");
            }
        }

        return sb.ToString();
    }

    private static string DisposeComponents(List<Type> types) {
        var sb = new StringBuilder();

        for (int i = 0; i < types.Count; i++) {
            var t = types[i];
            var attr = t.GetCustomAttribute<ComponentAttribute>();

            if (!attr.managed) {
                var line = DISPOSE.Replace("Component", t.Name);
                
                if (sb.Length > 0) {
                    sb.Append("\n");
                }

                sb.Append("\t\t" + line);
            }
        }

        return sb.ToString();
    }

    private static string ReplaceCode(string define, string create, string dispose) {
        var code = FILE_CONTENT;
        code = code.Replace("// Define Data", define);
        code = code.Replace("// Create Data", create);
        code = code.Replace("// Dispose Data", dispose);
        
        return code;
    }
}