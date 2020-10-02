
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDebugBulletLib : UiInstance
{
    private UIDebugBulletLibComponent _component;
    protected override void OnLoadFinish()
    {
        base.OnLoadFinish();
        _component = GetComponent<UIDebugBulletLibComponent>();
        _component.CloseBtn.onClick.AddListener(() => { this.Close(); });
        _component.BtnShowPoint.onClick.AddListener(OnClickShowPoint);
        StartCoroutine(LoadBullets());
    }

    private bool _showPoint = true;
    private List<GameObject> _pointList = new List<GameObject>();
    private void OnClickShowPoint()
    {
        _showPoint = !_showPoint;
        for(int i = 0; i < _pointList.Count; i++)
        {
            _pointList[i].SetActiveSafe(_showPoint);
        }
    }

    public override void OnClose(Action<UiInstance> notify)
    {
        base.OnClose(notify);
        StopAllCoroutines();
    }

    private IEnumerator LoadBullets()
    {
        var soringLayerId = UiManager.GetCanvasByLayer(UiLayer.Tips).sortingLayerID;

        var tab = TableUtility.GetTable<BulletDeploy>();
        foreach(var deploy in tab)
        {
            if (deploy.id < 1000) continue;

            //ui prefab
            var bulletObj = ResourceMgr.Instantiate(_component.BulletPrefab);
            var txtId = bulletObj.GetComponentInChildren<UiText>();
            txtId.text = deploy.id.ToString();
            bulletObj.transform.SetParent(_component.Grid, false);

            //createBullet
            Bullet bullet = null;
            BulletFactory.CreateBullet(deploy.id, bulletObj.transform, Layers.Ui, b =>
            {
                bullet = b;
            });
            yield return new WaitUntil(() => bullet != null);

            var localScale = bullet.Renderer.transform.localScale;
            var ratio = localScale.x * 100;
            if (ratio > 100) ratio = 96f / localScale.x;

            bullet.Renderer.sortingLayerID = soringLayerId;
            bullet.transform.localScale = Vector3.one * ratio;
            bullet.Renderer.transform.SetLayer(Layers.Ui);
            bullet.transform.SetParent(bulletObj.transform, false);

            //显示collider
            var col = new GameObject("collider");
            col.transform.SetParent(bullet.transform, false);
            col.AddComponent<MeshFilter>().sharedMesh = GameSystem.DefaultRes.QuadMesh;
            col.layer = Layers.Ui;

            var material = new Material(GameSystem.DefaultRes.CommonShader);
            material.SetColor("_TintColor", Color.red);
            material.SetFloat("_AlphaScale", deploy.alpha);

            var collider = bullet.GetComponentInChildren<Collider2D>();
            var circleCollider = collider as CircleCollider2D;
            if (circleCollider != null)
            {
                material.mainTexture = GameSystem.DefaultRes.CircleTexture;
                col.transform.localScale = Vector3.one * circleCollider.radius * 2;
                
            }
            else
            {
                var boxCollider = collider as BoxCollider2D;
                if(boxCollider != null)
                {
                    material.mainTexture = GameSystem.DefaultRes.BoxTexture;
                    col.transform.localScale = new Vector3(boxCollider.size.x, boxCollider.size.y, 1);
                    col.transform.localPosition = boxCollider.offset;
                }
            }

            var mr = col.AddComponent<MeshRenderer>();
            mr.sharedMaterial = material;
            mr.sortingLayerID = soringLayerId;
            mr.sortingOrder = bullet.Renderer.sortingOrder + 1;
            col.SetActiveSafe(_showPoint);
            _pointList.Add(col);
        }
    }
}
