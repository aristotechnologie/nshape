using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Dataweb.NShape;
using Dataweb.NShape.Advanced;


namespace BasicTutorial {

	public partial class Form1 : Form {

		private static string GetBasicTutorialPath() {
			string appDir = Path.GetDirectoryName(Path.GetDirectoryName(Application.StartupPath));
			return Path.GetFullPath(Path.Combine(appDir, @"Demo Programs\Tutorials\Basic"));
		}


		public Form1() {
			InitializeComponent();
		}


		private void Form1_Load(object sender, EventArgs e) {
			try {
				// Set path to the sample diagram and the sample diagram name
				string dir = Path.Combine(GetBasicTutorialPath(), "Sample Project");
				xmlStore1.DirectoryName = dir;
				project1.Name = "Circles";
				// Add path to the NShape shape library assemblies to the search paths
				string programFilesDir = Environment.GetEnvironmentVariable(string.Format("ProgramFiles{0}", (IntPtr.Size == sizeof(long)) ? "(x86)" : ""));
				project1.LibrarySearchPaths.Add(Path.Combine(programFilesDir, string.Format("dataweb{0}NShape{0}bin", Path.DirectorySeparatorChar)));
				project1.AutoLoadLibraries = true;
				
				// Open the NShape project
				project1.Open();
				
				// Load the diagram
				display1.LoadDiagram("Diagram 1");
			} catch (Exception exc) {
				MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}


		private void fileSaveToolStripMenuItem_Click(object sender, EventArgs e) {
			project1.Repository.SaveChanges();
		}


		private void fileLoadStatisticsToolStripMenuItem_Click(object sender, EventArgs e) {
			// Delete all diagrams (with shapes)
			List<Diagram> diagrams = new List<Diagram>(project1.Repository.GetDiagrams());
			for (int i = diagrams.Count - 1; i >= 0; --i)
				project1.Repository.DeleteAll(diagrams[i]);

			Dictionary<string, RectangleBase> shapeDict = new Dictionary<string, RectangleBase>(1000);
			Diagram diagram = new Diagram("D1");

			int x = 10;
			int y = 500;
			string statisticsFilePath = Path.Combine(GetBasicTutorialPath(), @"Sample Data\Small.txt");
			TextReader reader = new StreamReader(statisticsFilePath);
			string line = reader.ReadLine();
			while (line != null) {
				int idx1 = line.IndexOf(';');
				int idx2 = line.IndexOf(';', idx1 + 1);
				RectangleBase shape;
				string url = line.Substring(0, idx1);
				if (!shapeDict.TryGetValue(url, out shape)) {
					shape = (RectangleBase)project1.ShapeTypes["Ellipse"].CreateInstance();
					shape.Width = 100;
					shape.Height = 60;
					shape.X = x + 50;
					shape.Y = y + 50;
					x += 120;
					if (x > 1200) {
						x = 10;
						y += 70;
					}
					shape.SetCaptionText(0, Path.GetFileNameWithoutExtension(url));
					shapeDict.Add(url, shape);
					diagram.Shapes.Add(shape);
				}
				url = line.Substring(idx1 + 1, idx2 - idx1 - 1);
				if (!shapeDict.TryGetValue(url, out shape)) {
					shape = (RectangleBase)project1.ShapeTypes["Ellipse"].CreateInstance();
					shape.Width = 100;
					shape.Height = 60;
					shape.X = x + 50;
					shape.Y = y + 50;
					x += 120;
					if (x > 1200) {
						x = 10;
						y += 70;
					}
					shape.SetCaptionText(0, Path.GetFileNameWithoutExtension(url));
					shapeDict.Add(url, shape);
					diagram.Shapes.Add(shape);
				}
				line = reader.ReadLine();
			}
			reader.Close();
			cachedRepository1.InsertAll(diagram);
			
			display1.Diagram = diagram;
		}

	}

}