using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        string extension;
        string ext = "None";
        List<string> split;
        Stopwatch stopwatch = new Stopwatch();
        Stopwatch WatchForSize = new Stopwatch();
        Stopwatch watchFordate = new Stopwatch();
        string SizeByExt;
        string time;
        BackgroundWorker BGWorker = new BackgroundWorker();
        int DuplicatesNumber = 0;
        long JPGSize = 0;
        string TotalSizeAllFiles;
        Dictionary<string, long> FilesSize = new Dictionary<string, long>();
        Dictionary<string, string> Duplicates = new Dictionary<string, string>();
        long SizeofDuplicates = 0;
        List<PictureBox> PictureboxSet = new List<PictureBox>();
        Size size;
        List<CheckBox> CheckboxSet = new List<CheckBox>();
        //int SelectedIndex;

        public Form1()
        {
            InitializeComponent();
            InitializeBackgroundWorker(BGWorker);
            buttonCancel.Enabled = false;
            buttonDelete.Enabled = false;
            buttonSelectAll.Enabled = false;
            buttonselectNone.Enabled = false;
            CenterToScreen();
            string[] FormatOptions = new string[] { "All files (*)", "All Picture Files", "Bitmap files (*.bmp)", "JPEG (*.jpg, *.jpeg)", "GIF (*.gif)", "PNG (*.png)", "MP3 (*.mp3)", "DOC (*.doc, *.docs)" };
            comboBox1.Items.AddRange(FormatOptions);

            textBoxForInfo.Enabled = false;
            textBoxDuplicatesInfo.Enabled = false;
            //textBoxForInfo.ReadOnly = true;
            //textBoxDuplicatesInfo.ReadOnly = true;
            comboBox1.SelectedIndex = 0;
            buttonNext.Enabled = false;
            buttonPrevious.Enabled = false;
        }

        private BackgroundWorker InitializeBackgroundWorker(BackgroundWorker BGWorker)
        {
            BGWorker.DoWork += new DoWorkEventHandler(backgroundWorkerFiles_DoWork);
            BGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorkerFiles_RunWorkerCompleted);
            BGWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorkerFiles_ProgressChanged);
            BGWorker.WorkerReportsProgress = true;
            BGWorker.WorkerSupportsCancellation = true;
            return BGWorker;
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            textBoxForInfo.Clear();
            textBoxDuplicatesInfo.Clear();
            textBoxForInfo.BackColor = Color.White;
            textBoxDuplicatesInfo.BackColor = Color.White;
            CheckedListBoxForResult.Items.Clear();
            Duplicates.Clear();
            FilesSize.Clear();
            textBoxForPath.BackColor = Color.White;
            List<string> ListOfParameters;
            string NewPath;

            //listBox1.DataSource = null;
            listBox1.Items.Clear();
            ListOfParameters = new List<string>();

            NewPath = textBoxForPath.Text;
            buttonNext.Enabled = false;
            buttonPrevious.Enabled = false;
            //if (listBox1.Items.Count > 1)
            //{
            //    buttonNext.Enabled = false;
            //    buttonPrevious.Enabled = false;
            //}
            if (PictureboxSet.Count > 0)
            {
                foreach (PictureBox pb in PictureboxSet)
                {
                    pb.Image.Dispose();
                }
                PictureboxSet.Clear();
                listBox1.Items.Clear();
                splitContainer1.Panel2.Controls.Clear();
            }
            textBoxForInfo.Enabled = false;
            textBoxDuplicatesInfo.Enabled = false;

            if (string.IsNullOrWhiteSpace(NewPath) || NewPath.StartsWith(" ") || !Directory.Exists(NewPath))
            {
                textBoxForPath.BackColor = Color.Red;

                if (!Directory.Exists(NewPath))
                {
                MessageBox.Show("The path is invalid. Please specify the path to the folder.");
                }
            }
            else
            {
                buttonGetInfo.Enabled = false;
                buttonBrowseFolder.Enabled = false;

                comboBox1.Enabled = false;
                ListOfParameters.Add(NewPath);
                if (ext == "None")
                {
                    MessageBox.Show("Please select File extension!");
                    buttonGetInfo.Enabled = true;
                    comboBox1.Enabled = true;
                }
                else
                {
                    label_Percentage.Text = "Please wait. The files are processing.";
                    ListOfParameters.AddRange(split);
                    BGWorker.RunWorkerAsync(ListOfParameters);
                    buttonCancel.Enabled = true;
                }
                buttonDelete.Enabled = false;
                buttonSelectAll.Enabled = false;
                buttonselectNone.Enabled = false;
                textBoxForPath.Enabled = false;
            }
        }

        private void backgroundWorkerFiles_DoWork(object sender, DoWorkEventArgs e)
        {
            string Extension;
            string MainPath;
            List<string> FolderPathes;
            FileInfo[] ArrayOfFiles;
            Dictionary<string, string> DuplicatedImageBySize;
            Dictionary<string, string> DuplicatedNonImage;
            List<string> ListOfFullPathes;
            string PathToFile;
            string[] StringSeparators;
            List<string> NonImageFiles;
            List<string> ImageJPG;
            List<string> ImageFiles;
            List<string> ImageByDateDuplicates;
            long ImageFilesSize;
            long NonImageFilesSize;
            double SpentTime;
            TimeSpan ts;
            List<string> DuplicatesByName;

            FolderPathes = new List<string>();
            DuplicatedImageBySize = new Dictionary<string, string>();
            DuplicatedNonImage = new Dictionary<string, string>();
            ListOfFullPathes = new List<string>();
            NonImageFiles = new List<string>();
            ImageJPG = new List<string>();
            ImageFiles = new List<string>();
            ImageByDateDuplicates = new List<string>();
            DuplicatesByName = new List<string>();
            ImageFilesSize = 0;
            NonImageFilesSize = 0;
            SizeByExt = "Type(s) of files: ";
            StringSeparators = new string[] { " - copy", " - копия", " (", "(" };

            List<string> parameters = (List<string>)e.Argument;
            MainPath = parameters[0];
            parameters.RemoveAt(0);
            
            stopwatch.Restart();
            FolderPathes = ProcessDirectory(MainPath);
            FolderPathes.Add(MainPath);

            foreach (string subfolder in FolderPathes)
            {
                DirectoryInfo Subdirectory = new DirectoryInfo(subfolder);
                foreach (string param in parameters)
                {
                    if (BGWorker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }
                    else
                    {
                        ArrayOfFiles = Subdirectory.GetFiles(param);
                        foreach (FileInfo file in ArrayOfFiles)
                        {
                            PathToFile = Path.Combine(Subdirectory.ToString(), file.ToString());
                            ListOfFullPathes.Add(PathToFile);
                        }
                    }
                }
            }

            if (ListOfFullPathes.Count == 0)
            {
                e.Result = "N/A";
            }
            else
            {
                for (int x = 0; x < ListOfFullPathes.Count; x++)
                {
                    Extension = Path.GetExtension(ListOfFullPathes[x]).ToLower();
                    if (BGWorker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }
                    else if (Extension == ".jpg" || Extension == ".jpeg")
                    {
                        ImageJPG.Add(ListOfFullPathes[x]);
                    }
                    else if (Extension == ".bmp" || Extension == ".gif" || Extension == ".png")
                    {
                        ImageFiles.Add(ListOfFullPathes[x]);
                    }
                    else
                    {
                        NonImageFiles.Add(ListOfFullPathes[x]);
                    }
                }
            
                if (ImageJPG.Count != 0)
                {
                    watchFordate.Restart();

                    DuplicatesByName = GetDuplicatesByName(ImageJPG, e);
                    Duplicates = GetDateDuplicates(DuplicatesByName, e, out ImageByDateDuplicates);
                    watchFordate.Stop();
                    DuplicatesByName.Clear();
                }
                ImageFiles.AddRange(ImageByDateDuplicates);

                if (ImageFiles.Count != 0)
                {
                    WatchForSize.Restart();
                    DuplicatesByName = GetDuplicatesByName(ImageFiles, e);
                    DuplicatedImageBySize = GetSizeDuplicates(DuplicatesByName, e, out ImageFilesSize);
                    WatchForSize.Stop();
                    DuplicatesByName.Clear();

                    if (ImageFiles.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> kvp in DuplicatedImageBySize)
                        {
                            Duplicates.Add(kvp.Key, kvp.Value);
                        }
                    }
                }
                if (ImageFiles.Count != 0 || ImageJPG.Count != 0)
                {
                    SizeByExt += "Images, Size: " + GetSize((double)(ImageFilesSize + JPGSize));
                }

                if (NonImageFiles.Count != 0)
                {
                    WatchForSize.Start();

                    DuplicatesByName = GetDuplicatesByName(NonImageFiles, e);
                    DuplicatedNonImage = GetSizeDuplicates(DuplicatesByName, e, out NonImageFilesSize);
                    WatchForSize.Stop();
                    DuplicatesByName.Clear();

                    if (DuplicatedNonImage.Count > 0)
                    {
                    foreach (KeyValuePair<string, string> kvp in DuplicatedNonImage)
                    {
                        Duplicates.Add(kvp.Key, kvp.Value);
                    }
                        SizeByExt += " Non-images, Size: " + GetSize((double)NonImageFilesSize);
                    }
                }
                TotalSizeAllFiles = GetSize((double)(JPGSize + ImageFilesSize + NonImageFilesSize));

                if (Duplicates.Count == 0)
                {
                    e.Result = "No duplicates";
                }
                else
                {
                    foreach (KeyValuePair<string, string> kvp in Duplicates)
                    {
                        BGWorker.ReportProgress(0, kvp.Key);
                        //Duplicates.Add(kvp.Key, kvp.Value);
                    }
                }
            stopwatch.Stop();
            SpentTime = stopwatch.ElapsedMilliseconds;
            ts = TimeSpan.FromMilliseconds(SpentTime);
            time = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            }
        }

        public Dictionary<string, string> GetSizeDuplicates(List<string> AllFilesList, DoWorkEventArgs e, out long OtherImageSize)
        {
            long FileLength;
            List<float> LengthList;
            int LengthIndex;
            int RoundedPercent;
            int CountFiles;
            int ProcessedPercentage;
            Dictionary<string, string> FilesDup;
            string DictionaryKey;
            string DictionaryValue;

            LengthList = new List<float>();
            FilesDup = new Dictionary<string, string>();
            OtherImageSize = 0;
            CountFiles = 0;
            ProcessedPercentage = 0;

            RoundedPercent = AllFilesList.Count / 100;

            for (int a = 0; a < AllFilesList.Count; a++)
            {
                if (BGWorker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    CountFiles++;
                    if (CountFiles == RoundedPercent)
                    {
                        ProcessedPercentage = ProcessedPercentage + 1;
                        if (ProcessedPercentage > 100)
                        {
                            ProcessedPercentage = 100;
                        }
                        BGWorker.ReportProgress(ProcessedPercentage, null);
                        CountFiles = 0;
                    }
                    FileLength = new FileInfo(AllFilesList[a]).Length;
                    
                    LengthIndex = LengthList.IndexOf(FileLength);

                    if (LengthIndex != -1)
                    {
                        DictionaryKey = AllFilesList[LengthIndex];
                        if (!FilesDup.ContainsKey(DictionaryKey))
                        {
                            FilesDup.Add(DictionaryKey, AllFilesList[a]);
                        }
                        else
                        {
                            FilesDup.TryGetValue(DictionaryKey, out DictionaryValue);
                            DictionaryValue += ";" + AllFilesList[a];
                            FilesDup[DictionaryKey] = DictionaryValue;
                        }
                        FilesSize.Add(AllFilesList[a], FileLength);
                        OtherImageSize += FileLength;
                        DuplicatesNumber++;
                    }
                    LengthList.Add(FileLength);
                }
            }
            return FilesDup;
        }

        public List<string> GetDuplicatesByName(List<string> ListOfFiles, DoWorkEventArgs e)
        {
            string NameWithoutExt;
            string[] stringSeparators = new string[] { " - copy", " - копия", " (", "(" };
            string[] Split;
            List<string> ListOfFileName;

            ListOfFileName = new List<string>();
            foreach (string s in ListOfFiles)
            {
                NameWithoutExt = Path.GetFileNameWithoutExtension(s).ToLower();
                if (BGWorker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else if (NameWithoutExt.ToString().ToLower().Contains(" - copy") || NameWithoutExt.ToString().ToLower().Contains(" - копия") || NameWithoutExt.ToString().ToLower().Contains(" (") || NameWithoutExt.ToString().ToLower().Contains("("))
                {
                    Split = NameWithoutExt.ToString().ToLower().Split(stringSeparators, StringSplitOptions.None);
                    ListOfFileName.Add(Split[0]);
                }
                else
                {
                    ListOfFileName.Add(NameWithoutExt);
                }
            }

            string variable;
            for (int i = ListOfFileName.Count - 1; i >= 0; i--)
            {
                variable = ListOfFileName[i];
                ListOfFileName[i] = null;
                if (!ListOfFileName.Contains(variable))
                {
                    ListOfFiles.RemoveAt(i);
                }
                ListOfFileName[i] = variable;
            }
            return ListOfFiles;
        }


        public Dictionary<string, string> GetDateDuplicates(List<string> ListToAllFiles, DoWorkEventArgs e, out List<string> listfiles)
        {
            List<string> CreationDate;
            int PercentageRound;
            int DateIndex;
            int Filecount;
            int Processedpercent;
            FileStream fs;
            BitmapSource img;
            BitmapMetadata md;
            string CameraModel;
            long ItemSize;
            string Key;
            string Value;
            Dictionary<string, string> FilesDuplicates;
            string Date;

            CreationDate = new List<string>();
            listfiles = new List<string>();
            FilesDuplicates = new Dictionary<string, string>();
            Filecount = 0;
            Processedpercent = 0;

            PercentageRound = ListToAllFiles.Count / 100;
            
            for (int a = 0; a < ListToAllFiles.Count; a++)
            {
                if (BGWorker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    Filecount++;

                    if (Filecount == PercentageRound)
                    {
                        Processedpercent = Processedpercent + 1;
                        if (Processedpercent > 100)
                        {
                            Processedpercent = 100;
                        }
                        BGWorker.ReportProgress(Processedpercent, null);
                        Filecount = 0;
                    }
                        using (fs = new FileStream(ListToAllFiles[a], FileMode.Open))   
                        {
                            img = BitmapFrame.Create(fs);
                            md = (BitmapMetadata)img.Metadata;
                            CameraModel = md.CameraModel;
                            Date = md.DateTaken;                            
                        }
                        if (Date != null && CameraModel != null)
                        {
                            DateIndex = CreationDate.IndexOf(Date);

                            if (DateIndex != -1)
                            {
                                Key = ListToAllFiles[DateIndex];
                                ItemSize = new FileInfo(ListToAllFiles[a]).Length;

                                if (!FilesDuplicates.ContainsKey(Key))
                                {
                                    FilesDuplicates.Add(Key, ListToAllFiles[a]);
                                }
                                else
                                {
                                    FilesDuplicates.TryGetValue(Key, out Value);
                                    Value += ";" + ListToAllFiles[a];
                                    FilesDuplicates[Key] = Value;
                                }
                                FilesSize.Add(ListToAllFiles[a], ItemSize);
                                JPGSize += ItemSize;
                                DuplicatesNumber++;
                            }
                        }
                        else
                        {
                            listfiles.Add(ListToAllFiles[a]);
                        }
                    CreationDate.Add(Date);
                }
            }
            return FilesDuplicates;
        }

        public List<string> ProcessDirectory(string path)
        {
            try
            {
                return Directory.GetDirectories(path, "*", SearchOption.AllDirectories).ToList();
            }
            catch
            {
                //MessageBox.Show("!!!!");
                return null;
            }
        }

        public string GetSize(double TotalSize)
        {
            double ByteEquivalent;
            ByteEquivalent = 1024;
            string LengthType;
            double GigaByte = Math.Pow(ByteEquivalent, 3);
            double MegaByte = Math.Pow(ByteEquivalent, 2);

            if (TotalSize >= GigaByte)
            {
                TotalSize = TotalSize / GigaByte;
                LengthType = TotalSize.ToString("F") + " GB";
            }
            else if (TotalSize >= MegaByte)
            {
                TotalSize = TotalSize / MegaByte;
                LengthType = TotalSize.ToString("F") + " MB";
                
            }
            else if (TotalSize >= ByteEquivalent)
            {
                TotalSize = TotalSize / ByteEquivalent;
                LengthType = TotalSize.ToString("F") + " KB";
            }
            else
            {
                LengthType = TotalSize.ToString() + " bytes";
            }
            return LengthType; 
        }


        private void backgroundWorkerFiles_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string line = (string)e.UserState;
         
            progressBar1.Value = e.ProgressPercentage;
            //label_Percentage.Text = String.Format(label_Percentage.Text + "Percentage completed - {0}%", e.ProgressPercentage.ToString());
            label_Percentage.Text = "Percentage completed - " + e.ProgressPercentage.ToString() + "%";

            if (e.ProgressPercentage == 100)
            {
                progressBar1.Value = 0;
            }
            else if (e.ProgressPercentage == 0)
            {
                listBox1.Items.Add(line);
            }
        }

        private void backgroundWorkerFiles_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                label_Percentage.Text = String.Empty;
                listBox1.Items.Clear();
                MessageBox.Show("Process cancelled!");
                textBoxForInfo.Clear();
                textBoxDuplicatesInfo.Clear();
                buttonDelete.Enabled = false;
                progressBar1.Value = 0;
                Duplicates.Clear();
                buttonNext.Enabled = false;
                buttonPrevious.Enabled = false;
            }
            else if (e.Result != null)
            {
                if ((string)e.Result == "N/A")
                {
                    MessageBox.Show("No files with such extension(s) in the selected folder. Please try again.");
                }
                else
                {
                    MessageBox.Show("No duplicated files identified in the selected folder.");
                }
                textBoxForInfo.Clear();
                buttonDelete.Enabled = false;
                CheckedListBoxForResult.Items.Clear();
            }
            else
            {   
                //listBox1.DataSource = new BindingSource(Duplicates.Keys, null);
                listBox1.SelectedIndex = -1;
                textBoxForInfo.Enabled = true;
                textBoxDuplicatesInfo.Enabled = true;
                textBoxForInfo.ReadOnly = true;
                textBoxDuplicatesInfo.ReadOnly = true;
                textBoxForInfo.Text = "Duplicates - " + DuplicatesNumber + ";    Time spent, - " + time + Environment.NewLine;
                textBoxForInfo.AppendText("Total files size, " + TotalSizeAllFiles + "  " + SizeByExt);
                
                //textBoxForInfo.AppendText("Time spent on processing, Date and Size: ");

                //textBoxForInfo.AppendText(Math.Round(TimeSpan.FromMilliseconds(watchFordate.ElapsedMilliseconds).TotalSeconds, 1).ToString() + "; " + Math.Round(TimeSpan.FromMilliseconds(WatchForSize.ElapsedMilliseconds).TotalSeconds, 1).ToString() + Environment.NewLine;
                
                textBoxForInfo.BackColor = Color.LightGoldenrodYellow;
                textBoxDuplicatesInfo.BackColor = Color.LightGoldenrodYellow;
                MessageBox.Show("Processing has finished");

                    //buttonDelete.Enabled = true;
                    //buttonSelectAll.Enabled = true;
                    //buttonselectNone.Enabled = true;
                    //buttonNext.Enabled = true;
                    //buttonPrevious.Enabled = true;
            }

            buttonGetInfo.Enabled = true;
            buttonBrowseFolder.Enabled = true;
            buttonCancel.Enabled = false;
            comboBox1.Enabled = true;
            comboBox1.SelectedIndex = 0;
            textBoxForPath.Enabled = true;
        }

        private void OnBrowseButtonClick(object sender, EventArgs e)
        {
            using (FolderBrowserDialog FolderDialog = new FolderBrowserDialog())
            {
                FolderDialog.SelectedPath = textBoxForPath.Text;
                FolderDialog.Description = "Please select a folder";
                FolderDialog.ShowNewFolderButton = false;
                if (FolderDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxForPath.Text = FolderDialog.SelectedPath;
                }
            }
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            BGWorker.CancelAsync();
        }

        private void OnDeleteButtonClick(object sender, EventArgs e)
        {
            string Filepath;
            string KeyForDictionary;
            string ValueForDictionary;
            string NewDictionaryValue;

            for (int i = CheckedListBoxForResult.Items.Count - 1; i >= 0; i--)
                {
                    if (CheckedListBoxForResult.GetItemChecked(i))
                    {
                        Filepath = CheckedListBoxForResult.Items[i].ToString();
                        KeyForDictionary = listBox1.SelectedItem.ToString();
                        File.Delete(Filepath);
                        CheckedListBoxForResult.Items.RemoveAt(i);
                        Duplicates.TryGetValue(KeyForDictionary, out ValueForDictionary);
                        NewDictionaryValue = ValueForDictionary.Replace(Filepath, "");
                        Duplicates[KeyForDictionary] = NewDictionaryValue;
                    }
                }
            textBoxDuplicatesInfo.Clear();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (BGWorker.IsBusy)
            {
                BGWorker.CancelAsync();
            }
        }

        private void OnSelectAllButtonClick(object sender, EventArgs e)
        {
            for (int i = 0; i < CheckedListBoxForResult.Items.Count; i++)
            {
                CheckedListBoxForResult.SetSelected(i, true);
                CheckedListBoxForResult.SetItemCheckState(i, CheckState.Checked);
            }
            buttonSelectAll.Enabled = false;
        }

        private void OnSelectNoneButtonClick(object sender, EventArgs e)
        {
            for (int i = 0; i < CheckedListBoxForResult.Items.Count; i++)
            {
                CheckedListBoxForResult.SetItemCheckState(i, CheckState.Unchecked);
            }
            buttonselectNone.Enabled = false;
        }

        private void OmSelectionCompleted(object sender, EventArgs e)
        {
            int firstindex;
            int length;

            extension = comboBox1.SelectedItem.ToString();
 
            if (extension.Equals("All Picture Files"))
            {
                ext = "All Picture Files";
                split = new List<string>() { "*.bmp", "*.jpg", "*.jpeg", "*.gif", "*.png" };
            }
            else
            {
                firstindex = extension.IndexOf("(");
                length = extension.Length - firstindex;
                ext = extension.Substring((firstindex + 1), (length - 2));
                split = ext.Split(',').ToList();
            }
        }

        private void OnSelectedValueChanged(object sender, EventArgs e)
        {
            object selectedvalue;
            string[] DuplicatesArray;
            int NullCount;
            int DuplicatesAmount;
            string extension;
            string newfile;
            //List<CheckBox> CheckboxSet;
            int height;
            int labelheight;
            int picturebox2height;
            int interval;
            int checkboxinterval;
            int checkboxwidth;
            Label label;
            Label label2;
            PictureBox picturebox;
            
            SizeofDuplicates = 0;
            NullCount = 0;
            textBoxDuplicatesInfo.Clear();
            CheckedListBoxForResult.Items.Clear();
            CheckboxSet.Clear();
            buttonDelete.Enabled = false;

            if (listBox1.SelectedIndex == (listBox1.Items.Count - 1))
            {
                buttonNext.Enabled = false;
                buttonPrevious.Enabled = true;

            }
            else if (listBox1.SelectedIndex == 0)
            {
                buttonNext.Enabled = true;
                buttonPrevious.Enabled = false;
            }
            else
            {
                buttonNext.Enabled = true;
                buttonPrevious.Enabled = true;
            }

            if (PictureboxSet.Count > 0)
            {
                foreach (PictureBox pb in PictureboxSet)
                {
                    pb.Image.Dispose();
                }
                PictureboxSet.Clear();
                splitContainer1.Panel2.Controls.Clear();
            }

            if (listBox1.SelectedIndex != -1 && listBox1.Items.Count > 0)
            {
                buttonSelectAll.Enabled = true;
                //buttonselectNone.Enabled = true;

                selectedvalue = listBox1.SelectedItem;
                DuplicatesArray = Duplicates[selectedvalue.ToString()].Split(';');

                    foreach (string file in DuplicatesArray)
                    {
                        if (!String.IsNullOrEmpty(file))
                        {
                            CheckedListBoxForResult.Items.Add(file);
                        }
                        else
                        {
                            NullCount++;
                        }
                    }
                    DuplicatesAmount = DuplicatesArray.Count() - NullCount;
                    if (DuplicatesAmount > 0)
                    {
                        textBoxDuplicatesInfo.AppendText("Duplicate(s) - " + DuplicatesAmount.ToString());
                    }
                    else
                    {
                        textBoxDuplicatesInfo.AppendText("No duplicates");
                    }

                    extension = Path.GetExtension(selectedvalue.ToString()).ToLower();
                        if (extension == ".jpeg" || extension == ".jpg" || extension == ".png" || extension == ".gif" || extension == ".bmp")
                        {
                            //PictureboxSet = new List<PictureBox>();
                            CheckboxSet = new List<CheckBox>();
                            label = new Label();
                            label2 = new Label();

                            size = new Size(300, 300);
                            picturebox = new PictureBox();
                            PictureboxSet.Add(picturebox);
                            interval = 2;
                            checkboxwidth = 24;

                            picturebox.Size = size;
                            //picturebox.Resize += new EventHandler(picturebox_Resize);
                            
                            //picturebox.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top);

                            checkboxinterval = (picturebox.Height / 2 - interval);
                            label.Text = "Original file:";
                            labelheight = label.Height + interval;

                            try
                            {
                                picturebox.Image = Image.FromFile(selectedvalue.ToString());
                                picturebox.Location = new Point(checkboxwidth, labelheight);

                                height = picturebox.Height + labelheight + interval;

                                picturebox.SizeMode = PictureBoxSizeMode.Zoom;
                                splitContainer1.Panel2.Controls.Add(label);
                                splitContainer1.Panel2.Controls.Add(picturebox);

                                for (int n = 0; n < CheckedListBoxForResult.Items.Count; n++)
                                {
                                    CheckBox checkbox = new CheckBox();
                                    CheckboxSet.Add(checkbox);
                                    newfile = CheckedListBoxForResult.Items[n].ToString();

                                    PictureBox picturebox2 = new PictureBox();
                                    PictureboxSet.Add(picturebox2);
                                    picturebox2.Size = size;
                                    //picturebox2.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top);
                                    picturebox2.Image = Image.FromFile(newfile);
                                    if (n == 0)
                                    {
                                        label2.Text = "Duplicated files:";
                                        label2.Location = new Point(0, height);
                                        height += labelheight;
                                        splitContainer1.Panel2.Controls.Add(label2);
                                    }

                                    picturebox2.Location = new Point(checkboxwidth, height);
                                    picturebox2height = picturebox2.Height + interval;
                                    height += picturebox2height;
                                    picturebox2.SizeMode = PictureBoxSizeMode.Zoom;
                                    splitContainer1.Panel2.Controls.Add(picturebox2);
                                    int checkboxheight = checkbox.Height;
                                    checkbox.Location = new Point(0, (height - checkboxinterval));
                                    splitContainer1.Panel2.Controls.Add(checkbox);
                                    checkbox.CheckStateChanged += new EventHandler(CheckStateChanged);
                                    //checkbox.CheckStateChanged += (sender1, a) => CheckStateChanged(sender1, a, n);
                                    //SelectedIndex = i;
                                }

                                //for (int c = 0; c < CheckboxSet.Count; c++)
                                //{
                                //    if (CheckboxSet[c].Checked == true)
                                //    {
                                //        CheckedListBoxForResult.SetSelected(c, true);
                                //        CheckedListBoxForResult.SetItemCheckState(c, CheckState.Checked);
                                //    }
                                //}
                            }
                            catch
                            {
                                MessageBox.Show("Error occurs while loading image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
            }
        }
        private void CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            int currentindex = CheckboxSet.IndexOf(checkbox);

            if (checkbox.Checked == true)
            {
                CheckedListBoxForResult.SetSelected(currentindex, true);
                CheckedListBoxForResult.SetItemCheckState(currentindex, CheckState.Checked);
            }
            else
            {
                CheckedListBoxForResult.SetItemCheckState(currentindex, CheckState.Unchecked);
            }
        }
        private void OnSelectedItemClick(object sender, EventArgs e)
        {
            string ext;
            Form Form2;
            PictureBox picturebox;
            size = new Size(550, 550);

            if (CheckedListBoxForResult.Items.Count > 0 && CheckedListBoxForResult.SelectedItem != null)
            {
                ext = Path.GetExtension(CheckedListBoxForResult.SelectedItem.ToString()).ToLower();
                if (ext == ".jpeg" || ext == ".jpg" || ext == ".png" || ext == ".gif" || ext == ".bmp")
                {
                    Form2 = new Form();
                    Form2.MinimumSize = size;

                    Form2.Text = "Duplicated file";
                    Form2.StartPosition = FormStartPosition.CenterScreen;

                    picturebox = new PictureBox();
                    picturebox.Dock = DockStyle.Fill;
                    try
                    {
                        picturebox.Image = Image.FromFile(CheckedListBoxForResult.SelectedItem.ToString());

                        picturebox.SizeMode = PictureBoxSizeMode.Zoom;
                        Form2.Controls.Add(picturebox);
                        Form2.KeyDown += new KeyEventHandler(OnFormKeyDown);

                        Form2.ShowDialog();
                        picturebox.Image.Dispose();
                    }
                    catch
                    {
                        MessageBox.Show("Error occurs while loading image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            CheckedListBoxForResult.SelectedItem = null;
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                (sender as Form).Close();
            }
        }

 
        private void OnItemCheck(object sender, ItemCheckEventArgs e)
        {
            long FileSize;
            FileSize = 0;
            textBoxDuplicatesInfo.Clear();
            //buttonDelete.Enabled = true;
            //!!!!buttonselectedall

            if (e.CurrentValue != CheckState.Checked && e.NewValue == CheckState.Checked)
            {
                FilesSize.TryGetValue(CheckedListBoxForResult.SelectedItem.ToString(), out FileSize);
                SizeofDuplicates += FileSize;
                
                buttonselectNone.Enabled = true;
            }
            else if (e.CurrentValue == CheckState.Checked && e.NewValue != CheckState.Checked)
            {
                FilesSize.TryGetValue(CheckedListBoxForResult.SelectedItem.ToString(), out FileSize);
                SizeofDuplicates = SizeofDuplicates - FileSize;
            }
            if (SizeofDuplicates > 0)
            {
                textBoxDuplicatesInfo.AppendText(GetSize((double)SizeofDuplicates));
                buttonDelete.Enabled = true;
            }
            else
            {
                buttonDelete.Enabled = false;
                buttonselectNone.Enabled = false;
            }
        }

        private void OnNextButtonClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0 && listBox1.SelectedIndex < (listBox1.Items.Count - 1))
            {
                listBox1.SelectedIndex = listBox1.SelectedIndex + 1; 
            }
        }

        private void OnPreviousButtonClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > 0 && listBox1.SelectedIndex <= (listBox1.Items.Count - 1))
            {
                listBox1.SelectedIndex = listBox1.SelectedIndex - 1;
            }
        }
    }
}



