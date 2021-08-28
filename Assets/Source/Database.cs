using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Entity = System.Int32;

[BurstCompile]
public static class Database {
    public static NativeHashMap<Entity, TestCom> TestCom;
    public static Dictionary<Entity, TransformCom> TransformCom;
    public static Dictionary<Entity, MeshCom> MeshCom;

    public static void Init() {
        TestCom = new NativeHashMap<Entity, TestCom>(16, Allocator.Persistent);
        TransformCom = new Dictionary<Entity, TransformCom>();
        MeshCom = new Dictionary<Entity, MeshCom>();
    }

    public static void Clean() {
        TestCom.Dispose();
    }
}