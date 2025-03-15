using g3;
using goa.Revit.DirectContext3D;
using PubFuncWt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using goa.Common;

namespace MountSiteDesignAnaly
{
    class DisplayDomAndWpf
    {

        Thickness thickness5 = new Thickness(5);
        Thickness thickness2 = new Thickness(5);

        internal List<SingleGreenInfor> coverSoilInfors;

        List<IGrouping<double, SingleGreenInfor>> Group;

        List<double> heightDifferences;
        List<Color> colors;


        int count { get { return this.Group.Count; } }

        internal DisplayDomAndWpf(List<SingleGreenInfor> _coverSoilInfors)
        {
            this.coverSoilInfors = _coverSoilInfors;
        }

        internal void Excute()
        {
            this.GetGroup();

            this.ClearGridContent();
            this.ShowTris();

            // wpf 界面数据 显示处理
            this.AddTitle();
            this.AddColorBlocks();
        }
        void GetGroup()
        {
            this.Group = this.coverSoilInfors.GroupBy(p => p.CoverThickness).OrderByDescending(p => p.Key).ToList();
            // 图形三角化显示
            List<System.Drawing.Color> colors = Color_.GetColorListRedToMagenta(Color_.RainbowColors, this.count);

            for (int i = 0; i < this.count; i++)
            {
                foreach (var item in this.Group[i])
                {
                    item.ColorWpf = Color.FromArgb(colors[i].A, colors[i].R, colors[i].G, colors[i].B);
                }
            }

            this.heightDifferences = this.Group.Select(p => p.Key).ToList();
            this.colors = colors.Select(p => Color.FromArgb(p.A, p.R, p.G, p.B)).ToList();

        }

        void ClearGridContent()
        {
            MainWindow.Instance.Height = 250;

            int eleCount = MainWindow.Instance.OverlayLegendGrid.Children.Count;

            if (eleCount > 4)
            {
                for (int i = eleCount - 1; i > 3; i--)
                {
                    MainWindow.Instance.OverlayLegendGrid.Children.RemoveAt(i);
                }
            }
            else if (eleCount > 2)
            {
                for (int i = eleCount - 1; i > 1; i--)
                {
                    MainWindow.Instance.OverlayLegendGrid.Children.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Revit文档中显示三角面
        /// </summary>
        void ShowTris()
        {
            GeometryDrawServerInputs geometryDrawServerInputs = new GeometryDrawServerInputs();
            foreach (var item in this.coverSoilInfors)
            {
                Autodesk.Revit.DB.XYZ offsetXyz = new Autodesk.Revit.DB.XYZ(0, 0, item.coverFloor.elevation) + new Autodesk.Revit.DB.XYZ(0, 0, 100);
                foreach (Polygon2d o in item.EffectiveGreenPolygons)
                {
                    //CMD.Doc.CreateDirectShapeWithNewTransaction(o.ToCurveLoop().ToList());
                    //continue;

                    foreach (Triangle3d triangle3d in o.Triangle3ds())
                    {
                        geometryDrawServerInputs.AddTriangleToBuffer(triangle3d, new Autodesk.Revit.DB.XYZ(0, 0, 1), item.colorWithTransparency, offsetXyz, false);
                        // 取点会快一些，显示为点阵，效果差强人意
                        //geometryDrawServerInputs.AddPointToBuffer(triangle3d.Center().ToXYZ(), item.colorWithTransparency, new Autodesk.Revit.DB.XYZ(0, 0, 200));
                    }
                }
            }

            GeometryDrawServersMgr.ShowGraphics(geometryDrawServerInputs, Guid.NewGuid().ToString());
        }
        void AddTitle()
        {
            if (this.count > 0)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(10, GridUnitType.Auto);

                MainWindow.Instance.OverlayLegendGrid.RowDefinitions.Add(row);

                TextBlock textBlock01 = new TextBlock();
                textBlock01.Text = "图例颜色";
                textBlock01.Margin = thickness5;
                textBlock01.HorizontalAlignment = HorizontalAlignment.Center;

                TextBlock textBlock02 = new TextBlock();
                textBlock02.Text = "覆土厚度";
                textBlock02.Margin = thickness5;
                textBlock02.HorizontalAlignment = HorizontalAlignment.Center;

                MainWindow.Instance.OverlayLegendGrid.Children.Add(textBlock01);
                MainWindow.Instance.OverlayLegendGrid.Children.Add(textBlock02);

                Grid.SetRow(textBlock01, 0);
                Grid.SetColumn(textBlock01, 0);

                Grid.SetRow(textBlock02, 0);
                Grid.SetColumn(textBlock02, 1);
            }

            for (int i = 0; i < this.count / 2 + 1; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(10, GridUnitType.Auto);
                MainWindow.Instance.OverlayLegendGrid.RowDefinitions.Add(row);

            }

            if (this.count > 1)
            {
                TextBlock textBlock03 = new TextBlock();
                textBlock03.Text = "图例颜色";
                textBlock03.Margin = thickness5;
                textBlock03.HorizontalAlignment = HorizontalAlignment.Center;

                TextBlock textBlock04 = new TextBlock();
                textBlock04.Text = "覆土厚度";
                textBlock04.Margin = thickness5;
                textBlock04.HorizontalAlignment = HorizontalAlignment.Center;

                MainWindow.Instance.OverlayLegendGrid.Children.Add(textBlock03);
                MainWindow.Instance.OverlayLegendGrid.Children.Add(textBlock04);

                Grid.SetRow(textBlock03, 0);
                Grid.SetColumn(textBlock03, 2);

                Grid.SetRow(textBlock04, 0);
                Grid.SetColumn(textBlock04, 3);

            }

            double height = (this.count / 2 + 1) * 30;

            if (height > 300)
            {
                MainWindow.Instance.scrollViewer.Height = 400;
                MainWindow.Instance.Height += 430;
            }
            else
            {
                MainWindow.Instance.scrollViewer.Height = height;
                MainWindow.Instance.Height += height + 30;
            }

            //MainWindow.Instance.Height += (this.count / 2 + 1) * 35;
        }
        void AddColorBlocks()
        {
            for (int i = 0; i < this.count; i++)
            {
                Rectangle rectangle01 = new Rectangle();
                SolidColorBrush solidColorBrush = new SolidColorBrush();
                solidColorBrush.Color = this.colors[i];
                rectangle01.Fill = solidColorBrush;
                rectangle01.Margin = thickness2;

                TextBlock textBlock01 = new TextBlock();
                textBlock01.Text = this.heightDifferences[i].FeetToMillimeterString(2) + "mm";
                textBlock01.Margin = thickness2;
                textBlock01.HorizontalAlignment = HorizontalAlignment.Center;

                MainWindow.Instance.OverlayLegendGrid.Children.Add(rectangle01);
                MainWindow.Instance.OverlayLegendGrid.Children.Add(textBlock01);

                int remainder = i % 2;
                int quotient = i / 2;

                if (remainder == 1)
                {
                    Grid.SetRow(rectangle01, quotient + 1);
                    Grid.SetColumn(rectangle01, 2);

                    Grid.SetRow(textBlock01, quotient + 1);
                    Grid.SetColumn(textBlock01, 3);
                }
                else if (remainder == 0)
                {
                    Grid.SetRow(rectangle01, quotient + 1);
                    Grid.SetColumn(rectangle01, 0);

                    Grid.SetRow(textBlock01, quotient + 1);
                    Grid.SetColumn(textBlock01, 1);
                }
            }
        }
    }
}
