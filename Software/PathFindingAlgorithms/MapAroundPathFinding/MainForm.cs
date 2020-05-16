using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MapAround.DataProviders;
using MapAround.Geometry;
using MapAround.Mapping;
using MapAroundPathFinding.PathFinding;
using PathFinders;
using PathFinders.Algorithms;
using PathFinders.Algorithms.HpaStar;
using PathFinders.Algorithms.PathSmoothing;
using PathFinders.Logging;
using Coordinate = MapAround.Geometry.Coordinate;

namespace MapAroundPathFinding
{
    public partial class MainForm : Form, ILogger
    {
        private Map _mapAroundMap;
        private BoundingRectangle _initialRectangle;

        public MainForm()
        {
            InitializeComponent();

            LogManager.Logger = this;
        }

        #region RegionDrawing

        private const int PolygonFinalizePixelDelta = 10;
        private List<ICoordinate> _points = new List<ICoordinate>();
        private bool _isFinalized;
        private List<RasterLayer> _rasterLayers = new List<RasterLayer>();
        private FeatureLayer _userRegionLayer;
       
        private void InitUserPolygonLayer()
        {
            _userRegionLayer = new FeatureLayer();
            _userRegionLayer.Alias = "User region layer";
            _userRegionLayer.PolygonStyle.FillForeColor = Color.FromArgb(100, 255, 0, 0);
            _userRegionLayer.PolygonStyle.BorderColor = Color.Red;
            _userRegionLayer.Visible = true;

            _mapAroundMap.AddLayer(_userRegionLayer);
        }

        private void ClearRasterLayers()
        {
            foreach (var layer in _rasterLayers)
            {
                _mapAroundMap.RemoveLayer(layer);
            }

            _rasterLayers.Clear();
        }

        private void FinalizePolygon()
        {
            Debug.WriteLine("Finishing polygon...");

            ICoordinate firstCoordinate = _points[0];
            ICoordinate lastCoordinate = _points.Last();

            Point first = MapAroundControl.MapToClient(firstCoordinate);
            Point last = MapAroundControl.MapToClient(lastCoordinate);


            var rectangle = MapAroundControl.GetViewBox();

            DrawLine(rectangle, Color.Green, first, last);

            _isFinalized = true;

            Polygon polygon = new Polygon(_points);
            Feature polygonFeature = new Feature(FeatureType.Polygon) {Polygon = polygon};
            _userRegionLayer.AddPolygon(polygonFeature);
            _points.Clear();
            ClearRasterLayers();
            MapAroundControl.RedrawMap();
        }


        private void OnRightClick(Point point)
        {
            ICoordinate coordinate = MapAroundControl.ClientToMap(point);
            if (_isFinalized)
            {
                Debug.WriteLine("Clearing picture box");

                ClearRasterLayers();

                _points.Clear();
                _isFinalized = false;
            }

            if (_points.Count == 0)
            {
                _points.Add(coordinate);
            }
            else
            {
                Point firstPoint = MapAroundControl.MapToClient(_points[0]);
                int firstDx = Math.Abs(firstPoint.X - point.X);
                int firstDy = Math.Abs(firstPoint.Y - point.Y);

                if (firstDx < PolygonFinalizePixelDelta && firstDy < PolygonFinalizePixelDelta)
                {
                    FinalizePolygon();
                    return;
                }

                Point prevPoint = MapAroundControl.MapToClient(_points.Last());

                var rectangle = MapAroundControl.GetViewBox();
                DrawLine(rectangle, Color.Red, point, prevPoint);
                _points.Add(coordinate);
            }
        }

        private void DrawLine(BoundingRectangle rectangle, Color color, Point p0, Point p1)
        {
            int x0 = p0.X;
            int y0 = p0.Y;
            int x1 = p1.X;
            int y1 = p1.Y;

            var rectMinPixel = MapAroundControl.MapToClient(rectangle.Min);
            var rectMaxPixel = MapAroundControl.MapToClient(rectangle.Max);
            int bitmapWidth = Math.Abs(rectMinPixel.X - rectMaxPixel.X);
            int bitmapHeight = Math.Abs(rectMinPixel.Y - rectMaxPixel.Y);

            int minX = Math.Min(p0.X, p1.X);
            int minY = Math.Min(p0.Y, p1.Y);
            int maxX = Math.Max(p0.X, p1.X);
            int maxY = Math.Max(p0.Y, p1.Y);

            if (bitmapWidth == 0)
                bitmapWidth = 1;
            if (bitmapHeight == 0)
                bitmapHeight = 1;

            Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight);
            RasterLayer layer;
            layer = new RasterLayer();
            _rasterLayers.Add(layer);
            _mapAroundMap.AddLayer(layer);

            layer.AddRasterPreview(bitmap, rectangle, bitmap.Width, bitmap.Height);
            layer.Visible = true;


            //Rectangle lockRect = new Rectangle(minX, minY, maxX - minX, maxY - minY);
            Rectangle lockRect = new Rectangle(0, 0, bitmapWidth, bitmapHeight);
            BitmapData bitmapData = bitmap.LockBits(lockRect, ImageLockMode.WriteOnly, bitmap.PixelFormat);

            byte[] pixelBytes = { color.B, color.G, color.R, color.A };

            bool PlotPoint(int x, int y)
            {
                int plotX = x;
                int plotY = y;
                if (plotX < 0 || plotY < 0 || plotX >= bitmap.Width || plotY >= bitmap.Height)
                {
                    return true;
                }

                int positionByte = (y * bitmap.Width + x) * pixelBytes.Length;
                
                Marshal.Copy(pixelBytes, 0, bitmapData.Scan0 + positionByte, pixelBytes.Length);
                //bitmap.SetPixel(plotX, plotY, color);
                return true;
            }

            BresenhamLinePlotter plotter = new BresenhamLinePlotter();
            plotter.CastLine(new Vector2Int(x0, y0), new Vector2Int(x1, y1), PlotPoint);

            bitmap.UnlockBits(bitmapData);

            MapAroundControl.RedrawMap();
        }

        #endregion

        #region PathFinding
        private const double DefaultCellSize =  4E-5;
        //private const double DefaultCellSize =  2E-5;
        private ICoordinate _startPoint;
        private ICoordinate _endPoint;
        private MapAroundCellMap _cellMap;
        private ICellPathFinder _pathFinder;
        private Task _backgroundTask;
        private FeatureLayer _pathFeatureLayer;
        private bool _pathFindingUiActive = true;

        private void SetUiActive(bool active)
        {
            _pathFindingUiActive = active;
            if (InvokeRequired)
            {
                BeginInvoke(new Action<bool>(SetUiActive), active);
                return;
            }

            OpenMapButton.Enabled = active;
            LayerSettings.Enabled = active;
        }

        private void InitCellMap()
        {
            _cellMap = new MapAroundCellMap(_mapAroundMap, _initialRectangle, DefaultCellSize, DefaultCellSize);
            _cellMap.AddPolygonObstaclesLayer((FeatureLayer)FindLayerByAlias("buildings"));
        }

        private void InitHpaStar()
        {
            _pathFinder = new HpaStarAlgorithm();

            SetUiActive(false);
            
            _backgroundTask = new Task(PreBuildGraphJob, TaskCreationOptions.LongRunning);
            _backgroundTask.Start();
        }

        private void PreBuildGraphJob()
        {
            _pathFinder.Initialize(_cellMap);
            SetUiActive(true);
        }

        private void InitPathLayer()
        {
            _pathFeatureLayer = new FeatureLayer();
            _pathFeatureLayer.PointStyle.Color = Color.Green;
            _pathFeatureLayer.PointStyle.Size = 10;
            _pathFeatureLayer.PointStyle.Symbol = '*';
            _pathFeatureLayer.Visible = true;
            _mapAroundMap.AddLayer(_pathFeatureLayer);
        }

        private void ClearPathLayer()
        {
            _pathFeatureLayer.RemoveAllFeatures();
        }

        private Vector2Int ProjectToCellMap(ICoordinate coordinate)
        {
            double boundXMin = _cellMap.BoundingRectangle.MinX;
            double boundYMin = _cellMap.BoundingRectangle.MinY;
            double xCell = coordinate.X / _cellMap.CellWidth;
            double yCell = coordinate.Y / _cellMap.CellHeight;
            xCell -= boundXMin / _cellMap.CellWidth;
            yCell -= boundYMin / _cellMap.CellHeight;

            int x = (int)Math.Round(xCell);
            int y = (int)Math.Round(yCell);
            return new Vector2Int(x, y);
        }

        private Coordinate ProjectFromCellMap(Vector2Int cell)
        {
            double boundXMin = _cellMap.BoundingRectangle.MinX;
            double boundYMin = _cellMap.BoundingRectangle.MinY;
            double x = cell.X * _cellMap.CellWidth;
            double y = cell.Y * _cellMap.CellHeight;
            x += boundXMin;
            y += boundYMin;
            return new Coordinate() {X = x, Y = y};
        }

        private void StartPathFinding()
        {
            Debug.WriteLine("Path finding started...");
            ClearPathLayer();

            SetUiActive(false);
            _backgroundTask = new Task(GetPathJob, TaskCreationOptions.LongRunning);
            _backgroundTask.Start();
        }

        private void ShowCellMapDrawerForm(CellMapDrawerForm form, ICellMap map, IList<Vector2Int> path)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<CellMapDrawerForm, ICellMap, IList<Vector2Int>>(ShowCellMapDrawerForm), form, map, path);
                return;
            }
            Debug.WriteLine($"CellMapDrawerForm showed on thread {Thread.CurrentThread.ManagedThreadId}");
            form.ShowMap(map, path);
        }

        private void ShowBitmapDrawer(BitmapDrawerForm drawerForm, Bitmap bitmap)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<BitmapDrawerForm, Bitmap>(ShowBitmapDrawer), drawerForm, bitmap);
                return;
            }

            drawerForm.DrawBitmap(bitmap);
            drawerForm.Show();
        }

        private T CreateForm<T>() where T: Form
        {
            if (InvokeRequired)
            {
                var asyncResult = BeginInvoke(new Func<T>(CreateForm<T>));
                T result = (T)EndInvoke(asyncResult);
                return result;
            }

            T form = Activator.CreateInstance<T>();
            Debug.WriteLine($"CellMapDrawerForm instantiated on thread {Thread.CurrentThread.ManagedThreadId}");
            return form;
        }

        private void GetPathJob()
        {
            _pathFinder.ClearObstacles();
            foreach (var feature in _userRegionLayer.Features)
            {
                if (feature.FeatureType == FeatureType.Polygon)
                {
                    PolygonCellFragment cellFragment =
                        new PolygonCellFragment(feature, _initialRectangle, DefaultCellSize, DefaultCellSize);
                    _pathFinder.AddObstacle(cellFragment);
                }
            }

            _pathFinder.RecalculateObstacles();

            Vector2Int startVector2Int = ProjectToCellMap(_startPoint);
            Vector2Int stopVector2Int = ProjectToCellMap(_endPoint);

            var path = _pathFinder.GetSmoothedPath(_cellMap, startVector2Int, stopVector2Int, NeighbourMode.SidesAndDiagonals);
            //var path = _pathFinder.GetPath(_cellMap, startVector2Int, stopVector2Int, NeighbourMode.SidesAndDiagonals);

            if (path == null)
            {
                MessageBox.Show("Путь не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DrawPath(path);
            }

            _startPoint = null;
            _endPoint = null;

            SetUiActive(true);
        }

        private void DrawPath(IList<Vector2Int> path)
        {
            foreach (var cell in path)
            {
                ICoordinate coordinate = ProjectFromCellMap(cell);
                Feature pointFeature = new Feature(FeatureType.Point);
                PointD point = new PointD(coordinate);
                pointFeature.Point = point;
                _pathFeatureLayer.AddFeature(pointFeature);
            }
            Debug.WriteLine("Path finding finished!");
            RedrawMap();

            //CellMapDrawerForm drawerForm = CreateForm<CellMapDrawerForm>();
            //ShowCellMapDrawerForm(drawerForm, _hpaStar.LayeredCellMap, rawPath);
        }

        private void RedrawMap()
        {
            if (MapAroundControl.InvokeRequired)
            {
                MapAroundControl.BeginInvoke(new Action(RedrawMap));
            }
            else
            {
                MapAroundControl.RedrawMap();
            }
        }

        private void OnMiddleClick(Point point)
        {
            if (_pathFindingUiActive == false)
            {
                return;
            }
            if (_backgroundTask == null || _backgroundTask.Status != TaskStatus.Running)
            {
                if (_startPoint == null)
                {
                    _startPoint = MapAroundControl.ClientToMap(point);
                }
                else
                {
                    _endPoint = MapAroundControl.ClientToMap(point);
                    StartPathFinding();
                }
            }
        }
        #endregion

        #region EventHandlers
        private void MapAroundControl_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void MapAroundControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (_mapAroundMap == null)
                return;
            MouseEventArgs me = (MouseEventArgs)e;
            Point mousePoint = me.Location;
            switch (me.Button)
            {
                case MouseButtons.Right:
                {
                    OnRightClick(mousePoint);
                    break;
                }
                case MouseButtons.Middle:
                {
                    OnMiddleClick(mousePoint);
                    break;
                }
            }
        }

        private void OpenMapButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            //#if DEBUG
            folderBrowser.SelectedPath = "C:\\Users\\Dmitry\\Documents\\GitHub\\Diploma\\Data\\Wichita\\shape";
            //#endif
            folderBrowser.ShowDialog();

            if (String.IsNullOrEmpty(folderBrowser.SelectedPath))
                return;

            string mapPath = folderBrowser.SelectedPath;

            LoadMap(mapPath);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void LayerSettings_Click(object sender, EventArgs e)
        {
            if (_mapAroundMap == null)
                return;

            LayerSettingsForm settingsForm = new LayerSettingsForm(_mapAroundMap);
            settingsForm.OnLayerSettingsChanged += OnLayerSettingsChanged;
            settingsForm.Show();
        }

        private void OnLayerSettingsChanged(LayerBase layer)
        {
            MapAroundControl.RedrawMap();
        }

        private void GetCellMapButton_Click(object sender, EventArgs e)
        {
            BoundingRectangle rectangle = MapAroundControl.GetViewBox();
            double width = rectangle.Width;
            double height = rectangle.Height;

            double cellSize = 4E-5;
            MapAroundCellMap cellMap = new MapAroundCellMap(_mapAroundMap, rectangle, cellSize, cellSize);
            cellMap.AddPolygonObstaclesLayer((FeatureLayer)FindLayerByAlias("buildings"));
            //cellMap.AddPolygonObstaclesLayer((FeatureLayer)FindLayerByAlias("User region layer"));
            CellMapDrawerForm drawerForm = new CellMapDrawerForm(cellMap);
            drawerForm.Show();
        }

        private void HpaTestingButton_Click(object sender, EventArgs e)
        {
            HpaTestForm testForm = new HpaTestForm(null);
            testForm.Show();
        }

        #endregion

        #region Utils

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
            _initialRectangle = rectangle;
            if (rectangle.IsEmpty()) return; // Расчет  области данных карты

            // Поправка, для того, что бы вписать данные в контрол
            double deltaY = rectangle.Width * MapAroundControl.Height / 2 /
                            MapAroundControl.Width - rectangle.Height / 2;

            // Установка нового ViewBox                               
            MapAroundControl.SetViewBox(new BoundingRectangle(rectangle.MinX, rectangle.MinY - deltaY,
                rectangle.MaxX, rectangle.MaxY + deltaY));

            _mapAroundMap.FeatureRenderer.FlushTitles(MapAroundControl.CreateGraphics(), rectangle, 1);

            InitCellMap();
            InitPathLayer();
            InitHpaStar();
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
            InitUserPolygonLayer();
            SetViewBox();
        }

        

        private LayerBase FindLayerByAlias(string alias)
        {
            return _mapAroundMap.Layers.FirstOrDefault(l => l.Alias.Equals(alias));
        }

        #endregion

        private void TestUserYButton_Click(object sender, EventArgs e)
        {
            Feature polygonFeature = new Feature(FeatureType.Polygon);

            ICoordinate coordinate0 = _initialRectangle.Min;

            ICoordinate coordinate1 = ((ICoordinate) coordinate0.Clone());
            coordinate1.Translate(0, DefaultCellSize * 20);

            ICoordinate coordinate2 = ((ICoordinate) coordinate0.Clone());
            coordinate2.Translate(DefaultCellSize * 10, DefaultCellSize * 10);

            Polygon polygon = new Polygon(new []{coordinate0, coordinate1, coordinate2});
            polygonFeature.Polygon = polygon;

            _userRegionLayer.AddFeature(polygonFeature);
            RedrawMap();
        }

        public void Log(string message, int errorLevel)
        {
            Debug.WriteLine(message);
        }

        public void Log(Bitmap bmp)
        {
            BitmapDrawerForm drawerForm = CreateForm<BitmapDrawerForm>();
            ShowBitmapDrawer(drawerForm, bmp);
        }
    }
}
