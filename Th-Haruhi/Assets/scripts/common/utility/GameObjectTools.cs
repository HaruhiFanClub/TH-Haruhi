
using UnityEngine;

public static class GameObjectTools
{
    public static GameObject CreateGameObject()
    {
        return new GameObject();
    }

    public static GameObject CreateGameObject(string name, params System.Type[] components)
    {
        return new GameObject(name, components);
    }

    public static void DestroyAllChild(GameObject obj, string dontDestroyName = null)
    {
        foreach (Transform child in obj.transform)
        {
            if (!string.IsNullOrEmpty(dontDestroyName) && child.gameObject.name.Contains(dontDestroyName))
            {
                continue;
            }
            Object.Destroy(child.gameObject);
        }

        obj.transform.DetachChildren();
    }

    public static void DestroyGameObject(GameObject gameObject, float time = 0f)
    {
        GameObject.Destroy(gameObject, time);
    }

    public static void DontDestroyOnSceneChanged(GameObject gameObject)
    {
        GameObject.DontDestroyOnLoad(gameObject);
    }

    public static Component AddComponent(GameObject target, System.Type type)
    {
        return target.AddComponent(type);
    }

    public static T Find<T>(string path) where T : Component
    {
        T targetT = null;
        GameObject gameObject = GameObject.Find(path);
        if (gameObject)
            targetT = gameObject.GetComponent<T>();

        return targetT;
    }

    public static Object Find(System.Type type)
    {
        return Object.FindObjectOfType(type);
    }

    public static T Find<T>() where T : Object
    {
        T target = Object.FindObjectOfType<T>();
        return target;
    }

    public static GameObject Find(GameObject target, string path)
    {
        GameObject targetGo = null;
        if (target)
        {
            var t = target.transform.Find(path);
            if (t != null)
                targetGo = t.gameObject;
        }

        return targetGo;
    }

    public static Transform Find(Transform target, string path)
    {
        Transform targetGo = null;
        if (target)
            if (target = target.Find(path))
                targetGo = target;

        return targetGo;
    }

    public static void Bind(this Transform t, Transform parent)
    {
        t.SetParent(parent, false);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
    }

    public static T Find<T>(GameObject gameObj, string path) where T : Component
    {
        return Find<T>(gameObj.transform, path);
    }

    public static T Find<T>(Transform transform, string path) where T : Component
    {
        T targetT = null;
        if (transform)
            if (transform = transform.Find(path))
            {
                // to avoid null reference | add by liao
                targetT = transform.GetComponent<T>();
                if (targetT == null)
                {
                    targetT = transform.gameObject.AddComponent<T>();
                }
            }

        return targetT;
    }
    public static Transform Lookup(this GameObject g, string name)
    {
        return Lookup(g.transform, name);
    }
    public static Transform Lookup(this Transform transform, string name)
    {
        if (transform)
            foreach (Transform t in transform)
            {
                if (t.name == name)
                    return t;
                var finded = Lookup(t, name);
                if (finded)
                    return finded;
            }
        return null;
    }

    public static T Lookup<T>(Transform transform, string name) where T : Component
    {
        if (transform)
            foreach (Transform t in transform)
            {
                if (t.name == name)
                    return t.GetComponent<T>();
                T finded = Lookup<T>(t, name);
                if (finded)
                    return finded;
            }
        return null;
    }


    public static void ResetTransform(Transform transform)
    {
        if (transform)
        {
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    public static void Bind(Transform parentTransform, Transform childTransform, bool resetChildTransform = true)
    {
        if (parentTransform)
            childTransform.SetParent(parentTransform);
        else
            childTransform.parent = null;
        if (resetChildTransform)
            ResetTransform(childTransform);
    }

    public static void Bind(Transform parent, string targetPath, Transform child, bool resetChildTransform = true)
    {
        Transform target;
        if (string.IsNullOrEmpty(targetPath))
            target = parent;
        else if ((target = Find(parent, targetPath)) == null)
            target = parent;
        Bind(target, child, resetChildTransform);
    }

    public static void UnBind(Transform childTransform, bool resetTransform = true)
    {
        if (childTransform)
        {
            childTransform.SetParent(null);
            if (resetTransform)
                ResetTransform(childTransform);
        }
    }

    static public void SetChildLayer(this Transform t, int layer)
    {
        for (int i = 0; i < t.childCount; ++i)
        {
            Transform child = t.GetChild(i);
            child.gameObject.layer = layer;
            SetChildLayer(child, layer);
        }
    }

    public static void SetLayer(Transform transform, int layer, string except = "")
    {
        if (transform)
        {
            Transform[] ts = transform.GetComponentsInChildren<Transform>();
            if (ts != null)
            {
                int count = ts.Length;
                for (int i = 0; i < count; i++)
                {
                    Transform t = ts[i];
                    if(string.IsNullOrEmpty(except) || t.gameObject.name != except)
                        t.gameObject.layer = layer;
                }
            }
        }
    }

    public static GameObject InstantiateGO(Object _object)
    {
        if (_object == null)
        {
            Debug.LogError("GameObjectUtility.InstantiateGO obj is null.");
            return null;
        }
        return Object.Instantiate(_object) as GameObject;
    }
    public static int GetMinRenderQueue(Renderer renderer_)
    {
        Material[] _list = renderer_.sharedMaterials;
        int q = _list[0].renderQueue;
        for (int i = 1; i < _list.Length; ++i)
            if (_list[i].renderQueue < q)
                q = _list[i].renderQueue;
        return q;
    }
    public static GameObject AddInstantiateObj(GameObject obj, Transform p)
    {
        GameObject o = GameObjectTools.InstantiateGO(obj);
        o.SetActive(true);
        if(p)
            o.transform.SetParent(p);
        o.transform.localScale = Vector3.one;
        o.transform.localPosition = Vector3.zero;
        o.transform.localRotation = Quaternion.identity;
        return o;
    }

    public static GameObject Instantiate(Object _object, params System.Type[] components)
    {
        GameObject gameObject = InstantiateGO(_object);
        if (gameObject && components != null)
            foreach (System.Type c in components)
                gameObject.AddComponent(c);
        return gameObject;
    }

    public static T Instantiate<T>(Object _object) where T : Component
    {
        GameObject gameObject = InstantiateGO(_object);
        return gameObject.AddComponent<T>();
    }

    public static void SetRenderQueue(GameObject gameObject, int queue)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        Renderer render = null;
        for (int i = 0; i < renderers.Length; i++)
        {
            render = renderers[i];
            if (render)
            {
                Material[] materials = render.materials;
                Material material = null;
                for (int j = 0; j < materials.Length; j++)
                {
                    material = materials[j];
                    material.renderQueue = queue;
                }
            }
        }
    }

    public static void SetSharedRenderQueue(GameObject gameObject, int queue)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        Renderer render = null;
        for (int i = 0; i < renderers.Length; i++)
        {
            render = renderers[i];
            if (render)
            {
                Material[] materials = render.sharedMaterials;
                for (int j = 0; j < materials.Length; j++)
                    materials[j].renderQueue = queue;
            }
        }
    }

    public static Bounds GetBounds(GameObject gameObject)
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        GetBounds(gameObject, ref bounds);
        return bounds;
    }

    public static void GetBounds(GameObject gameObject, ref Bounds bounds)
    {
        if (gameObject)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer)
                bounds.Encapsulate(renderer.bounds);
        }
            
        foreach (Transform c in gameObject.transform)
            GetBounds(c.gameObject, ref bounds);
    }

    public static void GeFullName(GameObject gameObject, ref string name)
    {
        if (gameObject)
            name = gameObject.name + "/" + name;
        if (gameObject.transform.parent != null)
        {
            GeFullName(gameObject.transform.parent.gameObject, ref name);
        }   
    }

    public static void EnableRender(GameObject gameObject, bool enable = true)
    {
        Renderer[] renderers =
            gameObject.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].enabled = enable;
    }

    public static void EnableCollider(GameObject gameObject, bool enable = true)
    {
        Collider[] colliders =
            gameObject.GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = enable;
    }
    
    public static void SetParticleLoop(this GameObject gameObj, bool b)
    {
        var ps = gameObj.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < ps.Length; i++)
        {
            if (ps[i] != null)
            {
                var main = ps[i].main;
                main.loop = b;
            }
        }
    }

    public static void SetLayer(this Transform transform, int layer)
    {
        if (transform.gameObject.layer == layer) return;
        if (transform)
        {
            Transform[] ts = transform.GetComponentsInChildren<Transform>();
            if (ts != null)
            {
                int count = ts.Length;
                for (int i = 0; i < count; i++)
                {
                    Transform t = ts[i];
                    t.gameObject.layer = layer;
                }
            }
        }
    }

    public static void SetActiveSafe(this GameObject gameObj, bool b)
    {
        if (gameObj.activeSelf != b)
            gameObj.SetActive(b);
    }

    public static void SetActiveSafe(this MonoBehaviour mono, bool b)
    {
        mono.gameObject.SetActiveSafe(b);
    }

    public static void SetActiveByCanvasGroup(this CanvasGroup c, bool b)
    {
        if (b && !c.gameObject.activeSelf)
            c.gameObject.SetActive(true);

        if (c.GetActiveState() != b)
        {
            c.alpha = b ? 1 : 0;
            c.blocksRaycasts = b;
            c.interactable = b;
        }

    }

    public static bool GetActiveState(this CanvasGroup c)
    {
        return c.alpha > 0;
    }
    public static bool GetActiveState(this UiGameObject c)
    {
        return c.CanvasGroup.alpha > 0;
    }
    public static bool GetActiveState(this UiImage c)
    {
        return c.CanvasGroup.alpha > 0;
    }

    public static void RemoveComponent<T>(this GameObject gameobject) where T: MonoBehaviour
    {
        var component = gameobject.GetComponent<T>();
        if(component != null)
        {
            Object.Destroy(component);
        }
    }

    public static void DestroyGameObject(MonoBehaviour obj)
    {
        if (obj == null) return;

        //obj.OnPreDestroy();
        if (obj.gameObject != null)
            Object.DestroyImmediate(obj.gameObject);
    }

    public static void SetRendererOrderSort(this GameObject gameObj, int order)
    {
        var renderers = gameObj.GetComponentsInChildren<Renderer>();
        for(int i = 0; i < renderers.Length; i++)
        {
            if(renderers[i])
            {
                renderers[i].sortingOrder = order;
            }
        }
    }

    public static Rigidbody2D AddRigidBody(this EntityBase entity)
    {
        var r = entity.GetComponent<Rigidbody2D>();
        if (r == null) 
        {
            r = entity.gameObject.AddComponent<Rigidbody2D>();
        }

        if (entity.EntityType == EEntityType.Player)
        {
            r.bodyType = RigidbodyType2D.Dynamic;
            r.simulated = true;
            r.useAutoMass = false;
            r.mass = 10;
            r.drag = 30;
            r.gravityScale = 0f;
            r.angularDrag = 0f;
        }
        else
        {
            r.bodyType = RigidbodyType2D.Kinematic;
        }
        r.freezeRotation = true;
        return r;
    }
}
