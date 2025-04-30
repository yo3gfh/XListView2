using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Yo3Gfh.Components.Gui
{
    /// <summary>
    /// XListView is just the regular ListView class, but with the DoubleBuffered
    /// property exposed. It also has the OptimizeRedraw property, which, when enabled,
    /// will prevent NM_CUSTOMDRAW from being forwarded to the list during the processing of WM_NOTIFY
    /// messages.
    /// 
    /// These two will prevent the dreaded flicker and general sluggishness that the SysListView
    /// control exhibits when having a lot of items. DoubleBuffered works best in detail view, while both
    /// of them enabled seem to give good results in LargeIcon, SmallIcon, List and Tile view. Remember that
    /// VirtualMode is mandatory if you want to handle lots of items (say, hundred thousands to millions).
    /// 
    /// This used to be a known workaround in WinApi C programming years ago, I don't know why the default 
    /// WinForms ListView doesn't sport these nowadays :-)
    /// </summary>
    public class XListView : System.Windows.Forms.ListView
    {
        /// <summary>
        /// user32.dll import for SendMessage
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="messg"></param>
        /// <param name="wparam"></param>
        /// <param name="lparam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr handle, uint messg, int wparam, int lparam);

        /// <summary>
        /// NMHDR structure, for WM_NOTIFY message processing
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct NMHDR
        {
            public IntPtr hwndFrom;
            public uint idFrom;
            public uint code;
        }

        /// <summary>
        /// Message equates and constants
        /// </summary>
        private const uint LVM_SETTEXTBKCOLOR = 0x1026;
        private const uint LVM_GETTEXTBKCOLOR = 0x1025;
        private const uint NM_CUSTOMDRAW = unchecked((uint)-12);
        private const int WM_NOTIFY = 0x204E;
        private const int CLR_NONE = 0xFFFFFF;

        private bool _optimizeRedraw = false;
        private int _initialBkColor;

        /// <summary>
        /// This property exposes the base class protected DoubleBuffered. Very useful to have in the case of a ListView
        /// control, for situations in which the list holds a large number of items. Combined with virtual mode, it transforms 
        /// ListView in something actually usable :-)
        /// </summary>
        [Category("Performance")]
        [Description("When enabled, this will hopefully prevent flickering and other performance issues, especially when in SmallIcon or LargeIcon view mode.")]
        [DefaultValue(false)]
        public new bool DoubleBuffered
        {
            get
            {
                return base.DoubleBuffered;
            }
            set
            {
                if (DoubleBuffered != value)
                {
                    base.DoubleBuffered = value;
                    UpdateExtendedStyles();
                }
            }
        }

        /// <summary>
        /// When true, this will prevent NM_CUSTOMDRAW from being forwarded to the list during the processing of WM_NOTIFY
        /// message. Mostly useful in large icon or small icon view mode.
        /// </summary>
        [Category("Performance")]
        [Description("Experimental feature: When enabled, this will block NM_CUSTOMDRAW forwarding during WM_NOTIFY processing, resulting in (hopefully) better performance, especially in SmallIcon or LargeIcon view mode.")]
        [DefaultValue(false)]
        public bool OptimizeRedraw
        {
            get
            {
                return _optimizeRedraw;
            }
            set
            {
                if (_optimizeRedraw != value)
                {
                    _optimizeRedraw = value;

                    if (_optimizeRedraw)
                        SendMessage(Handle, LVM_SETTEXTBKCOLOR, 0, (int)CLR_NONE);
                    else
                        SendMessage(Handle, LVM_SETTEXTBKCOLOR, 0, _initialBkColor);

                    Invalidate();
                }
            }
        }

        public XListView()
        {
        }

        /// <summary>
        /// Make sure we have a handle, then send message to LV to clear the bkgorund color, 
        /// which is transparent by default.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            _initialBkColor = SendMessage(Handle, LVM_GETTEXTBKCOLOR, 0, 0);

            if (_optimizeRedraw)
            {
                SendMessage(Handle, LVM_SETTEXTBKCOLOR, 0, (int)CLR_NONE);
            }
        }

        /// <summary>
        /// Tap into the control window procedure.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message msg)
        {
            if (msg.Msg == WM_NOTIFY)
            {
                if (_optimizeRedraw)
                {
                    var lParam = msg.GetLParam(typeof(NMHDR));

                    if (lParam is NMHDR hdr)
                    {
                        if (hdr.code == NM_CUSTOMDRAW) // block ListView perpetual painting
                        {
                            msg.Result = (IntPtr)0;
                            return;
                        }
                    }
                }
            }

            base.WndProc(ref msg);
        }
    }
}
