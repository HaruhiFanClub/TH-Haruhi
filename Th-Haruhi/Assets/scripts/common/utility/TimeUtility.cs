using System;

public static class TimeUtility
{
    public static int GetWeekNo(int timeStamp)
    {
        return (timeStamp - 1592150400) / 604800;
    }

    public static int GetCurrentSeconds()
    {
        return (int)(((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000));
    }

    //返回目标时间的年月日
    public static DateTime GetTargetTime(int t)
    {
        var dt = new DateTime(1970, 1, 1, 8, 0, 0).AddSeconds(t);
        return dt;
    }

    public static DateTime TimestampToDateTime(int ts)
    {
        var diff = DateTime.Now - DateTime.UtcNow;  // 获取时区差值
        var dateTime = DateTime.Parse(DateTime.Now.ToString("1970-01-01 00:00:00")).AddSeconds(ts).Add(diff);
        return dateTime;
    }

    public static int DateTimeToTimestamp(DateTime date)
    {
        var diff = DateTime.Now - DateTime.UtcNow;  // 获取时区差值
        var date1970 = DateTime.Parse(DateTime.Now.ToString("1970-01-01 00:00:00"));
        var d = date - date1970;
        return (int)(d.TotalSeconds - diff.TotalSeconds);
    }

    public static bool IsSameDay(DateTime dt1, DateTime dt2)
    {
        return dt1.ToShortDateString() == dt2.ToShortDateString();
    }
}

