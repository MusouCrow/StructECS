using Unity.Burst;
using Unity.Collections;
using Entity = System.Int32;

[BurstCompile]
public static class Database {
    public static NativeHashMap<Entity, TestComponent> TestComponent;
    public static NativeHashMap<Entity, FuckComponent> FuckComponent;

    public static void Init() {
        TestComponent = new NativeHashMap<Entity, TestComponent>(16, Allocator.Persistent);
        FuckComponent = new NativeHashMap<Entity, FuckComponent>(16, Allocator.Persistent);
    }

    public static void Clean() {
        TestComponent.Dispose();
        FuckComponent.Dispose();
    }
}