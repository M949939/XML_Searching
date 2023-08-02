using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Diagnostics;
using System.Threading;


namespace ResultMesureSPC
{
    public partial class CheckupResult : Form
    {
        private static Mutex _mutex;
        private const string MutexName = "MyUniqueMutexNameForMyApp";
        private readonly Dictionary<TextBox, Button> _textBoxToButtonMapping = new Dictionary<TextBox, Button>();

        private readonly string _folderPath = @"C:\ZF\MesureSPC\TEMP";
        public CheckupResult()
        {
            bool isNewInstance;
            _mutex = new Mutex(true, MutexName, out isNewInstance);

            // If this is not a new instance and the condition is met
            if (!isNewInstance && SomeConditionMethod())
            {
                // Close previous instance
                ClosePreviousInstance();

                // Allow current instance to continue
                _mutex = new Mutex(true, MutexName);
            }
            XmlPhraseExtractor XMLPhraseExtractor = new XmlPhraseExtractor(_folderPath);
            if (XMLPhraseExtractor == null)
            {
                // Log or display an error about failing to create the XmlPhraseExtractor instance
                return;
            }

            string extractedvalue = XMLPhraseExtractor.ExtractInspectionPlanName();

            if (string.IsNullOrEmpty(extractedvalue))
            {
                // Log or display an error about failing to extract the inspection plan name
                return;
            }

            if (extractedvalue != "LIDAR_inspection_TPCB08" && extractedvalue != "LIDAR_inspection_TPCB09")
            {
                Environment.Exit(0);
            }

            // Now safely try to set button text from the XMLPhraseExtractor
            SetLabelText(XMLPhraseExtractor);

                      
            InitializeComponent();

            this.TopMost = true;
            InitializeMappings();
            this.Controls.Add(checkBox2);
            this.ActiveControl = checkBox2;
            foreach (var button in _textBoxToButtonMapping.Values)
                button.Click += Button_Click;
            this.StartPosition = FormStartPosition.Manual;
            this.Width = (int)(this.Width * 0.75);
            textBox1.Width = (int)(this.textBox1.Width * 0.75);
            textBox2.Width = (int)(this.textBox2.Width * 0.75);
            textBox3.Width = (int)(this.textBox3.Width * 0.75);
            textBox4.Width = (int)(this.textBox4.Width * 0.75);
            textBox5.Width = (int)(this.textBox5.Width * 0.75);
            label1.Text = "alpja";
            // Get the working area of the primary screen (considering taskbar and other docked UI elements)
            var screen = Screen.PrimaryScreen.WorkingArea;

            // Calculate the bottom-left position
            int xPosition = 0;
            int yPosition = screen.Height - this.Height;

            // Set the form position
            this.Location = new System.Drawing.Point(xPosition, yPosition);

            // Bring form to the front
            this.BringToFront();
        }
        private void SetLabelText(XmlPhraseExtractor extractor)
        {
            if (extractor == null) return;

            // Let's make sure button1 exists and is initialized before setting its text
            if (label1 != null)
            {
                string val1 = extractor.ExtractBarcode1().ToString();
                if (!string.IsNullOrEmpty(val1))
                {
                    label1.Text = val1;
                }
            }
        }
        private bool SomeConditionMethod()
        {
            XmlPhraseExtractor XMLPhraseExtractor = new XmlPhraseExtractor(_folderPath);
            string extractedvalue = XMLPhraseExtractor.ExtractInspectionPlanName();
            if (extractedvalue == "LIDAR_inspection_TPCB08" || extractedvalue == "LIDAR_inspection_TPCB09")
            { 
            return true; // example value.
            }
            else
            {
                Environment.Exit(0);
            }
            return true;
        }
        private static void ClosePreviousInstance()
        {
            foreach (var process in Process.GetProcesses())
            {
                if (process.ProcessName == Process.GetCurrentProcess().ProcessName && process.Id != Process.GetCurrentProcess().Id)
                {
                    process.Kill();
                    return;
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            var extractor = new XmlPhraseExtractor(_folderPath);
            ProcessXmlData(extractor);
            ExtractAllInfoFromXml(GetLatestXmlFile(_folderPath));
            UpdateButtonsBasedOnErrors();

        }





        #region XML

        public class Window
        {
            public string Id { get; set; }
            public string WinID { get; set; }
            public string WinType { get; set; }
            public string PartNumber { get; set; }
            public int PCBNumber { get; set; }
            public string SolderjointNumber { get; set; }
            public string Barcode { get; set; }
            public Analysis Analysis { get; set; }
            public Result Result { get; set; }
            public CADPosition CADPosition { get; set; }
        }

        public class Analysis
        {
            public string Mode { get; set; }
            public string SubMode { get; set; }
            public string WinNumber { get; set; }
            public string PictureFileName { get; set; }
        }

        public class Result
        {
            public int Type { get; set; }
            public int Number { get; set; }
            public string ErrorDescription { get; set; }
            public string GroupInformation { get; set; }
        }

        public class CADPosition
        {
            public int X { get; set; }
            public int Y { get; set; }
        }
        private string GetLatestXmlFile(string folderPath)
        {
            var dir = new DirectoryInfo(folderPath);
            var latestFile = dir.GetFiles("*.xml").OrderByDescending(f => f.LastWriteTime).FirstOrDefault();
            return latestFile?.FullName;
        }

        private void ProcessXmlData(XmlPhraseExtractor extractor)
        {
            var barcodes = extractor.ExtractBarcode();
            var numbers = extractor.ExtractNumber();
            var results = extractor.ExtractResult();

            int maxLength = results.Count;
            
            for (int i = 0; i < maxLength; i++)
            {
                var barcode = GetValueAtIndex(barcodes, i);
                var number = GetValueAtIndex(numbers, i);
                var result = GetValueAtIndex(results, i);

                if (!int.TryParse(number, out var numberInt))
                    Console.WriteLine($"Invalid number value at index {i}");
                if (!int.TryParse(barcode, out var barcodeInt))
                    Console.WriteLine($"Invalid Barcode value at index {i}");
                switch(i){

                    case 0: label1.Text = barcode.ToString();break;
                    case 1: label2.Text = barcode.ToString(); break;
                    case 2: label3.Text = barcode.ToString(); break;
                    case 3: label4.Text = barcode.ToString(); break;
                    case 4: label5.Text = barcode.ToString(); break;

                }
                SetConditionBasedOnResult(result, numberInt);
            }
        }

        private List<Window> ExtractAllInfoFromXml(string xmlFilePath)
        {
            var windowsList = new List<Window>();
            var xDoc = XDocument.Load(xmlFilePath);
            var windows = xDoc.Descendants("Window");
            foreach (var window in windows)
            {
                var windowObj = new Window
                {

                    Id = window.Attribute("id")?.Value,
                    WinID = window.Element("WinID")?.Value,
                    WinType = window.Element("WinType")?.Value,
                    PartNumber = window.Element("PartNumber")?.Value,
                    PCBNumber = int.TryParse(window.Element("PCBNumber")?.Value, out var pcbNumber) ? pcbNumber : 0,
                    SolderjointNumber = window.Element("SolderjointNumber")?.Value,
                    Barcode = window.Element("Barcode")?.Value,
                    Analysis = new Analysis
                    {
                        Mode = window.Element("Analysis")?.Element("Mode")?.Value,
                        SubMode = window.Element("Analysis")?.Element("SubMode")?.Value,
                        WinNumber = window.Element("Analysis")?.Element("WinNumber")?.Value,
                        PictureFileName = window.Element("Analysis")?.Element("PictureFileName")?.Value
                    },
                    Result = new Result
                    {
                        Type = int.TryParse(window.Element("Result")?.Element("Type")?.Value, out var type) ? type : 0,
                        Number = int.TryParse(window.Element("Result")?.Element("Number")?.Value, out var number) ? number : 0,
                        ErrorDescription = window.Element("Result")?.Element("ErrorDescription")?.Value,
                        GroupInformation = window.Element("Result")?.Element("GroupInformation")?.Value
                    },
                    CADPosition = new CADPosition
                    {
                        X = int.TryParse(window.Element("CADPosition")?.Element("X")?.Value, out var x) ? x : 0,
                        Y = int.TryParse(window.Element("CADPosition")?.Element("Y")?.Value, out var y) ? y : 0
                    }
                };

                windowsList.Add(windowObj);
            }

            return windowsList;
        }

        private class XmlPhraseExtractor
        {
            private readonly string _folderPath;
            public XmlPhraseExtractor(string folderPath)
            {
                _folderPath = folderPath;
            }

            public string ExtractInspectionPlanName() => ExtractSingleValue("InspectionPlanName", 0);

            public List<string> ExtractBarcode() => ExtractValues("Barcode", 0, 5);

            public string ExtractBarcode1() => ExtractSingleValue("Barcode", 0);
            public string ExtractBarcode2() => ExtractSingleValue("Barcode", 1);
            public string ExtractBarcode3() => ExtractSingleValue("Barcode", 2);
            public string ExtractBarcode4() => ExtractSingleValue("Barcode", 3);
            public string ExtractBarcode5() => ExtractSingleValue("Barcode", 4);

            public List<string> ExtractNumber() => ExtractValues("Number", 0, 5);

            public List<string> ExtractResult() => ExtractValues("Result", 1, 5);

            private List<string> ExtractValues(string elementName, int skip, int take)
            {
                var latestFile = GetLatestXmlFile();
                if (latestFile == null) return new List<string>();

                var xDoc = XDocument.Load(latestFile);
                return xDoc.Descendants(elementName).Skip(skip).Take(take).Select(x => x.Value).ToList();
            }

            private string ExtractSingleValue(string elementName, int skip)
            {
                var latestFile = GetLatestXmlFile();
                if (latestFile == null) return null;

                var xDoc = XDocument.Load(latestFile);
                var element = xDoc.Descendants(elementName).Skip(skip).FirstOrDefault();
                return element?.Value;
            }

            private string GetLatestXmlFile()
            {
                var dir = new DirectoryInfo(_folderPath);
                var latestFile = dir.GetFiles("*.xml").OrderByDescending(f => f.LastWriteTime).FirstOrDefault();
                return latestFile?.FullName;

            }
            
        }

        #endregion XML
        #region functions

        private void Button_Click(object sender, EventArgs e)
        {
            var clickedButton = sender as Button;

            // Determine the associated PCB based on button clicked
            int associatedPCB = _textBoxToButtonMapping.FirstOrDefault(x => x.Value == clickedButton).Key == textBox1 ? 1 :
                    _textBoxToButtonMapping.FirstOrDefault(x => x.Value == clickedButton).Key == textBox2 ? 2 :
                    _textBoxToButtonMapping.FirstOrDefault(x => x.Value == clickedButton).Key == textBox3 ? 3 :
                    _textBoxToButtonMapping.FirstOrDefault(x => x.Value == clickedButton).Key == textBox4 ? 4 :
                    _textBoxToButtonMapping.FirstOrDefault(x => x.Value == clickedButton).Key == textBox5 ? 5 : 0;

            var latestXmlFilePath = GetLatestXmlFile(_folderPath);
            if (string.IsNullOrEmpty(latestXmlFilePath))
            {
                MessageBox.Show("No XML files found in the directory.");
                return;
            }

            var windowsData = ExtractAllInfoFromXml(latestXmlFilePath);
            var errorForm = new ErrorInfoForm();
            errorForm.Text = $"Error Information PCB N°";

            var relevantWindows = windowsData.Where(w => w.PCBNumber == associatedPCB).ToList();
            Dictionary<string, string> errorDetails = new Dictionary<string, string>();

            int i = 0;
            foreach (var window in relevantWindows)
            {
                i++;
                string errorKey = $"Error {i}";
                errorForm.Text = $"Error Information PCB N°{associatedPCB} |Barcode:{window.Barcode}";

                string errorValue = $"WinType: {window.WinType}    \n" +
                                    $"SolderjointNumber: {window.SolderjointNumber}    \n" +
                                    $"Analysis WinNumber: {window.Analysis.WinNumber}    \n" +
                                    $"ErrorDescription: {window.Result.ErrorDescription.Replace(" ", "")}    \n" +
                                    $"CADPosition: X: {window.CADPosition.X} Y:{window.CADPosition.Y}";
                errorDetails.Add(errorKey, errorValue);
            }

            errorForm.AddErrorDetails(errorDetails);
            errorForm.Show();
        }

        private void UpdateButtonsBasedOnErrors()
        {
            var latestXmlFilePath = GetLatestXmlFile(_folderPath);
            if (string.IsNullOrEmpty(latestXmlFilePath))
            {
                // If there's no XML file, then disable all buttons
                foreach (var btn in _textBoxToButtonMapping.Values)
                {
                    btn.Enabled = false;
                }
                return;
            }

            var windowsData = ExtractAllInfoFromXml(latestXmlFilePath);

            // Enabling or disabling buttons based on the presence of data for each PCB
            for (int pcbNum = 1; pcbNum <= 5; pcbNum++)
            {
                var hasDataForPCB = windowsData.Any(w => w.PCBNumber == pcbNum);
                var correspondingButton = _textBoxToButtonMapping[GetTextBoxByPCBNumber(pcbNum)];
                correspondingButton.Enabled = hasDataForPCB;
            }
        }

        private TextBox GetTextBoxByPCBNumber(int pcbNum)
        {
            switch (pcbNum)
            {
                case 1: return textBox1;
                case 2: return textBox2;
                case 3: return textBox3;
                case 4: return textBox4;
                case 5: return textBox5;
                default: throw new ArgumentOutOfRangeException($"Invalid PCB Number: {pcbNum}");
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
                ChangeForeColor(textBox);
        }

        private void ChangeForeColor(TextBox textBox)
        {
            switch (textBox.Text)
            {
                case "PASS":
                    textBox.ForeColor = Color.Green;
                    break;
                case "FAIL":
                    textBox.ForeColor = Color.Red;
                    break;
                default:
                    textBox.ForeColor = Color.Black;
                    break;
            }
        }

        private Image LoadImageFromFolder(string folderPath, string imageName)
        {
            string imagePath = Path.Combine(folderPath, imageName + ".png");

            if (File.Exists(imagePath))
            {
                try
                {
                    return Image.FromFile(imagePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading image: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Image file not found: {imagePath}");
            }

            return null;
        }

        private string GetValueAtIndex(List<string> list, int index) => index < list.Count ? list[index] : string.Empty;

        private void SetConditionBasedOnResult(string result, int number)
        {
            var condition = result == "PASS";
            switch (number)
            {
                case 1: SetTextBoxTextAndLabelAndOverlay(condition, textBox1); break;
                case 2: SetTextBoxTextAndLabelAndOverlay(condition, textBox2); break;
                case 3: SetTextBoxTextAndLabelAndOverlay(condition, textBox3); break;
                case 4: SetTextBoxTextAndLabelAndOverlay(condition, textBox4); break;
                case 5: SetTextBoxTextAndLabelAndOverlay(condition, textBox5); break;
            }
        }

        private void SetTextBoxTextAndLabelAndOverlay(bool condition, TextBox textBox)
        {
            textBox.Text = condition ? "PASS" : "FAIL";
            ChangeForeColor(textBox);

        }


        private void InitializeMappings()
        {
            _textBoxToButtonMapping[textBox1] = button1;
            _textBoxToButtonMapping[textBox2] = button2;
            _textBoxToButtonMapping[textBox3] = button3;
            _textBoxToButtonMapping[textBox4] = button4;
            _textBoxToButtonMapping[textBox5] = button5;
        }

        #endregion functions
    }
}

