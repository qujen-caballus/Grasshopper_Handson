using System;
using System.Collections.Generic;
using System.Drawing;

using Rhino.Geometry;
using System.Windows.Input;
using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;

namespace Magnet_handson
{
    public class CustomAttributes : GH_ComponentAttributes
    {
        public CustomAttributes(MagnethandsonComponent owner) : base(owner) { }
        private RectangleF NpoleBounds { get; set; }
        private RectangleF SpoleBounds { get; set; }
        protected override void Layout()
        {
            base.Layout();

            SpoleBounds = new RectangleF(Bounds.X, Bounds.Bottom, Bounds.Width /2 , 30);
            NpoleBounds = new RectangleF(Bounds.X + Bounds.Width / 2, Bounds.Bottom, Bounds.Width / 2, 30);
            Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + 30);
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MagnethandsonComponent comp = Owner as MagnethandsonComponent;

                if (SpoleBounds.Contains(e.CanvasLocation))
                {
                    if (comp.flagNum == 0) return GH_ObjectResponse.Handled;
                    comp.RecordUndoEvent("S pole");
                    comp.flagNum = 0;
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;

                }

                if (NpoleBounds.Contains(e.CanvasLocation))
                {
                    if (comp.flagNum == 1) return GH_ObjectResponse.Handled;
                    comp.RecordUndoEvent("N pole");
                    comp.flagNum = 1;
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }

            }
            return base.RespondToMouseDown(sender, e);
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {

            if (channel == GH_CanvasChannel.Objects)
            {

                MagnethandsonComponent comp = Owner as MagnethandsonComponent;

                if (comp.flagNum == 0)
                {
                    // Cache the existing style.
                    GH_PaletteStyle style = GH_Skin.palette_hidden_standard;
                    // Swap out palette for normal, unselected components.
                    GH_Skin.palette_hidden_standard = new GH_PaletteStyle(Color.Red, Color.Teal, Color.PapayaWhip);
                    base.Render(canvas, graphics, channel);
                    // Put the original style back.
                    GH_Skin.palette_hidden_standard = style;
                }

                if (comp.flagNum == 1)
                {
                    // Cache the existing style.
                    GH_PaletteStyle style = GH_Skin.palette_hidden_standard;
                    // Swap out palette for normal, unselected components.
                    GH_Skin.palette_hidden_standard = new GH_PaletteStyle(Color.Blue, Color.Teal, Color.PapayaWhip);
                    base.Render(canvas, graphics, channel);
                    // Put the original style back.
                    GH_Skin.palette_hidden_standard = style;
                }



                GH_Capsule buttonSpole = GH_Capsule.CreateCapsule(SpoleBounds, comp.flagNum == 0 ? GH_Palette.Error : GH_Palette.White);
                buttonSpole.Render(graphics, this.Selected, Owner.Locked, Owner.Hidden);
                buttonSpole.Dispose();

                GH_Capsule buttonNpole = GH_Capsule.CreateCapsule(NpoleBounds, comp.flagNum == 1 ? GH_Palette.Blue : GH_Palette.White);
                buttonNpole.Render(graphics, this.Selected, Owner.Locked, Owner.Hidden);
                buttonNpole.Dispose();




                graphics.DrawString("S極", GH_FontServer.Standard, Brushes.Black, SpoleBounds, GH_TextRenderingConstants.CenterCenter);
                graphics.DrawString("N極", GH_FontServer.Standard, Brushes.Black, NpoleBounds, GH_TextRenderingConstants.CenterCenter);


            }
            else
            {
                base.Render(canvas, graphics, channel);
            }
        }
    }
}
