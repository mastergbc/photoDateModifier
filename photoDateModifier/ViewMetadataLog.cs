using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace photoDateModifier
{
    public partial class ViewMetadataLog : Form
    {
        public string JasonFileName { get; set; }

        public ViewMetadataLog(string fileName)
        {
            InitializeComponent();
            JasonFileName = fileName;

            var jsonString = File.ReadAllText(JasonFileName);
            var jsonLoadList = JsonConvert.DeserializeObject<SortedList<int, MetadataModifyLog>>(jsonString);
            var metadataLogList = jsonLoadList.Values.ToList();
            m_dgvMetadataLog.AutoGenerateColumns = true;
            m_dgvMetadataLog.HorizontalScrollingOffset = 20;
            m_dgvMetadataLog.DataBindingComplete += m_dgvMetadataLog_DataBindingComplete;
            m_dgvMetadataLog.DataSource = new BindingSource(metadataLogList, null);
        }

        private void m_dgvMetadataLog_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            int width = 0;
            foreach (DataGridViewColumn column in m_dgvMetadataLog.Columns)
            {
                width += column.Width;
            }

            if(width > 1840)
            {
                width = 1840;
            }
            m_dgvMetadataLog.Width = width + 20;
            this.Width = m_dgvMetadataLog.Width + 40;
        }
    }
}
