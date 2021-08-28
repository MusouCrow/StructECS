using Unity.Burst;
using Unity.Collections;
using Entity = System.Int32;

[BurstCompile]
public static class Database {
    public static NativeHashMap<Entity, TestComponent> TestComponentMap;

    public static void Init() {
        TestComponentMap = new NativeHashMap<Entity, TestComponent>(50, Allocator.Persistent);
    }

    public static void Clean() {
        TestComponentMap.Dispose();
    }
}