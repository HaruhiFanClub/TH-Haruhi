
using System.Collections.Generic;
using UnityEngine;

public class RegionRoot : MonoBehaviour
{
    public static Dictionary<ERegionType, List<RegionBase>> Regions;

    public static RegionBase FindBornRegionById(string bornId)
    {
        List<RegionBase> list;
        if (Regions.TryGetValue(ERegionType.Born, out list))
        {
            for(int i = 0; i < list.Count; i++)
            {
                if (list[i].BornId == bornId)
                    return list[i];
            }
        }
        return null;
    }

    private void Awake()
    {
        if(Regions == null)
        {
            Regions = new Dictionary<ERegionType, List<RegionBase>>();
        }
        Regions.Clear();

        var regions = GetComponentsInChildren<RegionBase>();
        for(int i = 0; i < regions.Length; i++)
        {
            var r = regions[i];
            if(!Regions.ContainsKey(r.Type))
            {
                Regions[r.Type] = new List<RegionBase>();
            }
            Regions[r.Type].Add(r);
        }

        var renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
           if(r) r.enabled = false;
        }
    }
}
