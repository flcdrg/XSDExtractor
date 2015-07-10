namespace MultiFileWatcher {
  partial class MainForm {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.lvwFileWatchers = new System.Windows.Forms.ListView();
      this.button1 = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.txtActivity = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
      this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
      this.SuspendLayout();
      // 
      // lvwFileWatchers
      // 
      this.lvwFileWatchers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.lvwFileWatchers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
      this.lvwFileWatchers.Location = new System.Drawing.Point(12, 39);
      this.lvwFileWatchers.Name = "lvwFileWatchers";
      this.lvwFileWatchers.Size = new System.Drawing.Size(427, 157);
      this.lvwFileWatchers.TabIndex = 0;
      this.lvwFileWatchers.UseCompatibleStateImageBehavior = false;
      this.lvwFileWatchers.View = System.Windows.Forms.View.Details;
      // 
      // button1
      // 
      this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.button1.Location = new System.Drawing.Point(364, 331);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 1;
      this.button1.Text = "Close";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(13, 13);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(171, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "List of files / folders being watched";
      // 
      // txtActivity
      // 
      this.txtActivity.Location = new System.Drawing.Point(12, 224);
      this.txtActivity.Multiline = true;
      this.txtActivity.Name = "txtActivity";
      this.txtActivity.ReadOnly = true;
      this.txtActivity.Size = new System.Drawing.Size(427, 80);
      this.txtActivity.TabIndex = 3;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(16, 203);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(41, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "Activity";
      // 
      // columnHeader1
      // 
      this.columnHeader1.Text = "Path";
      // 
      // columnHeader2
      // 
      this.columnHeader2.Text = "NotifyFilter";
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(451, 366);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.txtActivity);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.lvwFileWatchers);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.Name = "MainForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "MultiFileWatcher Main Form";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListView lvwFileWatchers;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtActivity;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private System.Windows.Forms.ColumnHeader columnHeader2;
  }
}

