using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MultiFileWatcher.Configuration;
using System.Configuration;

namespace MultiFileWatcher {
  public partial class MainForm : Form {

    List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

    private void CreateFileWatchers() {

      MultiWatcherConfigurationSection config = (MultiWatcherConfigurationSection)ConfigurationManager.GetSection("MultiWatcherConfigurationSection");
      foreach (FileElement fe in config.Files) {

        FileSystemWatcher fsw = new FileSystemWatcher();
        fsw.Path = fe.Path;
        fsw.NotifyFilter = fe.NotifyFilter;
        fsw.Changed += new FileSystemEventHandler(fsw_Changed);
        fsw.Deleted += new FileSystemEventHandler(fsw_Deleted);
        fsw.Renamed += new RenamedEventHandler(fsw_Renamed);
        fsw.EnableRaisingEvents = true;
        watchers.Add(fsw);

      }

    }

    void fsw_Renamed(object sender, RenamedEventArgs e) {
      txtActivity.Invoke(
        (MethodInvoker)delegate {
        txtActivity.AppendText(e.ChangeType.ToString() + " : " + e.Name + "\n");
      }
      );
    }

    void fsw_Deleted(object sender, FileSystemEventArgs e) {
      txtActivity.Invoke(
        (MethodInvoker)delegate {
        txtActivity.AppendText(e.ChangeType.ToString() + " : " + e.Name + "\n");
      }
      );
    }

    void fsw_Changed(object sender, FileSystemEventArgs e) {
      txtActivity.Invoke(
        (MethodInvoker)delegate {
          txtActivity.AppendText(e.ChangeType.ToString() + " : " + e.Name + "\n");
        }
      );
    }

    private void BuildListView() {
      foreach (FileSystemWatcher fsw in watchers) {
        ListViewItem lvi = new ListViewItem();
        lvi.Text = fsw.Path;
        lvi.SubItems.Add(fsw.NotifyFilter.ToString());
        lvwFileWatchers.Items.Add(lvi);
      }
    }

    public MainForm() {
      InitializeComponent();
      CreateFileWatchers();
      BuildListView();
    }

    private void button1_Click(object sender, EventArgs e) {
      Close();
    }

  }

}