using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_task
{
    class coordinates
    {
        // public int place_id { get; set; }
        // public string licence { get; set; }
        // public string osm_type { get; set; }
        // public int osm_id { get; set; }
        // public string[] boundingbox { get; set; }
        //  public string lat { get; set; }
        //  public string lon { get; set; }
        //  public string display_name { get; set; }
        //  public string type { get; set; }
        //  public float importance { get; set; }
        //  public string icon { get; set; }
        public Geojson geojson { get; set; }

        public class Geojson
        {
            public string type { get; set; }
            public float[][][][] coordinates { get; set; }
            
        }

    }




}



