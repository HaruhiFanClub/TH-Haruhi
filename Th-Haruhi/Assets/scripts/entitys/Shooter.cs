using UnityEngine;

public class Shooter
{
    private Character _master;
    private Object _source1;
    private Object _source2;
    public Shooter(Character entity)
    {
        _master = entity;

        _source1 = ResourceMgr.LoadImmediately("bullet/bullet1.prefab");
        _source2 = ResourceMgr.LoadImmediately("bullet/bullet2.prefab");
    }

    private bool _inShoot;
    public void StartShoot()
    {
        _inShoot = true;
    }

    public void EndShoot()
    {
        _inShoot = false;
    }

    RelayInterval interval = new RelayInterval(0.04f);
    private int _shootCount;
    public void Update()
    {
        if(_inShoot)
        {
            if (!interval.NextTime()) return;

            if(_shootCount == 1)
            {
                ResourceMgr.Instantiate(_source1).GetComponent<Bullet>().Shoot(_master.ShootSlot1.position, Vector2.up);
                ResourceMgr.Instantiate(_source1).GetComponent<Bullet>().Shoot(_master.ShootSlot2.position, Vector2.up);
                ResourceMgr.Instantiate(_source1).GetComponent<Bullet>().Shoot(_master.ShootSlot3.position, Vector2.up);
                ResourceMgr.Instantiate(_source1).GetComponent<Bullet>().Shoot(_master.ShootSlot4.position, Vector2.up);
                ResourceMgr.Instantiate(_source1).GetComponent<Bullet>().Shoot(_master.ShootSlot5.position, Vector2.up);
                ResourceMgr.Instantiate(_source1).GetComponent<Bullet>().Shoot(_master.ShootSlot6.position, Vector2.up);
            }
            else
            {
                ResourceMgr.Instantiate(_source2).GetComponent<Bullet>().Shoot(_master.ShootSlot1.position, Vector2.up);
                ResourceMgr.Instantiate(_source2).GetComponent<Bullet>().Shoot(_master.ShootSlot2.position, Vector2.up);
                ResourceMgr.Instantiate(_source2).GetComponent<Bullet>().Shoot(_master.ShootSlot3.position, Vector2.up);
                ResourceMgr.Instantiate(_source2).GetComponent<Bullet>().Shoot(_master.ShootSlot4.position, Vector2.up);
                ResourceMgr.Instantiate(_source2).GetComponent<Bullet>().Shoot(_master.ShootSlot5.position, Vector2.up);
                ResourceMgr.Instantiate(_source2).GetComponent<Bullet>().Shoot(_master.ShootSlot6.position, Vector2.up);
            }
            _shootCount++;
            if (_shootCount == 2) _shootCount = 0;
          

            Sound.PlayUiAudioOneShot(2001);
        }
    }
}