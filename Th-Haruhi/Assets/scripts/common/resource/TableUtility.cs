﻿//////////////////////////////////////////////////////////////////////////
//
//   FileName : TableMgr.cs
//     Author : Felon
// CreateTime : 2017-04-26
//       Desc :
//
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TableUtility
{
    public static void Init()
    {
        _(typeof(SoundDeploy), "sound");
        _(typeof(PlayerDeploy), "player");
        _(typeof(PlayerSupportDeploy), "player_support");
        _(typeof(StageDeploy), "stage");
        _(typeof(BulletDeploy), "bullet");
        _(typeof(TextureResourceDeploy), "texture_resource");
        _(typeof(TextureEffectDeploy), "effect");
        _(typeof(EnemyDeploy), "enemy");
        _(typeof(DialogDeploy), "drawing_dialog");
        _(typeof(BossDialogDeplpy), "boss_dialog");
    }
    
    private class RandomIdData
    {
        public int ItemId;
        public float StartProb;
        public float EndProb;
    }

    private static readonly List<RandomIdData> _dropPool = new List<RandomIdData>();
    private static readonly List<int> _exceptItemId = new List<int>();


    //按概率随机1个ID，配置格式为[[201,10],[202,10],[204,10]]
    public static Dictionary<int, int> RandomIds(int[][] itemIdPool, int count, bool allowSame = false)
    {
        var itemGiveCount = new int[count];
        for(int i = 0; i < count; i++)
        {
            itemGiveCount[i] = 1;
        }
        return RandomReward(itemIdPool, itemGiveCount, allowSame);
    }

    public static int RandomOne(int[][] itemIdPool)
    {
        _exceptItemId.Clear();
        var itemId = RandomOneReward(itemIdPool, _exceptItemId);
        return itemId;
    }

    //按概率随机n个ID，配置格式为[[201,10],[202,10],[204,10]]
    public static Dictionary<int, int> RandomReward(int[][] itemIdPool, int[] itemGiveCount, bool allowSame = false)
    {
        var dict = new Dictionary<int, int>();
        _exceptItemId.Clear();

        for (int i = 0; i < itemGiveCount.Length; i++)
        {
            var giveCount = itemGiveCount[i];

            var itemId = RandomOneReward(itemIdPool, _exceptItemId);
            if (itemId == 0)
            {
                Debug.LogError("随机掉落物品，随机出来物品ID为0，是否配置错了？");
                continue;
            }

            if (!allowSame)
            {
                _exceptItemId.Add(itemId);
            }
            dict[itemId] = giveCount;
        }
        return dict;
    }

    private static int RandomOneReward(int[][] itemIdPool, List<int> exceptItemId)
    {
        _dropPool.Clear();

        float startProb = 0;
        float totalProb = 0;
        for (int i = 0; i < itemIdPool.Length; i++)
        {
            var dropInfo = itemIdPool[i];
            var itemId = dropInfo[0];
            var prob = dropInfo[1];

            bool bExcept = false;
            for (int j = 0; j < exceptItemId.Count; j++)
            {
                if (exceptItemId[j] == itemId)
                {
                    bExcept = true;
                    break;
                }
            }

            if (bExcept)
            {
                continue;
            }

            totalProb = startProb + prob;

            _dropPool.Add(new RandomIdData
            {
                ItemId = itemId,
                StartProb = startProb,
                EndProb = totalProb,
            });

            startProb = totalProb;
        }


        var randomValue = UnityEngine.Random.Range(0f, totalProb);
        for (int j = 0; j < _dropPool.Count; j++)
        {
            var dropInfo = _dropPool[j];
            if (randomValue >= dropInfo.StartProb && randomValue < dropInfo.EndProb)
            {
                return dropInfo.ItemId;
            }
        }
        return 0;
    }
    //--------------------------------------------------------------------------------------------------//
    public static readonly Dictionary<Type, string> Paths = new Dictionary<Type, string>();

    public static T GetDeploy<T>(object id) where T: Conditionable
    {
        var table = GetTable<T>();
        return table == null ? null : table.GetSection(id);
    }

    public static TableT<T> GetTable<T>() where T : Conditionable
    {
        TableT<T> table = null;
        var className = typeof(T);
        if (Paths.ContainsKey(className))
        {
            table = TableDatabase.Load<T>(Paths[className]);
            if (table == null)
            {
                Debug.LogError("can not open " + Paths[className] + " table");
                return default(TableT<T>);
            }
        }

        if (table == null)
        {
            Debug.LogError(string.Format("table {0} not init url", className));
        }
        return table;
    }

    static void _(Type type, string url)
    {
        Paths[type] = url;
    }
}
