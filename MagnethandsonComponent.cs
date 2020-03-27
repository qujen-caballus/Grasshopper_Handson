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

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace Magnet_handson
{
    public class MagnethandsonComponent : GH_Component
    {
        GH_Document ghDocument = null;
        List<IGH_ActiveObject> ghActiveObjects = null;
        List<PointF> originalPivots = null;

        double minDistance = 200.0;
        double k = 0.3;
        bool isRestarting = false;

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        /// 


        public MagnethandsonComponent()
          : base("Magnet", 
                "Mag",
                "Joke Component",
                "playground", 
                "refrigerator")
        {
            flagNum = 0;
        }

       
        public override void CreateAttributes()
        {
            m_attributes = new CustomAttributes(this);
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("S極", "S", "Length of tangent lines", GH_ParamAccess.item, 1);
        }



        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("N極", "N", "Length of tangent lines", GH_ParamAccess.item);
        }

        

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (ghDocument == null)
            {
                ghDocument = OnPingDocument();
                ghActiveObjects = ghDocument.ActiveObjects();
                originalPivots = new List<PointF>();

                foreach (GH_ActiveObject ghActiveObject in ghActiveObjects)
                    originalPivots.Add(ghActiveObject.Attributes.Pivot);
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftAlt))
                isRestarting = true;

            if (isRestarting) // 元の位置に戻す
            {
                isRestarting = false;

                for (int i = 0; i < ghActiveObjects.Count; i++)
                {
                    IGH_ActiveObject ghActiveObject = ghActiveObjects[i];

                    if (ghActiveObject == null)
                        continue;

                    if (ghActiveObject == this)
                        continue;

                    PointF pivot = ghActiveObject.Attributes.Pivot;

                    // ピポットをもとの位置に戻す
                    PointF newPivot = new PointF(
                        (float)(1.0 - k) * pivot.X + (float)k * originalPivots[i].X,
                        (float)(1.0 - k) * pivot.Y + (float)k * originalPivots[i].Y);

                    // ピボットが元の位置に達したかどうかを確認
                    if (Math.Abs(newPivot.X - pivot.X) > 1.0 || Math.Abs(newPivot.Y - pivot.Y) > 1.0)
                    {
                        isRestarting = true;
                        ghActiveObject.Attributes.Pivot = new PointF((float)newPivot.X, (float)newPivot.Y);
                        ghActiveObject.Attributes.ExpireLayout();
                    }
                }
            }
            else if (flagNum == 0)// マウスカーソルによってコンポーネントを動かしていく
            {
                PointF cursorF = Instances.ActiveCanvas.CursorCanvasPosition;
                Point3d cursor = new Point3d(cursorF.X, cursorF.Y, 0.0);

                for (int i = 0; i < ghActiveObjects.Count; i++)
                {
                    IGH_ActiveObject ghActiveObject = ghActiveObjects[i];

                    if (ghActiveObject == null)
                        continue;

                    if (ghActiveObject == this) //　ここの処理が重要。コンポーネント自体を移動させると削除できなくなる
                        continue;

                    RectangleF bounds = ghActiveObject.Attributes.Bounds; // キャンバス上のコンポーネントの位置と寸法を記述する矩形
                    PointF centerF = new PointF(
                        (bounds.Left + bounds.Right) * 0.5f,
                        (bounds.Top + bounds.Bottom) * 0.5f);
                    Point3d center = new Point3d(centerF.X, centerF.Y, 0.0);

                    double r = Math.Sqrt(bounds.Width * bounds.Width + bounds.Height * bounds.Height) * 0.5;

                    Vector3d v = center - cursor;

                    if (v.Length < minDistance + r) // マウスカーソルがコンポーネントの位置に十分に近い場合
                    {
                        // コンポーネントをマウスカーソルから離して、minDistanceを満たすまで移動します。
                        PointF pivot = ghActiveObject.Attributes.Pivot;
                        Point3d newCenter = center + v / v.Length * k * (minDistance + r - v.Length);

                        PointF newPivot = new PointF(
                            (float)(newCenter.X + pivot.X - center.X),
                            (float)(newCenter.Y + pivot.Y - center.Y));

                        ghActiveObject.Attributes.Pivot = newPivot;
                        ghActiveObject.Attributes.ExpireLayout();
                    }
                }
            }
            else if (flagNum == 1)
            {
                PointF cursorF = Instances.ActiveCanvas.CursorCanvasPosition;
                Point3d cursor = new Point3d(cursorF.X, cursorF.Y, 0.0);

                for (int i = 0; i < ghActiveObjects.Count; i++)
                {
                    IGH_ActiveObject ghActiveObject = ghActiveObjects[i];

                    if (ghActiveObject == null)
                        continue;

                    if (ghActiveObject == this) 
                        continue;

                    RectangleF bounds = ghActiveObject.Attributes.Bounds; 
                    PointF centerF = new PointF(
                        (bounds.Left + bounds.Right) * 0.5f,
                        (bounds.Top + bounds.Bottom) * 0.5f);
                    Point3d center = new Point3d(centerF.X, centerF.Y, 0.0);

                    double r = Math.Sqrt(bounds.Width * bounds.Width + bounds.Height * bounds.Height) * 0.5;

                    Vector3d v = center - cursor;

                    if (v.Length < minDistance + r) 
                    {
                       
                        PointF pivot = ghActiveObject.Attributes.Pivot;
                        Point3d newCenter = cursor;

                        PointF newPivot = new PointF(
                            (float)(newCenter.X),
                            (float)(newCenter.Y));

                        ghActiveObject.Attributes.Pivot = newPivot;
                        ghActiveObject.Attributes.ExpireLayout();
                    }
                }
            }

            ExpireSolution(true);

        }



        public int flagNum { get; set; }



        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon { get { return Properties.Resources.icon; } }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a91757d1-7baf-4ab1-8c96-d7153fa266e9"); }
        }
    }
}
