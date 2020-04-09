using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MapAround.DataProviders;
using MapAround.Geometry;
using MapAround.Mapping;
using MapAroundPathFinding.PathFinding;

namespace MapAroundPathFinding
{
    public partial class MainForm : Form
    {
        private Map _mapAroundMap;

        public MainForm()
        {
            InitializeComponent();
        }

        private void ClearError()
        {
            ErrorLabel.ForeColor = Color.DarkGreen;
            ErrorLabel.Text = "OK";
        }

        private void PrintError(string errorMessage)
        {
            ErrorLabel.ForeColor = Color.Red;
            ErrorLabel.Text = errorMessage;
        }

        private void SetViewBox() // Метод поиска ViewBox
        {
            BoundingRectangle rectangle = _mapAroundMap.CalculateBoundingRectangle();
            if (rectangle.IsEmpty()) return; // Расчет  области данных карты

            // Поправка, для того, что бы вписать данные в контрол
            double deltaY = rectangle.Width * MapAroundControl.Height / 2 /
                            MapAroundControl.Width - rectangle.Height / 2;

            // Установка нового ViewBox                               
            MapAroundControl.SetViewBox(new BoundingRectangle(rectangle.MinX, rectangle.MinY - deltaY,
                rectangle.MaxX, rectangle.MaxY + deltaY));

            _mapAroundMap.FeatureRenderer.FlushTitles(MapAroundControl.CreateGraphics(), rectangle, 1);
        }

        private void AddLayer(string shapeFilePath)
        {
            FeatureLayer featureLayer = new FeatureLayer();
            featureLayer.Visible = true;
            ShapeFileSpatialDataProvider fileReader = new ShapeFileSpatialDataProvider();
            
            fileReader.FileName = shapeFilePath;
            fileReader.QueryFeatures(featureLayer);

            featureLayer.PointStyle.Size = 10;
            featureLayer.PointStyle.Symbol = '*';

            //featureLayer.MaxVisibleScale = 0;
            featureLayer.MinVisibleScale = 3000;

            if (shapeFilePath.EndsWith("roads.shp"))
            {
                featureLayer.Alias = "roads";
                featureLayer.PolylineStyle.Color = Color.Chocolate;
                
            }
            else if (shapeFilePath.EndsWith("buildings.shp"))
            {
                featureLayer.Alias = "buildings";
                featureLayer.PolygonStyle.BorderColor = Color.Gray;
                featureLayer.PolygonStyle.FillForeColor = Color.DarkGray;
            }
            else if (shapeFilePath.EndsWith("points.shp"))
            {
                featureLayer.Alias = "points";
            }
            else if (shapeFilePath.EndsWith("places.shp"))
            {
                featureLayer.Alias = "places";
                featureLayer.PolygonStyle.BorderColor = Color.Blue;
                featureLayer.PolygonStyle.FillForeColor = Color.LightBlue;
            }
            else if (shapeFilePath.EndsWith("natural.shp"))
            {
                featureLayer.Alias = "natural";
                featureLayer.PolygonStyle.BorderColor = Color.Green;
                featureLayer.PolygonStyle.FillForeColor = Color.LightGreen;
                featureLayer.MinVisibleScale = 0;
            }
            else if (shapeFilePath.EndsWith("railways.shp"))
            {
                featureLayer.Alias = "railways";
                featureLayer.PolylineStyle.Width = 2;
                featureLayer.PolylineStyle.Color = Color.Gray;
            }
            else if (shapeFilePath.EndsWith("places.shp"))
            {
                featureLayer.Alias = "places";
                featureLayer.PolygonStyle.BorderColor = Color.BlueViolet;
                featureLayer.PolygonStyle.FillForeColor = Color.CornflowerBlue;
            }
            else if (shapeFilePath.EndsWith("landuse.shp"))
            {
                featureLayer.Alias = "landuse";
                featureLayer.PolygonStyle.BorderColor = Color.DarkOliveGreen;
                Color fillColor = Color.FromArgb(50, Color.DarkKhaki);
                
                featureLayer.PolygonStyle.FillForeColor = fillColor;
            }
            else if (shapeFilePath.EndsWith("waterways.shp"))
            {
                featureLayer.Alias = "waterways";
                featureLayer.PolylineStyle.Color = Color.DarkBlue;
            }
            else
            {
                FileInfo file = new FileInfo(shapeFilePath);
                PrintError($"{file.Name} не распознан как допустимый слой");
            }

            _mapAroundMap.AddLayer(featureLayer);
        }

        private void LoadMap(string mapFolder)
        {
            ClearError();

            _mapAroundMap = new Map();

            var fileEnumerable = Directory.EnumerateFiles(mapFolder);
            var shpFiles = fileEnumerable.Where(n => n.EndsWith(".shp"));

            foreach (var geometryFile in shpFiles)
            {
                try
                {
                    AddLayer(geometryFile);
                }
                catch (Exception e)
                {
                    PrintError(e.Message);
                    throw;
                }
            }
            
            MapAroundControl.Map = _mapAroundMap;
            SetViewBox();
        }

        private void OpenMapButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
#if DEBUG
            folderBrowser.SelectedPath = "C:\\Users\\Dmitry\\Documents\\GitHub\\Diploma\\Data\\Wichita\\shape";
#endif
            folderBrowser.ShowDialog();

            if(String.IsNullOrEmpty(folderBrowser.SelectedPath))
                return;

            string mapPath = folderBrowser.SelectedPath;

            LoadMap(mapPath);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void LayerSettings_Click(object sender, EventArgs e)
        {
            if(_mapAroundMap == null)
                return;

            LayerSettingsForm settingsForm = new LayerSettingsForm(_mapAroundMap);
            settingsForm.OnLayerSettingsChanged += OnLayerSettingsChanged;
            settingsForm.Show();
        }

        private void OnLayerSettingsChanged(LayerBase layer)
        {
            MapAroundControl.RedrawMap();
        }

        private LayerBase FindLayerByAlias(string alias)
        {
            return _mapAroundMap.Layers.FirstOrDefault(l => l.Alias.Equals(alias));
        }

        private void GetCellMapButton_Click(object sender, EventArgs e)
        {
            BoundingRectangle rectangle = MapAroundControl.GetViewBox();
            double width = rectangle.Width;
            double height = rectangle.Height;

            double cellSize = 4E-5;
            MapAroundCellMap cellMap = new MapAroundCellMap(_mapAroundMap, rectangle, cellSize, cellSize);
            cellMap.AddPolygonObstaclesLayer((FeatureLayer)FindLayerByAlias("buildings"));
            CellMapDrawerForm drawerForm = new CellMapDrawerForm(cellMap);
            drawerForm.Show();
        }
    }
}
