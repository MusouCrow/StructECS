using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Entity = System.Int32;

[BurstCompile]
public static class Database {
	public static Dictionary<Entity, MeshCom> MeshCom;
	public static NativeHashMap<Entity, TestCom> TestCom;
	public static Dictionary<Entity, TransformCom> TransformCom;

    public static void Init() {
		MeshCom = new Dictionary<Entity, MeshCom>();
		TestCom = new NativeHashMap<Entity, TestCom>(16, Allocator.Persistent);
		TransformCom = new Dictionary<Entity, TransformCom>();
    }

    public static void Clean() {
		TestCom.Dispose();
    }
}