using Unity.Burst;
using Unity.Collections;
using Entity = System.Int32;

[BurstCompile]
public static class Manager {
    private static int entityMax;
    private static NativeList<int> recycles;

    public static void Init() {
        recycles = new NativeList<int>(Allocator.Persistent);
    }

    public static void Clean() {
        entityMax = 0;
        recycles.Dispose();
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
        recycles.Add(entity);
    }

    public static void ApplyEntity(in Entity entity) {

    }
}