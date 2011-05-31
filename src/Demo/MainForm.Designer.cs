namespace Demo
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.tvGroups = new System.Windows.Forms.TreeView();
            this.groupListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showSubscriptionsForThisGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.btAddGroup = new System.Windows.Forms.Button();
            this.btUpdateGroup = new System.Windows.Forms.Button();
            this.btDeleteGroup = new System.Windows.Forms.Button();
            this.btGroupRefresh = new System.Windows.Forms.Button();
            this.dgvTopics = new System.Windows.Forms.DataGridView();
            this.topicListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.btTopicRefresh = new System.Windows.Forms.Button();
            this.btTopicDelete = new System.Windows.Forms.Button();
            this.btTopicUpdate = new System.Windows.Forms.Button();
            this.btTopicAdd = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btDeleteSubs = new System.Windows.Forms.Button();
            this.btUpdateSubs = new System.Windows.Forms.Button();
            this.btAddSubscriptionToTopic = new System.Windows.Forms.Button();
            this.btRefreshSubs = new System.Windows.Forms.Button();
            this.dgvSubscriptions = new System.Windows.Forms.DataGridView();
            this.btAddSubscriptionToGroup = new System.Windows.Forms.Button();
            this.btStartListener = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.lbMessages = new System.Windows.Forms.ListBox();
            this.btRefreshMessages = new System.Windows.Forms.Button();
            this.btTopicPublish = new System.Windows.Forms.Button();
            this.btMessageGet = new System.Windows.Forms.Button();
            this.btStopListener = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.lbListeners = new System.Windows.Forms.ListBox();
            this.groupListContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTopics)).BeginInit();
            this.topicListContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubscriptions)).BeginInit();
            this.SuspendLayout();
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(88, 11);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(438, 20);
            this.txtUrl.TabIndex = 1;
            this.txtUrl.Text = "http://localhost:6156/";
            this.txtUrl.TextChanged += new System.EventHandler(this.txtUrl_TextChanged);
            // 
            // tvGroups
            // 
            this.tvGroups.ContextMenuStrip = this.groupListContextMenu;
            this.tvGroups.HideSelection = false;
            this.tvGroups.Location = new System.Drawing.Point(12, 66);
            this.tvGroups.Name = "tvGroups";
            this.tvGroups.ShowNodeToolTips = true;
            this.tvGroups.Size = new System.Drawing.Size(514, 174);
            this.tvGroups.TabIndex = 2;
            this.tvGroups.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvGroups_AfterSelect);
            // 
            // groupListContextMenu
            // 
            this.groupListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showSubscriptionsForThisGroupToolStripMenuItem});
            this.groupListContextMenu.Name = "groupListContextMenu";
            this.groupListContextMenu.Size = new System.Drawing.Size(253, 26);
            // 
            // showSubscriptionsForThisGroupToolStripMenuItem
            // 
            this.showSubscriptionsForThisGroupToolStripMenuItem.Name = "showSubscriptionsForThisGroupToolStripMenuItem";
            this.showSubscriptionsForThisGroupToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.showSubscriptionsForThisGroupToolStripMenuItem.Text = "Show Subscriptions for this group";
            this.showSubscriptionsForThisGroupToolStripMenuItem.Click += new System.EventHandler(this.showSubscriptionsForThisGroupToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Groups";
            // 
            // btAddGroup
            // 
            this.btAddGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAddGroup.Location = new System.Drawing.Point(244, 40);
            this.btAddGroup.Name = "btAddGroup";
            this.btAddGroup.Size = new System.Drawing.Size(63, 23);
            this.btAddGroup.TabIndex = 4;
            this.btAddGroup.Text = "Add";
            this.btAddGroup.UseVisualStyleBackColor = true;
            this.btAddGroup.Click += new System.EventHandler(this.btAddGroup_Click);
            // 
            // btUpdateGroup
            // 
            this.btUpdateGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btUpdateGroup.Location = new System.Drawing.Point(313, 40);
            this.btUpdateGroup.Name = "btUpdateGroup";
            this.btUpdateGroup.Size = new System.Drawing.Size(63, 23);
            this.btUpdateGroup.TabIndex = 5;
            this.btUpdateGroup.Text = "Update";
            this.btUpdateGroup.UseVisualStyleBackColor = true;
            this.btUpdateGroup.Click += new System.EventHandler(this.btUpdateGroup_Click);
            // 
            // btDeleteGroup
            // 
            this.btDeleteGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btDeleteGroup.Location = new System.Drawing.Point(382, 40);
            this.btDeleteGroup.Name = "btDeleteGroup";
            this.btDeleteGroup.Size = new System.Drawing.Size(63, 23);
            this.btDeleteGroup.TabIndex = 6;
            this.btDeleteGroup.Text = "Delete";
            this.btDeleteGroup.UseVisualStyleBackColor = true;
            this.btDeleteGroup.Click += new System.EventHandler(this.btDeleteGroup_Click);
            // 
            // btGroupRefresh
            // 
            this.btGroupRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btGroupRefresh.Location = new System.Drawing.Point(451, 40);
            this.btGroupRefresh.Name = "btGroupRefresh";
            this.btGroupRefresh.Size = new System.Drawing.Size(75, 23);
            this.btGroupRefresh.TabIndex = 7;
            this.btGroupRefresh.Text = "Refresh";
            this.btGroupRefresh.UseVisualStyleBackColor = true;
            this.btGroupRefresh.Click += new System.EventHandler(this.btGroupRefresh_Click);
            // 
            // dgvTopics
            // 
            this.dgvTopics.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTopics.ContextMenuStrip = this.topicListContextMenu;
            this.dgvTopics.Location = new System.Drawing.Point(532, 66);
            this.dgvTopics.MultiSelect = false;
            this.dgvTopics.Name = "dgvTopics";
            this.dgvTopics.RowTemplate.Height = 24;
            this.dgvTopics.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTopics.Size = new System.Drawing.Size(573, 174);
            this.dgvTopics.TabIndex = 8;
            // 
            // topicListContextMenu
            // 
            this.topicListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.topicListContextMenu.Name = "groupListContextMenu";
            this.topicListContextMenu.Size = new System.Drawing.Size(177, 26);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(176, 22);
            this.toolStripMenuItem1.Text = "Show subscriptions";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.showSubscriptionsForTopicMenuItem_Click);
            // 
            // btTopicRefresh
            // 
            this.btTopicRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btTopicRefresh.Location = new System.Drawing.Point(1030, 40);
            this.btTopicRefresh.Name = "btTopicRefresh";
            this.btTopicRefresh.Size = new System.Drawing.Size(75, 23);
            this.btTopicRefresh.TabIndex = 9;
            this.btTopicRefresh.Text = "Refresh";
            this.btTopicRefresh.UseVisualStyleBackColor = true;
            this.btTopicRefresh.Click += new System.EventHandler(this.btTopicRefresh_Click);
            // 
            // btTopicDelete
            // 
            this.btTopicDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btTopicDelete.Location = new System.Drawing.Point(961, 40);
            this.btTopicDelete.Name = "btTopicDelete";
            this.btTopicDelete.Size = new System.Drawing.Size(63, 23);
            this.btTopicDelete.TabIndex = 12;
            this.btTopicDelete.Text = "Delete";
            this.btTopicDelete.UseVisualStyleBackColor = true;
            this.btTopicDelete.Click += new System.EventHandler(this.btTopicDelete_Click);
            // 
            // btTopicUpdate
            // 
            this.btTopicUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btTopicUpdate.Location = new System.Drawing.Point(892, 40);
            this.btTopicUpdate.Name = "btTopicUpdate";
            this.btTopicUpdate.Size = new System.Drawing.Size(63, 23);
            this.btTopicUpdate.TabIndex = 11;
            this.btTopicUpdate.Text = "Update";
            this.btTopicUpdate.UseVisualStyleBackColor = true;
            this.btTopicUpdate.Click += new System.EventHandler(this.btTopicUpdate_Click);
            // 
            // btTopicAdd
            // 
            this.btTopicAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btTopicAdd.Location = new System.Drawing.Point(823, 40);
            this.btTopicAdd.Name = "btTopicAdd";
            this.btTopicAdd.Size = new System.Drawing.Size(63, 23);
            this.btTopicAdd.TabIndex = 10;
            this.btTopicAdd.Text = "Add";
            this.btTopicAdd.UseVisualStyleBackColor = true;
            this.btTopicAdd.Click += new System.EventHandler(this.btTopicAdd_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(529, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Group\'s topics";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Pub sub hub:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 255);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Subscriptions";
            // 
            // btDeleteSubs
            // 
            this.btDeleteSubs.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btDeleteSubs.Location = new System.Drawing.Point(382, 245);
            this.btDeleteSubs.Name = "btDeleteSubs";
            this.btDeleteSubs.Size = new System.Drawing.Size(63, 23);
            this.btDeleteSubs.TabIndex = 19;
            this.btDeleteSubs.Text = "Delete";
            this.btDeleteSubs.UseVisualStyleBackColor = true;
            this.btDeleteSubs.Click += new System.EventHandler(this.btDeleteSubs_Click);
            // 
            // btUpdateSubs
            // 
            this.btUpdateSubs.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btUpdateSubs.Location = new System.Drawing.Point(313, 245);
            this.btUpdateSubs.Name = "btUpdateSubs";
            this.btUpdateSubs.Size = new System.Drawing.Size(63, 23);
            this.btUpdateSubs.TabIndex = 18;
            this.btUpdateSubs.Text = "Update";
            this.btUpdateSubs.UseVisualStyleBackColor = true;
            this.btUpdateSubs.Click += new System.EventHandler(this.btUpdateSubs_Click);
            // 
            // btAddSubscriptionToTopic
            // 
            this.btAddSubscriptionToTopic.Location = new System.Drawing.Point(719, 40);
            this.btAddSubscriptionToTopic.Name = "btAddSubscriptionToTopic";
            this.btAddSubscriptionToTopic.Size = new System.Drawing.Size(98, 23);
            this.btAddSubscriptionToTopic.TabIndex = 17;
            this.btAddSubscriptionToTopic.Text = "Add subscription";
            this.btAddSubscriptionToTopic.UseVisualStyleBackColor = true;
            this.btAddSubscriptionToTopic.Click += new System.EventHandler(this.btAddSubscriptionToTopic_Click);
            // 
            // btRefreshSubs
            // 
            this.btRefreshSubs.Location = new System.Drawing.Point(451, 245);
            this.btRefreshSubs.Name = "btRefreshSubs";
            this.btRefreshSubs.Size = new System.Drawing.Size(75, 23);
            this.btRefreshSubs.TabIndex = 16;
            this.btRefreshSubs.Text = "Refresh";
            this.btRefreshSubs.UseVisualStyleBackColor = true;
            this.btRefreshSubs.Click += new System.EventHandler(this.btRefreshSubs_Click);
            // 
            // dgvSubscriptions
            // 
            this.dgvSubscriptions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSubscriptions.Location = new System.Drawing.Point(12, 271);
            this.dgvSubscriptions.MultiSelect = false;
            this.dgvSubscriptions.Name = "dgvSubscriptions";
            this.dgvSubscriptions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSubscriptions.Size = new System.Drawing.Size(514, 225);
            this.dgvSubscriptions.TabIndex = 15;
            this.dgvSubscriptions.SelectionChanged += new System.EventHandler(this.dgvSubscriptions_SelectionChanged);
            // 
            // btAddSubscriptionToGroup
            // 
            this.btAddSubscriptionToGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAddSubscriptionToGroup.Location = new System.Drawing.Point(140, 40);
            this.btAddSubscriptionToGroup.Name = "btAddSubscriptionToGroup";
            this.btAddSubscriptionToGroup.Size = new System.Drawing.Size(98, 23);
            this.btAddSubscriptionToGroup.TabIndex = 23;
            this.btAddSubscriptionToGroup.Text = "Add subscription";
            this.btAddSubscriptionToGroup.UseVisualStyleBackColor = true;
            this.btAddSubscriptionToGroup.Click += new System.EventHandler(this.btAddSubscriptionToGroup_Click);
            // 
            // btStartListener
            // 
            this.btStartListener.Location = new System.Drawing.Point(209, 245);
            this.btStartListener.Name = "btStartListener";
            this.btStartListener.Size = new System.Drawing.Size(98, 23);
            this.btStartListener.TabIndex = 22;
            this.btStartListener.Text = "Start listener";
            this.btStartListener.UseVisualStyleBackColor = true;
            this.btStartListener.Click += new System.EventHandler(this.btStartListener_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(529, 255);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(122, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Subscription\'s messages";
            // 
            // lbMessages
            // 
            this.lbMessages.FormattingEnabled = true;
            this.lbMessages.Location = new System.Drawing.Point(532, 271);
            this.lbMessages.Name = "lbMessages";
            this.lbMessages.Size = new System.Drawing.Size(573, 225);
            this.lbMessages.TabIndex = 25;
            this.lbMessages.DoubleClick += new System.EventHandler(this.lbMessages_DoubleClick);
            // 
            // btRefreshMessages
            // 
            this.btRefreshMessages.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btRefreshMessages.Location = new System.Drawing.Point(1030, 245);
            this.btRefreshMessages.Name = "btRefreshMessages";
            this.btRefreshMessages.Size = new System.Drawing.Size(75, 23);
            this.btRefreshMessages.TabIndex = 26;
            this.btRefreshMessages.Text = "Refresh";
            this.btRefreshMessages.UseVisualStyleBackColor = true;
            this.btRefreshMessages.Click += new System.EventHandler(this.btRefreshMessages_Click);
            // 
            // btTopicPublish
            // 
            this.btTopicPublish.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btTopicPublish.Location = new System.Drawing.Point(615, 40);
            this.btTopicPublish.Name = "btTopicPublish";
            this.btTopicPublish.Size = new System.Drawing.Size(98, 23);
            this.btTopicPublish.TabIndex = 27;
            this.btTopicPublish.Text = "Publish message";
            this.btTopicPublish.UseVisualStyleBackColor = true;
            this.btTopicPublish.Click += new System.EventHandler(this.btTopicPublish_Click);
            // 
            // btMessageGet
            // 
            this.btMessageGet.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btMessageGet.Location = new System.Drawing.Point(950, 245);
            this.btMessageGet.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btMessageGet.Name = "btMessageGet";
            this.btMessageGet.Size = new System.Drawing.Size(75, 23);
            this.btMessageGet.TabIndex = 28;
            this.btMessageGet.Text = "Get";
            this.btMessageGet.UseVisualStyleBackColor = true;
            this.btMessageGet.Click += new System.EventHandler(this.btMessageGet_Click);
            // 
            // btStopListener
            // 
            this.btStopListener.Location = new System.Drawing.Point(451, 502);
            this.btStopListener.Name = "btStopListener";
            this.btStopListener.Size = new System.Drawing.Size(75, 23);
            this.btStopListener.TabIndex = 29;
            this.btStopListener.Text = "Stop";
            this.btStopListener.UseVisualStyleBackColor = true;
            this.btStopListener.Click += new System.EventHandler(this.btStopListener_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 507);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 13);
            this.label6.TabIndex = 30;
            this.label6.Text = "Callback listeners";
            // 
            // lbListeners
            // 
            this.lbListeners.FormattingEnabled = true;
            this.lbListeners.Location = new System.Drawing.Point(12, 524);
            this.lbListeners.Name = "lbListeners";
            this.lbListeners.Size = new System.Drawing.Size(514, 82);
            this.lbListeners.TabIndex = 31;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(963, 614);
            this.Controls.Add(this.lbListeners);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btStopListener);
            this.Controls.Add(this.btTopicPublish);
            this.Controls.Add(this.btRefreshMessages);
            this.Controls.Add(this.lbMessages);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btAddSubscriptionToGroup);
            this.Controls.Add(this.btMessageGet);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btDeleteSubs);
            this.Controls.Add(this.btUpdateSubs);
            this.Controls.Add(this.btStartListener);
            this.Controls.Add(this.btAddSubscriptionToTopic);
            this.Controls.Add(this.btRefreshSubs);
            this.Controls.Add(this.dgvSubscriptions);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btTopicDelete);
            this.Controls.Add(this.btTopicUpdate);
            this.Controls.Add(this.btTopicAdd);
            this.Controls.Add(this.btTopicRefresh);
            this.Controls.Add(this.dgvTopics);
            this.Controls.Add(this.btGroupRefresh);
            this.Controls.Add(this.btDeleteGroup);
            this.Controls.Add(this.btUpdateGroup);
            this.Controls.Add(this.btAddGroup);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tvGroups);
            this.Controls.Add(this.txtUrl);
            this.Name = "MainForm";
            this.Text = "Demo";
            this.groupListContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTopics)).EndInit();
            this.topicListContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubscriptions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.TreeView tvGroups;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btAddGroup;
        private System.Windows.Forms.Button btUpdateGroup;
        private System.Windows.Forms.Button btDeleteGroup;
        private System.Windows.Forms.Button btGroupRefresh;
        private System.Windows.Forms.DataGridView dgvTopics;
        private System.Windows.Forms.Button btTopicRefresh;
        private System.Windows.Forms.Button btTopicDelete;
        private System.Windows.Forms.Button btTopicUpdate;
        private System.Windows.Forms.Button btTopicAdd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btDeleteSubs;
        private System.Windows.Forms.Button btUpdateSubs;
        private System.Windows.Forms.Button btAddSubscriptionToTopic;
        private System.Windows.Forms.Button btRefreshSubs;
        private System.Windows.Forms.DataGridView dgvSubscriptions;
        private System.Windows.Forms.Button btAddSubscriptionToGroup;
        private System.Windows.Forms.Button btStartListener;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox lbMessages;
        private System.Windows.Forms.Button btRefreshMessages;
        private System.Windows.Forms.Button btTopicPublish;
        private System.Windows.Forms.Button btMessageGet;
        private System.Windows.Forms.ContextMenuStrip groupListContextMenu;
        private System.Windows.Forms.ToolStripMenuItem showSubscriptionsForThisGroupToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip topicListContextMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.Button btStopListener;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox lbListeners;
    }
}

