using DiscUtils.Iso9660;
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

namespace IsoMaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            if (!String.IsNullOrEmpty(folderBrowserDialog1.SelectedPath))
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                textBox2.Text = folderBrowserDialog1.SelectedPath + "\\output.iso";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            if (!String.IsNullOrEmpty(saveFileDialog1.FileName))
            {
                textBox2.Text = saveFileDialog1.FileName;
            }
        }

        private Dictionary<string, string> getFileList(DirectoryInfo folder, DirectoryInfo home)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            CDBuilder builder = new CDBuilder();

            foreach (FileInfo file in folder.GetFiles())
            {
                string fileFullPath = file.FullName;
                
                // The code below might chop off too much of the string. I had this issue.
                // string fileOnIso = fileFullPath.TrimStart(home.FullName.ToCharArray());
                
                // This code simply delimits the string by the root directory path with a backslash. This works better.
                string fileOnIso = fileFullPath.Split(home.FullName + '\\')[1];
                
                output.Add(fileOnIso, file.FullName);
            }

            // Now do it for all subfolders
            foreach (DirectoryInfo directory in folder.GetDirectories())
            {
                getFileList(directory, home).ToList().ForEach(file => output.Add(file.Key, file.Value));
            }

            return output;
        }

        private int BuildIso(DirectoryInfo sourceDirectory, string targetFile)
        {
            CDBuilder builder = new CDBuilder();
            Dictionary<string, string> resultList = new Dictionary<string, string>();

            try
            {
                // Get main folder and put it into results.
                getFileList(sourceDirectory, sourceDirectory).ToList().ForEach(file => resultList.Add(file.Key, file.Value));

                // Now do it for all subfolders
                /*foreach (DirectoryInfo directory in sourceDirectory.GetDirectories())
                {
                    getFileList(directory, sourceDirectory).ToList().ForEach(file => resultList.Add(file.Key, file.Value));
                }*/

                // Finally, add all files collected to the ISO.
                foreach (KeyValuePair<string, string> pair in resultList.ToList())
                {
                    builder.AddFile(pair.Key, pair.Value);
                }

                builder.Build(targetFile);
            } catch(Exception e)
            {
                MessageBox.Show("Error Writing ISO. Check Permissions and Files. " + e.Message);
                return 1;
            }
            
            return 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CDBuilder builder = new CDBuilder();
            Dictionary<string, string> resultList = new Dictionary<string, string>();
  


            if (Directory.Exists(textBox1.Text))
            {
                DirectoryInfo chosenDirectory = new DirectoryInfo(textBox1.Text);
                int buildResult = BuildIso(chosenDirectory, textBox2.Text);
                if(buildResult == 0)
                {
                    MessageBox.Show("ISO written sucessfully!");
                } 
                   

            }
            else
            {
                MessageBox.Show("Please make sure the directory exists!");
            }


        }
    }
}
