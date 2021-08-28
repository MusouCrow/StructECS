using UnityEngine;

public struct TransformCom : IComponent {
    public Transform transform;
}

public class TransformBhr : MonoBehaviour, IConverted {
    protected void Start() {
        this.Convert(this.gameObject, true);
        Destroy(this);
    }

    public void Convert(GameObject gameObject, bool self) {
        var entity = Manager.GetEntityWithGameObject(gameObject);
        Database.TransformCom[entity] = new TransformCom() {
            transform = gameObject.transform
        };

        if (!self) {
            gameObject.transform.localPosition = this.transform.localPosition;
            gameObject.transform.localScale = this.transform.localScale;
            gameObject.transform.localRotation = this.transform.localRotation;
        }
    }
}