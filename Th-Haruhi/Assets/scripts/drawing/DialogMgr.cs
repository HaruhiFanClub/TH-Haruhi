using System.Collections.Generic;

public static class DialogMgr
{
    public static bool InDrawingDialog;

    public static List<DialogDeploy> GetDrawList(int paragraphId)
    {
        var list = new List<DialogDeploy>();

        var t = TableUtility.GetTable<DialogDeploy>();
        foreach(var d in t)
        {
            if(d.paragraphId == paragraphId)
            {
                list.Add(d);
            }
        }

        list.Sort((a, b) =>
        {
            if (a.paragraphId < b.paragraphId) return -1;
            if (a.paragraphId > b.paragraphId) return 1;
            return 0;
        });
        return list;
    }

    public static BossDialogDeplpy GetBossDialog(int playerId, int bossId, bool beforeBoss)
    {
        var t = TableUtility.GetTable<BossDialogDeplpy>();
        foreach (var d in t)
        {
            if (d.playerId == playerId && d.bossId == bossId && beforeBoss == d.beforeBoss)
            {
                return d;
            }
        }
        return null;

    }
}

public class DialogDeploy : Conditionable
{
    public int id;
    public int paragraphId;
    public int dialogId;
    public bool isLeft;
    public string drawing;
    public string text;
    public bool revertImage;
    public bool refreshBoss;
    public float forceWaitTime;
}

public class BossDialogDeplpy : Conditionable
{
    public int id;
    public int bossId;
    public int playerId;
    public int dialogId;
    public int bgmId;
    public bool beforeBoss;
}


