using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Entity = System.Int32;

[BurstCompile]
public static class Manager {
    public struct InstantiatedTask {
        public GameObject prefab;
        public IConverted behaviour;
        public bool directly;
        public InstantiatedHandle handle;
    }

    public class InstantiatedHandle {
        public GameObject gameObject;
        public Entity entity;
        public bool completed;
    }

    private const int TASK_FRAME_COUNT = 30;

    private static int entityMax;
    private static NativeList<int> recycles;
    private static NativeHashSet<Entity> removes;
    private static Dictionary<GameObject, Entity> gameObject2Entity;
    private static Dictionary<Entity, GameObject> entity2GameObject;

    private static List<InstantiatedTask> instantiatedTasks;
    private static List<GameObject> instantingGameObjects;

    public static List<SystemBase> systems;

    public static void Init() {
        InitSystems();

        recycles = new NativeList<int>(Allocator.Persistent);
        removes = new NativeHashSet<Entity>(16, Allocator.Persistent);
        gameObject2Entity = new Dictionary<GameObject, Entity>();
        entity2GameObject = new Dictionary<Entity, GameObject>();
        instantiatedTasks = new List<InstantiatedTask>();
        instantingGameObjects = new List<GameObject>();
    }

    public static void Clean() {
        entityMax = 0;
        recycles.Dispose();
        removes.Dispose();

        foreach (var system in systems) {
            system.Dispose();
        }
    }

    public static void DoRemove() {
        if (removes.Count() == 0) {
            return;
        }

        foreach (var e in removes) {
            foreach (var system in systems) {
                system.DelEntity(e);
            }

            if (entity2GameObject.ContainsKey(e)) {
                var go = entity2GameObject[e];
                gameObject2Entity.Remove(go);
                entity2GameObject.Remove(e);
                GameObject.Destroy(go);
            }

            recycles.Add(e);
        }

        removes.Clear();
    }

    public static void DoTask() {
        if (instantiatedTasks.Count == 0) {
            return;
        }

        int count = instantiatedTasks.Count > TASK_FRAME_COUNT ? TASK_FRAME_COUNT : instantiatedTasks.Count;

        for (int i = 0; i < count; i++) {
            ApplyInstantiatedTasks(instantiatedTasks[i]);
        }

        if (instantiatedTasks.Count <= TASK_FRAME_COUNT) {
            instantiatedTasks.Clear();
        }
        else {
            instantiatedTasks.RemoveRange(0, TASK_FRAME_COUNT);
        }
    }

    public static Entity NewEntity() {
        int id = 0;
        
        if (recycles.Length == 0) {
            id = entityMax++;
        }
        else {
            id = recycles[0];
            recycles.RemoveAt(0);
        }

        return id;
    }

    public static void DelEntity(in Entity entity) {
        removes.Add(entity);
    }

    public static void ApplyEntity(in Entity entity) {
        foreach (var system in systems) {
            system.AddEntity(entity);
        }
    }

    public static Entity GetEntityWithGameObject(GameObject gameObject) {
        if (!gameObject2Entity.ContainsKey(gameObject)) {
            var entity = NewEntity();
            gameObject2Entity[gameObject] = entity;
            entity2GameObject[entity] = gameObject;
        }
        
        return gameObject2Entity[gameObject];
    }

    public static GameObject Instantiate(GameObject prefab, GameObject parent=null) {
        var converts = prefab.GetComponents<IConverted>();

        if (converts.Length == 0) {
            return InstantiateGameObject(prefab, parent);
        }

        var go = NewGameObject(prefab, parent);
        
        foreach (var c in converts) {
            ConvertComponent(go, c);
        }

        for (int i = 0; i < prefab.transform.childCount; i++) {
            Instantiate(prefab.transform.GetChild(i).gameObject, go);
        }

        // 由根节点统一驱动
        if (!parent) {
            ApplyEntityWithGameObject(go);
        }

        return go;
    }

    public static InstantiatedHandle InstantiateAsync(GameObject prefab) {
        var converts = prefab.GetComponents<IConverted>();
        var handle = new InstantiatedHandle() {entity = -1};

        if (converts.Length == 0) {
            // InstantiateGameObject(prefab, parent);
            instantiatedTasks.Add(new InstantiatedTask() {
                prefab = prefab,
                directly = true,
                handle = handle
            });

            return handle;
        }

        // var go = NewGameObject(prefab, parent);
        instantiatedTasks.Add(new InstantiatedTask() {prefab = prefab});
        
        foreach (var c in converts) {
            // ConvertComponent(go, c);
            instantiatedTasks.Add(new InstantiatedTask() {behaviour = c});
        }

        for (int i = 0; i < prefab.transform.childCount; i++) {
            InstantiateAsync(prefab.transform.GetChild(i).gameObject);
        }

        // ApplyEntityWithGameObject(go, parent);
        instantiatedTasks.Add(new InstantiatedTask() {handle = handle});

        return handle;
    }

    private static void InitSystems() {
        systems = new List<SystemBase>();

        var type = typeof(SystemAttribute);
        var asm = type.Assembly;
        var types = asm.GetExportedTypes();

        foreach (var t in types) {
            var attr = t.GetCustomAttribute<SystemAttribute>();

            if (attr != null) {
                var system = asm.CreateInstance(t.Name) as SystemBase;
                system.order = attr.order;
                systems.Add(system);
            }
        }

        systems.Sort((SystemBase a, SystemBase b) => {
            if (a.order == b.order) {
                return 0;
            }

            return a.order > b.order ? -1 : 1;
        });
    }

    private static void ApplyInstantiatedTasks(in InstantiatedTask task) {
        var parent = instantingGameObjects.Count > 0 ? instantingGameObjects[instantingGameObjects.Count - 1] : null;

        if (task.prefab && task.directly) {
            var go = InstantiateGameObject(task.prefab, parent);
            task.handle.gameObject = go;
            task.handle.completed = true;
            Debug.Log("InstantiateGameObject, " + task.prefab + ", " + parent);
        }
        else if (task.prefab) {
            var go = NewGameObject(task.prefab, parent);
            instantingGameObjects.Add(go);
            Debug.Log("NewGameObject, " + task.prefab + ", " + parent + ", " + go);
        }
        else if (task.behaviour != null) {
            ConvertComponent(parent, task.behaviour);
            Debug.Log("ConvertComponent, " + parent + ", " + task.behaviour);
        }
        else {
            instantingGameObjects.RemoveAt(instantingGameObjects.Count - 1);

            if (instantingGameObjects.Count == 0) {
                ApplyEntityWithGameObject(parent);
                task.handle.gameObject = parent;
                task.handle.entity = gameObject2Entity[parent];
                task.handle.completed = true;
                Debug.Log("ApplyEntityWithGameObject, " + parent);
            }
        }
    }

    private static GameObject NewGameObject(GameObject prefab, GameObject parent) {
        var go = new GameObject(prefab.name);
        go.layer = prefab.layer;
        go.tag = prefab.tag;

        if (parent) {
            go.transform.SetParent(parent.transform);
        }

        return go;
    }

    private static GameObject InstantiateGameObject(GameObject prefab, GameObject parent) {
        var go = GameObject.Instantiate(prefab);
        
        if (parent) {
            go.transform.SetParent(parent.transform);
        }

        return go;
    }

    private static void ConvertComponent(GameObject gameObject, IConverted behaviour) {
        behaviour.Convert(gameObject, false);
    }

    private static void ApplyEntityWithGameObject(GameObject gameObject) {
        if (!gameObject2Entity.ContainsKey(gameObject)) {
            return;
        }

        var entity = gameObject2Entity[gameObject];
        ApplyEntity(entity);

        for (int i = 0; i < gameObject.transform.childCount; i++) {
            var t = gameObject.transform.GetChild(i);
            ApplyEntityWithGameObject(t.gameObject);
        }
    }
}