using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using System.IO.Packaging;
using System.Data.SqlClient;
using Microsoft.Msagl.GraphViewerGdi;
using System.Windows.Forms;

namespace AdventureWorksDiagram
{
        internal delegate void MD();

        public partial class Form1 : Form
        {
            Graph graph;
            StatusStrip statusStrip = new StatusStrip();
            ToolTip tt = new ToolTip();

            public Form1()
            {
                InitializeComponent();
                CreateWideGraph();
                tt.SetToolTip(aspectRatioUpDown, "Aspect ratio of the layout");
                SuspendLayout();
                ToolStripItem toolStripLabel = new ToolStripStatusLabel();
                statusStrip.Items.Add(toolStripLabel);
                Controls.Add(statusStrip);
                ResumeLayout();
                
                viewer.MouseMove += GViewerMouseMove;
            }

        void GViewerMouseMove(object sender, MouseEventArgs e)
        {
            float viewerX;
            float viewerY;

            viewer.ScreenToSource(e.Location.X, e.Location.Y, out viewerX, out viewerY);

            // Find the object (edge) under the mouse cursor
            object obj = viewer.GetObjectAt(e.Location);
            string str = "";
            if (obj is IViewerEdge edge) // Check if the object is an edge
            {
                // Get the source and target nodes of the edge
                string sourceNode = edge.Edge.SourceNode.UserData.ToString();
                string targetNode = edge.Edge.TargetNode.UserData.ToString();
                 str =$"each {sourceNode} may have multiple { targetNode}";
                // Show tooltip with the names of the source and target nodes
            
                viewer.SetToolTip(tt, str);
                tt.SetToolTip(viewer, $"{sourceNode} -> {targetNode}");
               // edge.Edge.Label.IsVisible = true;
            }
            else
            {
                // Clear tooltip if not hovering over an edge
                tt.SetToolTip(viewer, null);
            }


            foreach (var item in statusStrip.Items)
            {
                var label = item as ToolStripStatusLabel;
                if (label != null)
                {
                    label.Text = str;
                    return;
                }
            }
        }

        void Relayout()
            {
                SetGraphParams();
                viewer.Graph = graph;
            }


        void CreateWideGraph()
        {
            graph = new Graph();

        string connectionString = "Server=DESKTOP-E0FAPSB;Database=AdventureWorks2022;Integrated Security=true;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    SqlCommand tablesCommand = new SqlCommand("SELECT t.TABLE_NAME, k.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLES t JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE k ON t.TABLE_NAME = k.TABLE_NAME WHERE t.TABLE_TYPE = 'BASE TABLE'", connection);
                    using (SqlDataReader reader = tablesCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string tableName = reader.GetString(0);
                            string primaryKey = reader.GetString(1);

                            // Add a node for the table with the table name


                            var tableNode = graph.AddNode(tableName);
                            tableNode.UserData = tableName;
                            tableNode.Label.Text = tableName;
                            tableNode.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightBlue;
                            tableNode.Label.FontSize = 1000;
                            tableNode.Label.FontName = "Arial";
                            tableNode.Label.Width = 100;
                            tableNode.Label.Height = 40;
                            tableNode.Label.FontColor = Microsoft.Msagl.Drawing.Color.Black;

                            // Add the primary key as a label to the node
                            tableNode.LabelText += "\n" + primaryKey;
                        }
                    }

                    SqlCommand foreignKeysCommand = new SqlCommand("SELECT OBJECT_NAME(parent_object_id) AS SourceTable, OBJECT_NAME(referenced_object_id) AS TargetTable, name AS ForeignKeyName FROM sys.foreign_keys", connection);
                    using (SqlDataReader foreignKeysReader = foreignKeysCommand.ExecuteReader())
                    {
                        while (foreignKeysReader.Read())
                        {
                            string sourceTable = foreignKeysReader.GetString(0);
                            string targetTable = foreignKeysReader.GetString(1);
                            string foreignKeyName = foreignKeysReader.GetString(2);
                           // string label = $"each {sourceTable} may have multiple {targetTable} ";

                            var edge = graph.AddEdge(sourceTable, targetTable);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }
           
        

            
        

            void gViewer1_Load(object sender, EventArgs e)
            {
                SetGraphParams();
                viewer.Graph = graph;
            }

            void SetGraphParams()
            {
                graph.Attr.AspectRatio = (double)aspectRatioUpDown.Value;
                graph.Attr.SimpleStretch = simpleStretchCheckBox.Checked;
                graph.Attr.MinimalWidth = (double)MinWidth.Value;
                graph.Attr.MinimalHeight = (double)MinHeight.Value;
            }

            private void button1_Click(object sender, EventArgs e)
            {
                Relayout();
            }
        }
    
}