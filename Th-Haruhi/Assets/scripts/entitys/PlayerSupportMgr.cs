using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//僚机
public class PlayerSupportMgr 
{
    public class SupportSlot : IPool
    {
        public Vector3 Pos;
        public float Rota;

        public override void Init()
        {
        }

        public override void OnDestroy()
        {
        }

        public override void Recycle()
        {
        }

        public override void Reset()
        {
        }
    }

    private List<SupportSlot> _slots = new List<SupportSlot>();
    public List<SupportSlot> GetSlots(int count)
    {
        _slots.Clear();
        var spacingUp = _master.InSlow ? _master.Deploy.supportUpSlow : _master.Deploy.supportUp;
        var spacingDown = _master.InSlow ? _master.Deploy.supportDownSlow : _master.Deploy.supportDown;


        //单数情况，先放一个在上面
        var maxCount = count;
        if (count % 2 == 1)
        {
            var slot = Pool.New<SupportSlot>() as SupportSlot;
            slot.Pos = new Vector3(0, spacingUp[1]);
            slot.Rota = 0;
            _slots.Add(slot);
            maxCount--;
        }
        for (int i = 0; i < maxCount; i++)
        {
            var slot = Pool.New<SupportSlot>() as SupportSlot;
            int loopCount = (i + 4) / 4;

            //上排
            if (i % 4 == 0)
            {
                slot.Pos = new Vector3(-spacingUp[0] * loopCount, spacingUp[1]);
                slot.Rota = 0;
            }
            if (i % 4 == 1)
            {
                slot.Pos = new Vector3(spacingUp[0] * loopCount, spacingUp[1]);
                slot.Rota = 0;
            }

            //下排
            if (i % 4 == 2)
            {
                slot.Pos = new Vector3(-spacingDown[0] * loopCount, spacingDown[1]);
                slot.Rota = 30;
            }
            if (i % 4 == 3)
            {
                slot.Pos = new Vector3(spacingDown[0] * loopCount, spacingDown[1]);
                slot.Rota = -30;
            }
            _slots.Add(slot);
        }
        return _slots;
    }


    private Player _master;
    private List<PlayerSupport> _supportList = new List<PlayerSupport>();
    public void Init(Player player)
    {
        _master = player;
    }

    public void AddSupport()
    {
        var deploy = TableUtility.GetDeploy<PlayerSupportDeploy>(_master.Deploy.supprotId);
        if(deploy == null)
        {
            Debug.LogError("僚机ID不存在:" + _master.Deploy.supprotId);
            return;
        }

        GameSystem.CoroutineStart(LoadSupport(deploy));
    }

    private IEnumerator LoadSupport(PlayerSupportDeploy deploy)
    {
        Sprite sprite = null;
        yield return TextureUtility.LoadResourceById(deploy.resourceId, sprites =>
        {
            sprite = sprites[0];
        });

        var gameObj = new GameObject("support");
        gameObj.transform.SetParent(null, false);
        gameObj.transform.position = _master.transform.position;
        gameObj.transform.localScale = _master.transform.localScale;

        var rendererObj = new GameObject("SpriteRenderer");
        var renderer = rendererObj.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = SortingOrder.PlayerSupport;
        rendererObj.transform.SetParent(gameObj.transform, false);

        //待机效果
        var effectClass = deploy.idleEffect;
        Type type = null;
        if (!string.IsNullOrEmpty(effectClass) && (type = Common.GetType(effectClass)) != null)
        {
            rendererObj.AddComponent(type);
        }

        var script = gameObj.AddComponent<PlayerSupport>();
        script.Init(deploy);
        _supportList.Add(script);
    }

    public void Update()
    {
        UpdateMove();
        UpdateShoot();
    }

    private void UpdateShoot()
    {
        for (int i = 0; i < _supportList.Count; i++)
        {
            var support = _supportList[i];
            support.UpdateShoot(_master.InSlow, _master.gameObject.layer, _master.InShoot);
        }
    }

    private void UpdateMove()
    {
        if (_supportList.Count > 0)
        {
            float deltaTime = Time.deltaTime;
            var slots = GetSlots(_supportList.Count);
            var masterPos = _master.transform.position;

            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                var support = _supportList[i];
                support.transform.position = Vector3.Lerp(support.transform.position, masterPos + slot.Pos, deltaTime * 15f);

                if (slot.Rota != support.transform.localEulerAngles.z)
                {
                    var euler = support.transform.eulerAngles;
                    euler.z = slot.Rota;
                    support.transform.localEulerAngles = euler;
                }

                Pool.Free(slot);
            }
        }
    }

    private void Clear()
    {
        _supportList.Clear();
        _slots.Clear();
    }
}

public class PlayerSupportDeploy : Conditionable
{
    public int id;
    public int resourceId;
    public int slowBulletId;
    public int slowFrame;
    public int fastBulletId;
    public int fastFrame;

    public string idleEffect;
}


