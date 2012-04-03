using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using Dataweb.NShape.GeneralShapes;
using Dataweb.NShape.Layouters;


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
				project1.Name = "WebVisits Project";
				// Add path to the NShape shape library assemblies to the search paths
				string programFilesDir = Environment.GetEnvironmentVariable(string.Format("ProgramFiles{0}", (IntPtr.Size == sizeof(long)) ? "(x86)" : ""));
				project1.LibrarySearchPaths.Add(Path.Combine(programFilesDir, string.Format("dataweb{0}NShape{0}bin", Path.DirectorySeparatorChar)));
				
				// Add general shapes library
				project1.AddLibrary(typeof(Ellipse).Assembly, false);

				// Create a new NShape project
				project1.AutoGenerateTemplates = false;
				project1.Create();

				// Define the arrow style
				((CapStyle)project1.Design.CapStyles.ClosedArrow).CapSize = 20;
				((CapStyle)project1.Design.CapStyles.ClosedArrow).CapShape = CapShape.ClosedArrow;
				((CapStyle)project1.Design.CapStyles.ClosedArrow).ColorStyle = project1.Design.ColorStyles.Green;
				project1.Repository.Update(project1.Design.CapStyles.ClosedArrow);

				// Create a template for web pages
				Template webPageTemplate = new Template("Web Page", project1.ShapeTypes["Ellipse"].CreateInstance());
				((IPlanarShape)webPageTemplate.Shape).FillStyle = project1.Design.FillStyles.Green;
				project1.Repository.InsertAll(webPageTemplate);

				// Create a template for annotations
				Template textTemplate = new Template("Text", project1.ShapeTypes["Text"].CreateInstance());
				((ICaptionedShape)textTemplate.Shape).SetCaptionCharacterStyle(0, project1.Design.CharacterStyles.Normal);
				project1.Repository.InsertAll(textTemplate);

				// Create a template for headings
				Template headingTemplate = new Template("Heading", project1.ShapeTypes["Text"].CreateInstance());
				((ICaptionedShape)headingTemplate.Shape).SetCaptionCharacterStyle(0, project1.Design.CharacterStyles.Heading1);
				project1.Repository.InsertAll(headingTemplate);

				// We do not want a tool for the web page template.
				toolSetController1.DeleteTool(toolSetController1.FindTool(webPageTemplate));

				// Display a diagram and enable editing
				display1.Diagram = new Diagram("");
				display1.CurrentTool = new SelectionTool();
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
			Template webPageTemplate = project1.Repository.GetTemplate("Web Page");

			int x = 10;
			int y = 500;
			string statisticsFilePath = Path.Combine(GetBasicTutorialPath(), @"Sample Data\Small.txt");
			TextReader reader = new StreamReader(statisticsFilePath);
			string line = reader.ReadLine();
			while (line != null) {
				int idx1 = line.IndexOf(';');
				int idx2 = line.IndexOf(';', idx1 + 1);
				string url = line.Substring(0, idx1);
				RectangleBase referringShape;
				if (!shapeDict.TryGetValue(url, out referringShape)) {
					referringShape = (RectangleBase)webPageTemplate.CreateShape();
					referringShape.Width = 100;
					referringShape.Height = 60;
					referringShape.X = x + 50;
					referringShape.Y = y + 50;
					x += 120;
					if (x > 1200) {
						x = 10;
						y += 70;
					}
					referringShape.SetCaptionText(0, Path.GetFileNameWithoutExtension(url));
					shapeDict.Add(url, referringShape);
					diagram.Shapes.Add(referringShape);
				}
				url = line.Substring(idx1 + 1, idx2 - idx1 - 1);
				RectangleBase referredShape;
				if (!shapeDict.TryGetValue(url, out referredShape)) {
					referredShape = (RectangleBase)webPageTemplate.CreateShape();
					referredShape.Width = 100;
					referredShape.Height = 60;
					referredShape.X = x + 50;
					referredShape.Y = y + 50;
					x += 120;
					if (x > 1200) {
						x = 10;
						y += 70;
					}
					referredShape.SetCaptionText(0, Path.GetFileNameWithoutExtension(url));
					shapeDict.Add(url, referredShape);
					diagram.Shapes.Add(referredShape);
				}
				// Add the connection
				Polyline arrow = (Polyline)project1.ShapeTypes["Polyline"].CreateInstance();
				diagram.Shapes.Add(arrow);
				arrow.EndCapStyle = project1.Design.CapStyles.ClosedArrow; 
				arrow.Connect(ControlPointId.FirstVertex, referringShape, ControlPointId.Reference);
				arrow.Connect(ControlPointId.LastVertex, referredShape, ControlPointId.Reference);
				//
				// Next line
				line = reader.ReadLine();
			}
			reader.Close();
			cachedRepository1.InsertAll(diagram);
			display1.Diagram = diagram;
		}


		private void fileLayoutToolStripMenuItem_Click(object sender, EventArgs e) {
			foreach (Shape s in display1.Diagram.Shapes) {
				s.X = 100;
				s.Y = 100;
			}
			RepulsionLayouter layouter = new RepulsionLayouter(project1);
			layouter.SpringRate = 8;
			layouter.Repulsion = 3;
			layouter.RepulsionRange = 500;
			layouter.Friction = 0;
			layouter.Mass = 50;
			layouter.AllShapes = display1.Diagram.Shapes;
			layouter.Shapes = display1.Diagram.Shapes;
			//
			layouter.Prepare();
			layouter.Execute(10);
			layouter.Fit(50, 50, display1.Diagram.Width - 100, display1.Diagram.Height - 100);
		}

	}

}