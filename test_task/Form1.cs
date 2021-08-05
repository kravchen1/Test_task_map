using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;



using System.Net;
using System.Text.Json;
using Newtonsoft.Json;

namespace test_task
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void gMapControl1_Load(object sender, EventArgs e)
        {

            gmap.Bearing = 0;
            gmap.GrayScaleMode = true;
            gmap.MarkersEnabled = true;
            gmap.MaxZoom = 20;
            gmap.MinZoom = 2;
            gmap.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
            gmap.NegativeMode = false;
            gmap.PolygonsEnabled = true;
            gmap.ShowTileGridLines = false;
            gmap.ShowCenter = false;
            gmap.Zoom = 10;

            gmap.MapProvider = GMap.NET.MapProviders.GMapProviders.GoogleMap;

            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gmap.Position = new GMap.NET.PointLatLng(47.22, 38.84);
          

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            try
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    if(textBox3.Text != "")
                    {
                        dialog.FileName = textBox3.Text;
                    }
                    else
                    {
                        dialog.FileName = "Polygons";
                    }
                    dialog.Filter = "PNG (*.png)|*.png";
                    
                    Image image = this.gmap.ToImage();

                    if(image != null)
                    {
                        using(image)
                        {
                            if(dialog.ShowDialog() == DialogResult.OK)
                            {
                                string fileName = dialog.FileName;
                                if(!fileName.EndsWith(".png",
                                        StringComparison.OrdinalIgnoreCase))
                                {
                                    fileName += ".png";
                                }
                                image.Save(fileName);
                                MessageBox.Show("Карта сохранена в директории: "
                                    + Environment.NewLine
                                    + dialog.FileName, "GMap.Net",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Asterisk);
                            }
                        }
                    }
                }
            }
            catch(Exception exception)
            {
                MessageBox.Show("Ошибка при сохранении карты: "
                    + Environment.NewLine
                    + exception.Message,
                    "GMap.NET",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);
            }
            
            
            
          
        }

        private void button2_Click(object sender, EventArgs e)
        {

            WebClient client = new WebClient { Encoding = Encoding.UTF8 };
            client.Headers["User-Agent"] = "Mozilla/5.0";
            string url;
            if (textBox1.Text == "")
            {
                MessageBox.Show("Пожалуйста укажите адрес "
                                    + Environment.NewLine,
                                    "GMap.Net",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Asterisk);
                return;
            }
            else
            {
                url = "https://nominatim.openstreetmap.org/search?q=" + textBox1.Text + "&format=json&polygon_geojson=1";
            }
            int count_point_skip = 1;
            if (textBox2.Text != "")
            {
                try
                {
                    count_point_skip = Convert.ToInt32(textBox2.Text);
                }
                catch
                {
                    MessageBox.Show("В поле количество точек введены некорректные данные "
                                    + Environment.NewLine,
                                    "GMap.Net",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Asterisk);
                    return;
                }
            }
            string strPageCode = client.DownloadString(url);
            strPageCode = strPageCode.Substring(1);
            strPageCode = strPageCode.Substring(0, strPageCode.Length - 1);
            coordinates data = JsonConvert.DeserializeObject<coordinates>(strPageCode);


            gmap.Position = new GMap.NET.PointLatLng(data.geojson.coordinates[0][0][0][0], data.geojson.coordinates[0][0][0][1]);


            GMapOverlay PolygonOverlay = new GMapOverlay("polygons");
            List<PointLatLng> points = new List<PointLatLng>();
            List<GMapPolygon> polygon = new List<GMapPolygon>();

            if (count_point_skip == 0) { count_point_skip = 1; }
            for (int i = 0; i < data.geojson.coordinates.Length; ++i)
            {
                for (int j = 0; j < data.geojson.coordinates[i][0].Length; j += count_point_skip)
                {
                    points.Add(new PointLatLng(data.geojson.coordinates[i][0][j][0], data.geojson.coordinates[i][0][j][1]));
                }
                polygon.Add(new GMapPolygon(points, i.ToString()));
                polygon.Last().Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
                polygon.Last().Stroke = new Pen(Color.Blue, 1);
                points.Clear();
            }

            foreach (var p in polygon)
            {
                PolygonOverlay.Polygons.Add(p);
            }
            gmap.Overlays.Add(PolygonOverlay);
            foreach (var p in polygon)
            {
                gmap.UpdatePolygonLocalPosition(p);
            }


        }
    }
}
