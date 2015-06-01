using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        string extension;
        List<string> split;
        long allFilesSize = 0;
        int countUnverified = 0;
        string time;
        long sizeofDuplicates = 0;
        int duplicatesAmount = 0;
        Label label4 = new Label();
        Stopwatch stopwatch = new Stopwatch();
        BackgroundWorker bgWorker = new BackgroundWorker();
        List<PictureBox> pictureboxSet = new List<PictureBox>();
        List<CheckBox> checkboxSet = new List<CheckBox>();
        Dictionary<ItemInfo, List<ItemInfo>> verifiedDuplicated = new Dictionary<ItemInfo, List<ItemInfo>>();
        Dictionary<ItemInfo, List<ItemInfo>> unverifiedDuplicated = new Dictionary<ItemInfo, List<ItemInfo>>();
        Dictionary<ItemInfo, List<ItemInfo>> duplicates = new Dictionary<ItemInfo, List<ItemInfo>>();

        public const int BYTESTOREAD = sizeof(Int64);
        public const int INTERVAL = 2;
        public const byte NUMBEROFITERATIONS = 3;
        public const int CHECKBOXWIDTH = 24;
        public const string NOTAPPLICABLE = "N/A";
        public const string NODUPLICATES = "No duplicates";
        public const string INVALIDPATH = "The path is invalid. Please specify the path to the folder.";
        public const string FILEPROCESSING = "Please wait. Files are being processed.";
        public const string COMPLETEDPERCENTAGE = "100% completed";
        public readonly string[] STRINGSEPARATORS = new string[] { " - copy", " - копия", " (", "(" };
        public readonly string[] IMAGEEXTENSIONS = new string[] { "*.bmp", "*.jpg", "*.jpeg", "*.gif", "*.png" };
        public readonly Size size = new Size(300, 300);

        public Form1()
        {
            InitializeComponent();
            InitializeBackgroundWorker(bgWorker);
            comboBox1.SelectedIndex = 0;
        }

        public class ItemInfo
        {
            public ItemInfo(string fullname)
            {
                FullPath = fullname;
            }

            public string FullPath
            {
                get;
                private set;
            }

            public string CameraAndDateTaken
            {
                get;
                set;
            }

            public string FileName
            {
                get;
                set;
            }

            public long FileSize
            {
                get;
                set;
            }

            public int AttributeCount
            {
                get;
                set;
            }
        }

        public string GetShortName(string file)
        {
            string nameWithoutExt;

            nameWithoutExt = Path.GetFileNameWithoutExtension((file).ToLower());

            if (STRINGSEPARATORS.Any(s => nameWithoutExt.Contains(s)))
            {
                return nameWithoutExt.Split(STRINGSEPARATORS, StringSplitOptions.None)[0];
            }
            else
            {
                return nameWithoutExt;
            }
        }

        public string GetDate(string file)
        {
            FileStream filestream;
            BitmapSource image;
            BitmapMetadata metadata;
            string cameraModel;
            string dateTaken;

            using (filestream = new FileStream(file, FileMode.Open))
            {
                try
                {
                    image = BitmapFrame.Create(filestream);
                    metadata = (BitmapMetadata)image.Metadata;
                    cameraModel = metadata.CameraModel;
                    dateTaken = metadata.DateTaken;

                    if (dateTaken != null && cameraModel != null)
                    {
                        return string.Concat(dateTaken, "_", cameraModel);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        private void InitializeBackgroundWorker(BackgroundWorker bgWorker)
        {
            bgWorker.DoWork += new DoWorkEventHandler(OnFilesWorkerDoWork);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnFilesWorkerRunWorkerCompleted);
            bgWorker.ProgressChanged += new ProgressChangedEventHandler(OnFilesWorkerProgressChanged);
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
        }

        private void OnGetInfoButtonClick(object sender, EventArgs e)
        {
            List<string> listOfParameters;
            string newPath;

            listOfParameters = new List<string>();
            countUnverified = 0;
            allFilesSize = 0;
            newPath = textBoxForPath.Text;

            ResultCheckedListBox.Items.Clear();
            textBoxForInfo.Clear();
            textBoxDuplicatesInfo.Clear();
            duplicates.Clear();
            verifiedDuplicated.Clear();
            unverifiedDuplicated.Clear();
            listBox1.Items.Clear();

            textBoxForInfo.BackColor = Color.White;
            textBoxDuplicatesInfo.BackColor = Color.White;
            textBoxForPath.BackColor = Color.White;

            buttonNext.Enabled = false;
            buttonPrevious.Enabled = false;
            textBoxForInfo.Enabled = false;
            textBoxDuplicatesInfo.Enabled = false;

            if (pictureboxSet.Count > 0)
            {
                foreach (PictureBox picture in pictureboxSet)
                {
                    picture.Image.Dispose();
                }

                pictureboxSet.Clear();
                splitContainer1.Panel2.Controls.Clear();
            }

            if (string.IsNullOrWhiteSpace(newPath) || newPath.StartsWith(" ") || !Directory.Exists(newPath))
            {
                textBoxForPath.BackColor = Color.Red;
                MessageBox.Show(INVALIDPATH);
            }
            else
            {
                buttonGetInfo.Enabled = false;
                buttonBrowseFolder.Enabled = false;
                comboBox1.Enabled = false;
                buttonCancel.Enabled = true;
                buttonDelete.Enabled = false;
                buttonSelectAll.Enabled = false;
                buttonselectNone.Enabled = false;
                textBoxForPath.Enabled = false;

                labelForPercentage.Text = FILEPROCESSING;
                listOfParameters.Add(newPath);
                listOfParameters.AddRange(split);
                bgWorker.RunWorkerAsync(listOfParameters);
            }
        }

        private void OnFilesWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            string extension;
            string mainPath;
            int percent;
            int filecount;
            TimeSpan ts;
            List<string> folderPathes;
            FileInfo[] arrayOfFiles;
            List<string> listOfFullPathes;
            List<ItemInfo> listAllFiles;
            DirectoryInfo Subdirectory;
            ItemInfo iteminfo;
            List<string> parameters;

            folderPathes = new List<string>();
            listOfFullPathes = new List<string>();
            listAllFiles = new List<ItemInfo>();
            percent = 0;
            filecount = 0;
            parameters = (List<string>)e.Argument;
            mainPath = parameters[0];
            parameters.RemoveAt(0);

            stopwatch.Restart();
            folderPathes = ProcessDirectory(mainPath);
            folderPathes.Add(mainPath);

            foreach (string subfolder in folderPathes)
            {
                Subdirectory = new DirectoryInfo(subfolder);

                foreach (string param in parameters)
                {
                    if (bgWorker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }
                    else
                    {
                        arrayOfFiles = Subdirectory.GetFiles(param);

                        listOfFullPathes.AddRange(arrayOfFiles.Select(x => Path.Combine(Subdirectory.ToString(), x.ToString())));
                    }
                }
            }

            if (listOfFullPathes.Count != 0)
            {

                listOfFullPathes = listOfFullPathes.OrderBy(a => Path.GetFileName(a).Length).ToList();

                for (int x = 0; x < listOfFullPathes.Count; x++)
                {
                    if (bgWorker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }
                    else
                    {
                        extension = Path.GetExtension(listOfFullPathes[x]).ToLower();
                        iteminfo = new ItemInfo(listOfFullPathes[x]);
                        iteminfo.AttributeCount = (x + 1);
                        iteminfo.FileSize = new FileInfo(listOfFullPathes[x]).Length;
                        iteminfo.FileName = GetShortName(listOfFullPathes[x]);

                        if (extension.Equals(IMAGEEXTENSIONS[1]) || extension.Equals(IMAGEEXTENSIONS[2]))
                        {
                            iteminfo.CameraAndDateTaken = GetDate(listOfFullPathes[x]);
                        }
                        listAllFiles.Add(iteminfo);
                    }
                    filecount++;
                    percent = PercentageReportProgress(listOfFullPathes.Count, percent, ref filecount);
                }

                duplicates = IdentifyDuplicates(listAllFiles, e);
                stopwatch.Stop();

                if (duplicates.Count == 0)
                {
                    e.Result = NODUPLICATES;
                }
                else
                {
                    foreach (KeyValuePair<ItemInfo, List<ItemInfo>> kvp in duplicates)
                    {
                        bgWorker.ReportProgress(0, kvp.Key.FullPath);
                    }
                }

                ts = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds);

                time = string.Format("{0:D2}h:{1:D2}m:{2:D2}s", ts.Hours, ts.Minutes, ts.Seconds);
            }
            else
            {
                e.Result = NOTAPPLICABLE;
            }
        }

        public Dictionary<ItemInfo, List<ItemInfo>> IdentifyDuplicates(List<ItemInfo> filesList, DoWorkEventArgs e)
        {
            ItemInfo currentitem;
            ItemInfo nullItem;
            string date;
            List<ItemInfo> items;
            int filesAmount;
            int processedPercentage;
            int verifiedItemsCount;
            int unverifiedItemsCount;
            int filesListCount;

            nullItem = new ItemInfo(null);
            filesAmount = 0;
            processedPercentage = 0;
            countUnverified = 0;
            unverifiedItemsCount = 0;
            verifiedItemsCount = 0;
            filesListCount = filesList.Count;

            for (int b = 0; b < filesListCount; b++)
            {
                date = filesList[b].CameraAndDateTaken;

                currentitem = filesList[b];
                filesList[b] = nullItem;

                if (bgWorker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else if (currentitem.AttributeCount != 0)
                {
                    if ((items = filesList.FindAll(x => x.FileSize == currentitem.FileSize)).Count > 0)
                    {
                        for (int a = 0; a < items.Count; a++)
                        {
                            if (bgWorker.CancellationPending == true)
                            {
                                e.Cancel = true;
                                break;
                            }
                            else if (currentitem.FileName == items[a].FileName)
                            {
                                if (currentitem.CameraAndDateTaken != null && currentitem.CameraAndDateTaken == items[a].CameraAndDateTaken)
                                {
                                    verifiedDuplicated = AddToDictionary(verifiedDuplicated, currentitem, items[a], verifiedItemsCount, out verifiedItemsCount);
                                    duplicatesAmount++;
                                    allFilesSize += items[a].FileSize;
                                }
                                else
                                {
                                    unverifiedDuplicated = AddToDictionary(unverifiedDuplicated, currentitem, items[a], unverifiedItemsCount, out unverifiedItemsCount);
                                    countUnverified++;
                                }
                            }
                            else
                            {
                                unverifiedDuplicated = AddToDictionary(unverifiedDuplicated, currentitem, items[a], unverifiedItemsCount, out unverifiedItemsCount);
                                countUnverified++;
                            }
                            filesList[(items[a].AttributeCount - 1)] = nullItem;
                        }
                    }
                    else if ((items = filesList.FindAll(x => x.FileName == currentitem.FileName)).Count > 0)
                    {
                        for (int c = 0; c < items.Count; c++)
                        {
                            if (bgWorker.CancellationPending == true)
                            {
                                e.Cancel = true;
                                break;
                            }
                            else if (date != null && currentitem.CameraAndDateTaken == items[c].CameraAndDateTaken)
                            {
                                verifiedDuplicated = AddToDictionary(verifiedDuplicated, currentitem, items[c], verifiedItemsCount, out verifiedItemsCount);
                                duplicatesAmount++;
                                allFilesSize += items[c].FileSize;
                            }
                            else
                            {
                                unverifiedDuplicated = AddToDictionary(unverifiedDuplicated, currentitem, items[c], unverifiedItemsCount, out unverifiedItemsCount);
                                countUnverified++;
                            }
                            filesList[(items[c].AttributeCount - 1)] = nullItem;
                        }
                    }
                    else if (date != null && (items = filesList.FindAll(x => x.CameraAndDateTaken == date)).Count > 0)
                    {
                        for (int d = 0; d < items.Count; d++)
                        {
                            if (bgWorker.CancellationPending == true)
                            {
                                e.Cancel = true;
                                break;
                            }
                            unverifiedDuplicated = AddToDictionary(unverifiedDuplicated, currentitem, items[d], unverifiedItemsCount, out unverifiedItemsCount);
                            filesList[(items[d].AttributeCount - 1)] = nullItem;
                            countUnverified++;
                        }
                    }
                }
                filesAmount++;
                processedPercentage = PercentageReportProgress(filesListCount, processedPercentage, ref filesAmount);
            }
            if (unverifiedDuplicated.Count > 0)
            {
                verifiedDuplicated = BytesComparison(unverifiedDuplicated, e, verifiedItemsCount);
            }
            return verifiedDuplicated;
        }

        public int PercentageReportProgress(int totalAmount, int processedPercentage, ref int count)
        {
            int Percent;

            if (totalAmount >= 100)
            {
                Percent = totalAmount / 100;
                if (count == Percent)
                {
                    processedPercentage = processedPercentage + 1;
                    if (processedPercentage > 100)
                    {
                        processedPercentage = 100;
                    }

                    bgWorker.ReportProgress(processedPercentage, null);
                    count = 0;
                }
            }
            else
            {
                Percent = 100 / totalAmount;
                processedPercentage = processedPercentage + Percent;
                if (processedPercentage > 100)
                {
                    processedPercentage = 100;
                }

                bgWorker.ReportProgress(processedPercentage, null);
            }

            return processedPercentage;
        }

        public Dictionary<ItemInfo, List<ItemInfo>> BytesComparison(Dictionary<ItemInfo, List<ItemInfo>> filesToVerify, DoWorkEventArgs e, int countItems)
        {
            ItemInfo key;
            bool byteEquality;
            int count;
            int percent;
            int processedPercent;

            count = 0;
            percent = 0;
            processedPercent = 0;

            foreach (KeyValuePair<ItemInfo, List<ItemInfo>> dictionaryItem in filesToVerify)
            {
                key = dictionaryItem.Key;
                foreach (ItemInfo entry in dictionaryItem.Value)
                {
                    byteEquality = FilesEquality(new FileInfo(key.FullPath), new FileInfo(entry.FullPath), e);
                    if (bgWorker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }
                    else if (byteEquality)
                    {
                        verifiedDuplicated = AddToDictionary(verifiedDuplicated, key, entry, countItems, out countItems);
                        duplicatesAmount++;
                        allFilesSize += entry.FileSize;
                    }
                    count++;
                    processedPercent = PercentageReportProgress(countUnverified, processedPercent, ref count);
                    count = percent;
                }
            }
            return verifiedDuplicated;
        }

        public bool FilesEquality(FileInfo first, FileInfo second, DoWorkEventArgs e)
        {
            int randomNumber;
            Random rnd;

            if (first.Length != second.Length)
            {
                return false;
            }
            else
            {
                try
                {
                    using (FileStream fs1 = first.OpenRead(), fs2 = second.OpenRead())
                    {
                        byte[] one = new byte[BYTESTOREAD];
                        byte[] two = new byte[BYTESTOREAD];

                        for (int i = 0; i < NUMBEROFITERATIONS; i++)
                        {
                            if (i > 0)
                            {
                                rnd = new Random();

                                if (first.Length > Int32.MaxValue)
                                {
                                    randomNumber = rnd.Next((BYTESTOREAD + 1), Int32.MaxValue - BYTESTOREAD);
                                }
                                else
                                {
                                    randomNumber = rnd.Next((BYTESTOREAD + 1), (int)(first.Length - (BYTESTOREAD + 1)));
                                }
                                fs1.Seek(randomNumber, SeekOrigin.Begin);
                                fs2.Seek(randomNumber, SeekOrigin.Begin);
                            }
                            fs1.Read(one, 0, BYTESTOREAD);
                            fs2.Read(two, 0, BYTESTOREAD);

                            if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public Dictionary<ItemInfo, List<ItemInfo>> AddToDictionary(Dictionary<ItemInfo, List<ItemInfo>> dictionary, ItemInfo item1, ItemInfo item2, int keycount, out int newcount)
        {
            List<ItemInfo> valueList;
            newcount = keycount;

            if (!dictionary.ContainsKey(item1))
            {
                item1.AttributeCount = keycount;
                dictionary.Add(item1, new List<ItemInfo> { item2 });
                keycount++;
                newcount = keycount;
            }
            else
            {
                dictionary.TryGetValue(item1, out valueList);
                valueList.Add(item2);

                dictionary[item1] = valueList;
            }
            return dictionary;
        }

        public List<string> ProcessDirectory(string path)
        {
            try
            {
                return Directory.GetDirectories(path, "*", SearchOption.AllDirectories).ToList();
            }
            catch
            {
                return null;
            }
        }

        public string ConvertSize(double totalSize)
        {
            double byteEquivalent;
            string lengthType;

            byteEquivalent = 1024;
            double gigaByte = Math.Pow(byteEquivalent, 3);
            double megaByte = Math.Pow(byteEquivalent, 2);

            if (totalSize >= gigaByte)
            {
                totalSize = totalSize / gigaByte;
                lengthType = totalSize.ToString("F") + " GB";
            }
            else if (totalSize >= megaByte)
            {
                totalSize = totalSize / megaByte;
                lengthType = totalSize.ToString("F") + " MB";

            }
            else if (totalSize >= byteEquivalent)
            {
                totalSize = totalSize / byteEquivalent;
                lengthType = totalSize.ToString("F") + " KB";
            }
            else
            {
                lengthType = totalSize.ToString() + " bytes";
            }
            return lengthType;
        }

        private void OnFilesWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string line = (string)e.UserState;

            progressBar.Value = e.ProgressPercentage;
            labelForPercentage.Text = "Percentage completed - " + e.ProgressPercentage.ToString() + "%";

            if (e.ProgressPercentage == 0)
            {
                listBox1.Items.Add(line);
            }
        }

        private void OnFilesWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                listBox1.Items.Clear();
                MessageBox.Show("Process cancelled!");
                textBoxForInfo.Clear();
                textBoxDuplicatesInfo.Clear();
            }
            else if (e.Result != null)
            {
                if ((string)e.Result == NOTAPPLICABLE)
                {
                    labelForPercentage.Text = String.Empty;
                    MessageBox.Show("No files with such extension(s) in the selected folder. Please try again.");
                }
                else
                {
                    labelForPercentage.Text = COMPLETEDPERCENTAGE;
                    progressBar.Value = 100;
                    MessageBox.Show("No duplicated files identified in the selected folder.");
                }
                textBoxForInfo.Clear();
                ResultCheckedListBox.Items.Clear();
            }
            else
            {
                listBox1.SelectedIndex = -1;
                textBoxForInfo.Enabled = true;
                textBoxDuplicatesInfo.Enabled = true;
                textBoxForInfo.ReadOnly = true;
                textBoxDuplicatesInfo.ReadOnly = true;

                textBoxForInfo.Text = "Number of duplicates - " + duplicatesAmount + ";    Time spent - " + time + Environment.NewLine;
                textBoxForInfo.AppendText("Total size of identified duplicated files - " + ConvertSize((double)allFilesSize));

                textBoxForInfo.BackColor = Color.LightGoldenrodYellow;
                textBoxDuplicatesInfo.BackColor = Color.LightGoldenrodYellow;
                labelForPercentage.Text = COMPLETEDPERCENTAGE;

                MessageBox.Show("Processing has finished");
            }
            buttonGetInfo.Enabled = true;
            buttonBrowseFolder.Enabled = true;
            buttonCancel.Enabled = false;
            comboBox1.Enabled = true;
            textBoxForPath.Enabled = true;

            progressBar.Value = 0;
            labelForPercentage.Text = String.Empty;
        }

        private void OnBrowseButtonClick(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.SelectedPath = textBoxForPath.Text;
                folderDialog.Description = "Please select a folder";
                folderDialog.ShowNewFolderButton = false;
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxForPath.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            bgWorker.CancelAsync();
        }

        private void OnDeleteButtonClick(object sender, EventArgs e)
        {
            List<ItemInfo> valueForDictionary;
            Point newpoint;
            int heightOfPicture;

            for (int i = ResultCheckedListBox.Items.Count - 1; i >= 0; i--)
            {
                if (ResultCheckedListBox.GetItemChecked(i))
                {
                    try
                    {
                        if (pictureboxSet.Count > 0)
                        {
                            for (int a = 0; a < pictureboxSet.Count; a++)
                            {
                                if (i == (a - 1))
                                {
                                    pictureboxSet[a].Image.Dispose();
                                    splitContainer1.Panel2.Controls.Remove(pictureboxSet[a]);
                                    splitContainer1.Panel2.Controls.Remove(checkboxSet[i]);

                                    if (a != (pictureboxSet.Count - 1))
                                    {
                                        heightOfPicture = pictureboxSet[a].Size.Height;

                                        for (int b = (a + 1); b < pictureboxSet.Count; b++)
                                        {
                                            newpoint = pictureboxSet[b].Location;
                                            newpoint.Offset(0, (-heightOfPicture));
                                            pictureboxSet[b].Location = newpoint;

                                            newpoint = checkboxSet[b - 1].Location;
                                            newpoint.Offset(0, -(heightOfPicture + INTERVAL));
                                            checkboxSet[b - 1].Location = newpoint;
                                        }
                                    }
                                    break;
                                }
                            }
                            pictureboxSet.RemoveAt(i + 1);
                            checkboxSet.RemoveAt(i);
                        }

                        File.Delete(ResultCheckedListBox.Items[i].ToString());
                        ResultCheckedListBox.Items.RemoveAt(i);
                        duplicates.TryGetValue(duplicates.Keys.ElementAt(listBox1.SelectedIndex), out valueForDictionary);
                        valueForDictionary.RemoveAt(i);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            if (pictureboxSet.Count == 1)
            {
                label4.Text = String.Empty;
            }
            if (ResultCheckedListBox.Items.Count == 0)
            {
                buttonSelectAll.Enabled = false;
            }

            buttonDelete.Enabled = false;
            buttonselectNone.Enabled = false;

            textBoxDuplicatesInfo.Clear();
            sizeofDuplicates = 0;
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (bgWorker.IsBusy)
            {
                bgWorker.CancelAsync();
            }
        }

        private void OnSelectAllButtonClick(object sender, EventArgs e)
        {
            for (int i = 0; i < ResultCheckedListBox.Items.Count; i++)
            {
                ResultCheckedListBox.SetItemCheckState(i, CheckState.Checked);
            }
            buttonSelectAll.Enabled = false;
        }

        private void OnSelectNoneButtonClick(object sender, EventArgs e)
        {
            for (int i = 0; i < ResultCheckedListBox.Items.Count; i++)
            {
                ResultCheckedListBox.SetItemCheckState(i, CheckState.Unchecked);
            }
            buttonselectNone.Enabled = false;
        }

        private void OnComboBoxSelectionCompleted(object sender, EventArgs e)
        {
            int firstindex;

            extension = comboBox1.SelectedItem.ToString();

            if (extension.Equals("All Image Files"))
            {
                split = IMAGEEXTENSIONS.ToList();
            }
            else
            {
                firstindex = extension.IndexOf("(");
                split = extension.Substring(firstindex).Trim('(', ')').Replace(" ", String.Empty).Split(',').ToList();
            }
        }

        private void OnSelectedValueChanged(object sender, EventArgs e)
        {
            int duplicatesAmount;

            ItemInfo selectedkey;
            List<ItemInfo> valueList;

            sizeofDuplicates = 0;
            duplicatesAmount = 0;
            selectedkey = null;
            textBoxDuplicatesInfo.Clear();
            ResultCheckedListBox.Items.Clear();
            checkboxSet.Clear();
            buttonDelete.Enabled = false;
            buttonselectNone.Enabled = false;
            buttonSelectAll.Enabled = false;

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
            else if (listBox1.SelectedIndex != -1)
            {
                buttonNext.Enabled = true;
                buttonPrevious.Enabled = true;
            }

            if (pictureboxSet.Count > 0)
            {
                foreach (PictureBox pb in pictureboxSet)
                {
                    pb.Image.Dispose();
                }
                pictureboxSet.Clear();
                splitContainer1.Panel2.Controls.Clear();
            }

            if (listBox1.SelectedIndex != -1 && listBox1.Items.Count > 0)
            {
                selectedkey = duplicates.Keys.ElementAt(listBox1.SelectedIndex);

                duplicates.TryGetValue(selectedkey, out valueList);

                if (valueList.Count > 0)
                {
                    buttonSelectAll.Enabled = true;

                    foreach (ItemInfo entry in valueList)
                    {
                        duplicatesAmount = valueList.Count;
                        ResultCheckedListBox.Items.Add(entry.FullPath);
                    }
                }

                if (duplicatesAmount > 0)
                {
                    textBoxDuplicatesInfo.AppendText("Amount of duplicate(s) - " + duplicatesAmount.ToString());
                }
                else
                {
                    textBoxDuplicatesInfo.AppendText(NODUPLICATES);
                }
                PicturePreview(listBox1.SelectedItem.ToString());
            }
        }

        private void PicturePreview(string filename)
        {
            int height;
            int labelheight;
            Label label;
            PictureBox picturebox;
            CheckBox checkbox;
            PictureBox picturebox2;
            string filePreviewExtension;

            filePreviewExtension = String.Concat("*", Path.GetExtension(filename).ToLower());

            if (Array.IndexOf(IMAGEEXTENSIONS, filePreviewExtension) != -1)
            {
                checkboxSet = new List<CheckBox>();
                label = new Label();

                picturebox = new PictureBox();
                pictureboxSet.Add(picturebox);

                picturebox.Size = size;
                label.Text = "Original file:";
                labelheight = label.Height + INTERVAL;

                try
                {
                    picturebox.Image = Image.FromFile(filename);
                    picturebox.Location = new Point(CHECKBOXWIDTH, labelheight);

                    picturebox.SizeMode = PictureBoxSizeMode.Zoom;
                    splitContainer1.Panel2.Controls.Add(label);
                    splitContainer1.Panel2.Controls.Add(picturebox);

                    for (int n = 0; n < ResultCheckedListBox.Items.Count; n++)
                    {
                        checkbox = new CheckBox();
                        checkboxSet.Add(checkbox);
                        picturebox2 = new PictureBox();

                        picturebox2.Size = size;
                        picturebox2.Image = Image.FromFile(ResultCheckedListBox.Items[n].ToString());

                        height = pictureboxSet[pictureboxSet.Count - 1].Location.Y + size.Height + INTERVAL;
                        pictureboxSet.Add(picturebox2);
                        if (n == 0)
                        {
                            label4.Text = "Duplicated files:";
                            label4.Location = new Point(0, height);
                            splitContainer1.Panel2.Controls.Add(label4);
                            picturebox2.Location = new Point(CHECKBOXWIDTH, (label4.Bottom + INTERVAL));
                        }
                        else
                        {
                            picturebox2.Location = new Point(CHECKBOXWIDTH, height);
                        }
                        picturebox2.SizeMode = PictureBoxSizeMode.Zoom;
                        splitContainer1.Panel2.Controls.Add(picturebox2);
                        checkbox.Location = new Point(0, (pictureboxSet[pictureboxSet.Count - 1].Location.Y + size.Height / 2));
                        splitContainer1.Panel2.Controls.Add(checkbox);
                        checkbox.Click += new EventHandler(OnCheckboxClick);
                    }
                }
                catch
                {
                    MessageBox.Show("Error occurs while loading image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnCheckboxClick(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;

            if (checkbox.Checked == true)
            {
                ResultCheckedListBox.SetItemCheckState(checkboxSet.IndexOf(checkbox), CheckState.Checked);
            }
            else
            {
                ResultCheckedListBox.SetItemCheckState(checkboxSet.IndexOf(checkbox), CheckState.Unchecked);
            }
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                (sender as Form).Close();
            }
        }

        private void OntemDoubleClick(object sender, EventArgs e)
        {
            string ext;
            Form form2;
            PictureBox picturebox;
            Size size;
            size = new Size(550, 550);
            string filePath;

            filePath = ResultCheckedListBox.SelectedItem.ToString();

            if (ResultCheckedListBox.Items.Count > 0 && ResultCheckedListBox.SelectedItem != null)
            {
                ext = Path.GetExtension(ResultCheckedListBox.SelectedItem.ToString()).ToLower();
                if (Array.IndexOf(IMAGEEXTENSIONS, ext) != -1)
                {
                    form2 = new Form();
                    form2.MinimumSize = size;

                    form2.Text = "Duplicated file";
                    form2.StartPosition = FormStartPosition.CenterScreen;

                    picturebox = new PictureBox();
                    picturebox.Dock = DockStyle.Fill;
                    try
                    {
                        picturebox.Image = Image.FromFile(filePath);

                        picturebox.SizeMode = PictureBoxSizeMode.Zoom;
                        form2.Controls.Add(picturebox);
                        form2.KeyDown += new KeyEventHandler(OnFormKeyDown);

                        form2.ShowDialog();
                        picturebox.Image.Dispose();
                    }
                    catch
                    {
                        MessageBox.Show("Error occurs while loading image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Process.Start("rundll32.exe", "shell32.dll, OpenAs_RunDLL " + filePath);
                }
            }
            ResultCheckedListBox.SelectedItem = null;
        }

        private void OnItemCheck(object sender, ItemCheckEventArgs e)
        {
            long fileSizeInfo;
            long value = 0;
            List<ItemInfo> valueDictionary;

            fileSizeInfo = 0;
            textBoxDuplicatesInfo.Clear();

            duplicates.TryGetValue(duplicates.Keys.ElementAt(listBox1.SelectedIndex), out valueDictionary);
            fileSizeInfo = valueDictionary[e.Index].FileSize;

            if (e.CurrentValue != CheckState.Checked && e.NewValue == CheckState.Checked)
            {
                buttonselectNone.Enabled = true;

                sizeofDuplicates += fileSizeInfo;
                
                if (checkboxSet.Count > 0)
                {
                    checkboxSet[e.Index].Checked = true;
                } 
            }
            else if (e.CurrentValue == CheckState.Checked && e.NewValue != CheckState.Checked)
            {
                buttonSelectAll.Enabled = true;

                sizeofDuplicates = sizeofDuplicates - fileSizeInfo;
                
                if (checkboxSet.Count > 0)
                {
                    checkboxSet[e.Index].Checked = false;
                }
            }

            for (int i = 0; i < valueDictionary.Count; i++)
            {
                value += valueDictionary[i].FileSize;
            }

            if (sizeofDuplicates == value)
            {
                buttonSelectAll.Enabled = false;
            }

            if (sizeofDuplicates > 0)
            {
                textBoxDuplicatesInfo.AppendText("Size of selected file(s) - " + ConvertSize((double)sizeofDuplicates));
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