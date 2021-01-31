using System;
using System.Windows.Forms;

class MyCalendar : MonthCalendar
{
    public event EventHandler DoubleClickEx;

    public MyCalendar()
    {
        lastClickTick = Environment.TickCount - SystemInformation.DoubleClickTime;
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        int tick = Environment.TickCount;
        if (tick - lastClickTick <= SystemInformation.DoubleClickTime)
        {
            EventHandler handler = DoubleClickEx;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        else
        {
            base.OnMouseDown(e);
            lastClickTick = tick;
        }
    }

    private int lastClickTick;
}