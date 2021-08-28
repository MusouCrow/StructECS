using Unity.Burst;
using Unity.Collections;
using Entity = System.Int32;

[BurstCompile]
public static class Manager {
    private static int entityMax;
    private static NativeList<int> recycles;
    private static NativeHashSet<Entity> removes;
    private static SystemBase[] systems;

    public static void Init(SystemBase[] systems) {
        Manager.systems = systems;
        recycles = new NativeList<int>(Allocator.Persistent);
        removes = new NativeHashSet<Entity>(16, Allocator.Persistent);
    }

    public static void Clean() {
        entityMax = 0;
        systems = null;
        recycles.Dispose();
        removes.Dispose();
    }

    public static void DoRemove() {
        if (removes.Count() == 0) {
            return;
        }

        foreach (var e in removes) {
            foreach (var system in systems) {
                system.DelEntity(e);
            }

            recycles.Add(e);
        }

        removes.Clear();
    }

    public static Entity NewEntity() {
        if (recycles.Length == 0) {
            return entityMax++;
        }

        int top = recycles[0];
        recycles.RemoveAt(0);

        return top;
    }

    public static void DelEntity(in Entity entity) {
        removes.Add(entity);
    }

    public static void ApplyEntity(in Entity entity) {
        foreach (var system in systems) {
            system.AddEntity(entity);
        }
    }
}