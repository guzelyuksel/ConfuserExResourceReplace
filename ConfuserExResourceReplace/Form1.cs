using dnlib.DotNet;
using dnlib.DotNet.Writer;
using System;
using System.IO;
using System.Windows.Forms;

namespace ConfuserExResourceReplace
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
        }

        private ModuleDefMD mainasm;
        private ModuleDefMD resasm;
        private int totalreplace = 0;

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                string ext = Path.GetExtension(file);
                if(ext == ".dll")
                {
                    this.textBox2.Text = file;
                    this.textBox2.Click -= new System.EventHandler(this.textBox2_Click);

                }
                else if(ext == ".exe")
                {
                    this.textBox1.Text = file;
                    this.textBox1.Click -= new System.EventHandler(this.textBox1_Click);

                }
                else
                {
                    MessageBox.Show("File cannot accepted.");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                mainasm = ModuleDefMD.Load(this.textBox1.Text);
                resasm = ModuleDefMD.Load(this.textBox2.Text);

                if (resasm.HasResources && mainasm.HasResources)
                {
                    if (resasm.Resources.Count == mainasm.Resources.Count)
                    {
                        foreach (var goodres in resasm.Resources)
                        {
                            mainasm.Resources.Remove(mainasm.Resources.Find(goodres.Name));
                            mainasm.Resources.Add(goodres);
                            totalreplace++;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Resource counts must be equal !");
                    }
                }
                ModuleWriterOptions moduleWriterOptions = new ModuleWriterOptions(mainasm);
                if (mainasm.IsILOnly)
                {
                    
                    moduleWriterOptions.MetaDataOptions.Flags |= (MetaDataFlags.PreserveTypeRefRids | MetaDataFlags.PreserveTypeDefRids | MetaDataFlags.PreserveFieldRids | MetaDataFlags.PreserveMethodRids | MetaDataFlags.PreserveParamRids | MetaDataFlags.PreserveMemberRefRids | MetaDataFlags.PreserveStandAloneSigRids | MetaDataFlags.PreserveEventRids | MetaDataFlags.PreservePropertyRids | MetaDataFlags.PreserveTypeSpecRids | MetaDataFlags.PreserveMethodSpecRids | MetaDataFlags.PreserveUSOffsets | MetaDataFlags.PreserveBlobOffsets | MetaDataFlags.PreserveExtraSignatureData | MetaDataFlags.KeepOldMaxStack);
                    moduleWriterOptions.Logger = DummyLogger.NoThrowInstance;
                }

                mainasm.Write(this.textBox1.Text.Replace(".exe", "_fixed.exe"), moduleWriterOptions);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            MessageBox.Show("Total " + totalreplace.ToString() + " resource replaced.");
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            mainexefile.Filter = ".exe |*.exe";
            if (mainexefile.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = mainexefile.FileName;
            }
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            dllfile.Filter = ".dll |*.dll";
            if (dllfile.ShowDialog() == DialogResult.OK)
            {
                this.textBox2.Text = dllfile.FileName;
            }
        }

    }
}